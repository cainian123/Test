using AssetStream.Editor.AssetBundleSetting.ResourceModule.Data;
using UnityEditor;
using UnityEngine;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.TreeViewItem
{
    public class AssetInfoEntryTreeViewItem : BaseTreeViewItem
    {
        public Texture2D assetIcon;
        public AssetBaseInfo entry;
        public AssetInfoEntryTreeViewItem(int id, int depth, string displayName) : base(id, depth, displayName)
        {
        }

        public AssetInfoEntryTreeViewItem(AssetBaseInfo e, int d) : base(e == null ? 0 : (e.FullAssetName).GetHashCode(), d, e == null ? "[Missing Reference]" : e.FullAssetName)
        {
            entry = e;
            assetIcon = e == null ? null : AssetDatabase.GetCachedIcon(e.FullAssetName) as Texture2D;
        }

     
    }
}