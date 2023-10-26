using UnityEditor;
using UnityEngine;

namespace Rehawk.Kite
{
    public static class AssetHelper
    {
        public static T[] FindAssetsOfType<T>() where T : Object
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            T[] a = new T[guids.Length];
            for(int i =0;i<guids.Length;i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }
 
            return a;
        }
    }
}