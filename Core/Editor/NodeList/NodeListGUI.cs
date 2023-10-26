namespace Rehawk.Kite.NodeList
{
    public static class NodeListGUI
    {
        public static int IndexOfChangedItem { get; internal set; }

        static NodeListGUI()
        {
            IndexOfChangedItem = -1;
        }
    }
}