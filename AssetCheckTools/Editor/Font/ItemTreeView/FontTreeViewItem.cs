using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetCheckTools.Editor.Font.ItemTreeView
{
    public sealed class FontTreeViewItem : TreeViewItem
    {

        private FontInfo _fontInfo;

        public FontInfo FontInfo => _fontInfo;

        
        internal FontTreeViewItem() : base(-1, -1) { }
        
        internal FontTreeViewItem(FontInfo a) : base(a?.fullAssetName.GetHashCode() ?? Random.Range(int.MinValue, int.MaxValue), 0, a != null ? a.displayName : "failed")
        {
            _fontInfo = a;
            displayName = a.displayName;
            icon = AssetDatabase.GetCachedIcon(a.fullAssetName) as Texture2D;
        }
    }
}