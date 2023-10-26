using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Rehawk.Kite
{
    public struct MenuItemInfo
    {
        public bool isValid { get; }
        public GUIContent content;
        public bool separator;
        public bool selected;
        public GenericMenu.MenuFunction func;
        public GenericMenu.MenuFunction2 func2;
        public object userData;

        public MenuItemInfo(GUIContent c, bool sep, bool slc, GenericMenu.MenuFunction f1, GenericMenu.MenuFunction2 f2, object o)
        {
            isValid = true;
            content = c;
            separator = sep;
            selected = slc;
            func = f1;
            func2 = f2;
            userData = o;
        }
    }

    public class GenericMenuBrowser : PopupWindowContent
    {
        private readonly Color hoverColor = new Color(0.5f, 0.5f, 1, 0.3f);

        ///----------------------------------------------------------------------------------------------
        private GenericMenu boundMenu;

        private Node currentNode;

        private readonly string headerTitle;
        private float helpRectRequiredHeight;
        private int hoveringIndex;
        private int lastHoveringIndex;
        private string lastSearch;
        private List<Node> leafNodes;
        private float loadProgress;
        private Node rootNode;
        private Vector2 scrollPos;
        private string search;
        private EditorWindow wasFocusedWindow;

        private bool willRepaint;

        public GenericMenuBrowser(GenericMenu newMenu, string title)
        {
            Current = this;
            headerTitle = title;
            rootNode = new Node();
            currentNode = rootNode;
            lastHoveringIndex = -1;
            hoveringIndex = -1;
            SetMenu(newMenu);
        }

        public static GenericMenuBrowser Current { get; private set; }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(480, Mathf.Max(500 + helpRectRequiredHeight, 500));
        }

        public static void ShowAsync(Vector2 pos, string title, Func<GenericMenu> getMenu)
        {
            GenericMenuBrowser browser = new GenericMenuBrowser(null, title);
            
            Task.Run(getMenu)
                .ContinueWith(m => browser.SetMenu(m.Result));
            
            PopupWindow.Show(new Rect(pos.x, pos.y, 0, 0), browser);
        }

        public void SetMenu(GenericMenu newMenu)
        {
            if (newMenu == null)
            {
                return;
            }

            willRepaint = true;
            boundMenu = newMenu;
            Current.GenerateTree();
        }

        public override void OnOpen()
        {
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
            wasFocusedWindow = EditorWindow.focusedWindow;
        }

        public override void OnClose()
        {
            EditorApplication.update -= OnEditorUpdate;
            EditorWindow.FocusWindowIfItsOpen(wasFocusedWindow != null ? wasFocusedWindow.GetType() : null);

            Current = null;
        }

        private void OnEditorUpdate()
        {
            if (willRepaint)
            {
                willRepaint = false;
                editorWindow.Repaint();
            }
        }

        private void GenerateTree()
        {
            MenuItemInfo[] tempItems = GetMenuItems(boundMenu);
            
            var tempLeafNodes = new List<Node>();
            var tempRoot = new Node();
            
            for (int i = 0; i < tempItems.Length; i++)
            {
                MenuItemInfo item = tempItems[i];
                string itemPath = item.content.text;
                string[] parts = itemPath.Split('/');
                Node current = tempRoot;
                
                string path = string.Empty;
                for (int j = 0; j < parts.Length; j++)
                {
                    string part = parts[j];
                    path += '/' + part;

                    if (!current.children.TryGetValue(part, out Node child))
                    {
                        child = new Node
                        {
                            name = part, 
                            parent = current,
                            fullPath = path
                        };
                        
                        current.children[part] = child;
                        if (part == parts.Last())
                        {
                            child.item = item;
                            tempLeafNodes.Add(child);
                        }
                    }

                    current = child;
                }
            }

            leafNodes = tempLeafNodes;
            rootNode = tempRoot;
            currentNode = rootNode;
        }

        public static MenuItemInfo[] GetMenuItems(GenericMenu menu)
        {
            FieldInfo itemField = typeof(GenericMenu).GetField("m_MenuItems", BindingFlags.Instance | BindingFlags.NonPublic);
            
            IList items = itemField.GetValue(menu) as IList;
            
            if (items.Count == 0)
            {
                return new MenuItemInfo[0];
            }

            Type itemType = items[0].GetType();
            
            Func<object, GUIContent> contentGetter = ReflectionUtils.GetFieldGetter<object, GUIContent>(itemType.GetField("content"));
            Func<object, bool> sepGetter = ReflectionUtils.GetFieldGetter<object, bool>(itemType.GetField("separator"));
            Func<object, bool> selectedGetter = ReflectionUtils.GetFieldGetter<object, bool>(itemType.GetField("on"));
            Func<object, GenericMenu.MenuFunction> func1Getter = ReflectionUtils.GetFieldGetter<object, GenericMenu.MenuFunction>(itemType.GetField("func"));
            Func<object, GenericMenu.MenuFunction2> func2Getter = ReflectionUtils.GetFieldGetter<object, GenericMenu.MenuFunction2>(itemType.GetField("func2"));
            Func<object, object> dataGetter = ReflectionUtils.GetFieldGetter<object, object>(itemType.GetField("userData"));

            var result = new List<MenuItemInfo>();
            
            foreach (object item in items)
            {
                GUIContent content = contentGetter(item);
                bool separator = sepGetter(item);
                bool selected = selectedGetter(item);
                GenericMenu.MenuFunction func1 = func1Getter(item);
                GenericMenu.MenuFunction2 func2 = func2Getter(item);
                object userData = dataGetter(item);
                result.Add(new MenuItemInfo(content, separator, selected, func1, func2, userData));
            }

            return result.ToArray();
        }

        private Node GenerateSearchResults()
        {
            var searchRootNode = new Node { name = "Search Root" };
            
            searchRootNode.children = leafNodes
                .Where(x => StringUtils.SearchMatch(search, x.name, x.category))
                .OrderBy(x => StringUtils.ScoreSearchMatch(search, x.name, x.category))
                .ToDictionary(x => x.fullPath, y => y);
            
            return searchRootNode;
        }

        //Show stuff
        public override void OnGUI(Rect rect)
        {
            Event e = Event.current;
            EditorGUIUtility.SetIconSize(Vector2.zero);
            hoveringIndex = Mathf.Clamp(hoveringIndex, -1, currentNode.children.Count - 1);
            GUIHelper.Draw(rect, Styles.ShadowedBackground);

            int headerHeight = currentNode.parent != null ? 95 : 60;
            Rect headerRect = new Rect(0, 0, rect.width, headerHeight);
            DoHeader(headerRect, e);

            Rect treeRect = Rect.MinMaxRect(0, headerHeight, rect.width, rect.height);
            DoTree(treeRect, e);

            //handle the events
            HandeEvents(e);

            EditorGUIUtility.SetIconSize(Vector2.zero);
        }

        //...
        private void DoHeader(Rect headerRect, Event e)
        {
            //HEADER
            GUILayout.Space(5);
            GUILayout.Label(string.Format("<color=#{0}><size=14><b>{1}</b></size></color>", "dddddd", headerTitle), Styles.TopCenterLabel);

            ///SEARCH
            if (e.keyCode == KeyCode.DownArrow)
            {
                GUIUtility.keyboardControl = 0;
            }

            if (e.keyCode == KeyCode.UpArrow)
            {
                GUIUtility.keyboardControl = 0;
            }

            if (e.keyCode == KeyCode.Return)
            {
                GUIUtility.keyboardControl = 0;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(4);
            GUI.SetNextControlName("SearchToolbar");
            search = SearchField(search);

            GUILayout.EndHorizontal();
            BoldSeparator();

            ///BACK
            if (currentNode.parent != null && string.IsNullOrEmpty(search))
            {
                GUILayout.BeginHorizontal("box");
                if (GUILayout.Button(string.Format("<b><size=14>◄ {0}/{1}</size></b>", currentNode.parent.name, currentNode.name), Styles.LeftLabel))
                {
                    currentNode = currentNode.parent;
                }

                GUILayout.EndHorizontal();
                Rect lastRect = GUILayoutUtility.GetLastRect();
                if (lastRect.Contains(e.mousePosition))
                {
                    GUI.color = hoverColor;
                    GUI.DrawTexture(lastRect, EditorGUIUtility.whiteTexture);
                    GUI.color = Color.white;
                    willRepaint = true;
                    hoveringIndex = -1;
                }
            }
        }

        ///A Search Field
        public static string SearchField(string search)
        {
            GUILayout.BeginHorizontal();
            search = EditorGUILayout.TextField(search, Styles.ToolbarSearchTextField);
            if (GUILayout.Button(string.Empty, Styles.ToolbarSearchCancelButton))
            {
                search = string.Empty;
                GUIUtility.keyboardControl = 0;
            }

            GUILayout.EndHorizontal();
            return search;
        }

        ///A thin separator
        public static void Separator()
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            GUILayout.Space(7);
            GUI.color = new Color(0, 0, 0, 0.3f);
            GUI.DrawTexture(Rect.MinMaxRect(lastRect.xMin, lastRect.yMax + 4, lastRect.xMax, lastRect.yMax + 6), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        public static void BoldSeparator()
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            GUILayout.Space(14);
            GUI.color = new Color(0, 0, 0, 0.3f);
            GUI.DrawTexture(new Rect(0, lastRect.yMax + 6, Screen.width, 4), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(0, lastRect.yMax + 6, Screen.width, 1), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(0, lastRect.yMax + 9, Screen.width, 1), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        //THE TREE
        private void DoTree(Rect treeRect, Event e)
        {
            if (search != lastSearch)
            {
                lastSearch = search;
                hoveringIndex = -1;
                
                if (!string.IsNullOrEmpty(search))
                {
                    Task.Run(GenerateSearchResults)
                        .ContinueWith(task =>
                        {
                            currentNode = task.Result;
                            if (Current != null)
                            {
                                willRepaint = true;
                            }
                        });
                }
                else
                {
                    currentNode = rootNode;
                }
            }


            GUILayout.BeginArea(treeRect);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
            GUILayout.BeginVertical();

            ///----------------------------------------------------------------------------------------------

            int i = 0;
            bool itemAdded = false;
            string lastSearchCategory = null;
            bool isSearch = !string.IsNullOrEmpty(search);
            foreach (KeyValuePair<string, Node> childPair in currentNode.children)
            {
                if (isSearch && i >= 200)
                {
                    EditorGUILayout.HelpBox("There are more than 200 results. Please try refine your search input.", MessageType.Info);
                    break;
                }

                Node node = childPair.Value;
                MemberInfo memberInfo = node.isLeaf ? node.item.userData as MemberInfo : null;
                bool isDisabled = node.isLeaf && node.item.func == null && node.item.func2 == null;

                // var icon = node.isLeaf ? node.item.content.image : Icons.folderIcon;
                // if (icon == null && memberInfo != null)
                // {
                //     icon = TypePrefs.GetTypeIcon(memberInfo);
                // }

                //when within search, show category on top
                if (isSearch)
                {
                    string searchCategory = lastSearchCategory;
                    if (memberInfo == null || memberInfo is Type)
                    {
                        searchCategory = node.parent.fullPath != null ? node.parent.fullPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() : null;
                    }
                    else
                    {
                        searchCategory = memberInfo.ReflectedType.Name;
                    }

                    if (searchCategory != lastSearchCategory)
                    {
                        lastSearchCategory = searchCategory;
                        GUI.color = Color.black;
                        GUILayout.BeginHorizontal("box");
                        GUI.color = Color.white;
                        GUILayout.Label(searchCategory, Styles.LeftLabel, GUILayout.Height(16));
                        GUILayout.EndHorizontal();
                    }
                }
                //

                if (node.isLeaf && node.item.separator)
                {
                    if (itemAdded)
                    {
                        Separator();
                    }

                    continue;
                }

                itemAdded = true;

                GUI.color = Color.clear;
                GUILayout.BeginHorizontal("box");
                GUI.color = Color.white;

                //Prefix icon
                // GUILayout.Label(icon, GUILayout.Width(22), GUILayout.Height(16));
                GUI.enabled = !isDisabled;

                //Content
                string label = node.name;
                string hexColor = "#B8B8B8";
                hexColor = isDisabled ? "#666666" : hexColor;
                string text = string.Format("<color={0}><size=11>{1}</size></color>", hexColor, !node.isLeaf ? string.Format("<b>{0}</b>", label) : label);
                GUILayout.Label(text, Styles.LeftLabel, GUILayout.Width(0), GUILayout.ExpandWidth(true));
                GUILayout.Label(node.isLeaf ? "●" : "►", Styles.LeftLabel, GUILayout.Width(20));
                GUILayout.EndHorizontal();

                Rect elementRect = GUILayoutUtility.GetLastRect();
                if (e.type == EventType.MouseDown && e.button == 0 && elementRect.Contains(e.mousePosition))
                {
                    e.Use();
                    if (node.isLeaf)
                    {
                        ExecuteItemFunc(node.item);
                        break;
                    }

                    currentNode = node;
                    hoveringIndex = 0;
                    break;
                }

                if (e.type == EventType.MouseMove && elementRect.Contains(e.mousePosition))
                {
                    hoveringIndex = i;
                }

                if (hoveringIndex == i)
                {
                    GUI.color = hoverColor;
                    GUI.DrawTexture(elementRect, EditorGUIUtility.whiteTexture);
                    GUI.color = Color.white;
                }

                i++;
                GUI.enabled = true;
            }

            if (hoveringIndex != lastHoveringIndex)
            {
                willRepaint = true;
                lastHoveringIndex = hoveringIndex;
            }

            if (!itemAdded)
            {
                GUILayout.Label("No results to display with current search and filter combination");
            }

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        ///----------------------------------------------------------------------------------------------

        //Executes the item's registered delegate
        private void ExecuteItemFunc(MenuItemInfo item)
        {
            if (item.func != null)
            {
                item.func();
            }
            else
            {
                item.func2(item.userData);
            }

            editorWindow.Close();
        }

        //Handle events
        private void HandeEvents(Event e)
        {
            //Go back with right click as well...
            if (e.type == EventType.MouseDown && e.button == 1)
            {
                if (currentNode.parent != null)
                {
                    currentNode = currentNode.parent;
                }

                e.Use();
            }

            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.RightArrow || e.keyCode == KeyCode.Return)
                {
                    if (hoveringIndex >= 0)
                    {
                        Node next = currentNode.children.Values.ToList()[hoveringIndex];
                        if (e.keyCode == KeyCode.Return && next.isLeaf)
                        {
                            ExecuteItemFunc(next.item);
                        }
                        else if (!next.isLeaf)
                        {
                            currentNode = next;
                            hoveringIndex = 0;
                        }

                        e.Use();
                        return;
                    }
                }

                if (e.keyCode == KeyCode.LeftArrow)
                {
                    Node previous = currentNode.parent;
                    if (previous != null)
                    {
                        hoveringIndex = currentNode.parent.children.Values.ToList().IndexOf(currentNode);
                        currentNode = previous;
                    }

                    e.Use();
                    return;
                }

                if (e.keyCode == KeyCode.DownArrow)
                {
                    hoveringIndex++;
                    e.Use();
                    return;
                }

                if (e.keyCode == KeyCode.UpArrow)
                {
                    hoveringIndex = Mathf.Max(hoveringIndex - 1, 0);
                    e.Use();
                    return;
                }

                if (e.keyCode == KeyCode.Escape)
                {
                    editorWindow.Close();
                    e.Use();
                    return;
                }

                EditorGUI.FocusTextInControl("SearchToolbar");
            }
        }

        //class node used in browser
        private class Node
        {
            public Dictionary<string, Node> children;
            public string fullPath;
            public MenuItemInfo item;
            public string name;
            public Node parent;
            public bool unfolded;

            public Node()
            {
                children = new Dictionary<string, Node>(StringComparer.Ordinal);
                unfolded = true;
            }

            //Leafs have menu items
            public bool isLeaf
            {
                get { return item.isValid; }
            }

            public string category
            {
                get { return parent != null ? parent.fullPath : string.Empty; }
            }
        }
    }
}