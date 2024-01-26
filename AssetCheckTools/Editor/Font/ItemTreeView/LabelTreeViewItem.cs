using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetCheckTools.Editor.Font.ItemTreeView
{
    public class LabelTreeViewItem : TreeViewItem
    {
        public string path;
        internal LabelTreeViewItem(string a) : base(a?.GetHashCode() ?? Random.Range(int.MinValue, int.MaxValue), 0,a)
        {
            path = a;
          
        }
    }
}