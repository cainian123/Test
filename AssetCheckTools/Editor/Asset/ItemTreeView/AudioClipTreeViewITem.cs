using AssetCheckTools.Editor.Asset.Data;
using AssetCheckTools.Editor.Asset.Tree;
using UnityEngine;

namespace AssetCheckTools.Editor.Asset.ItemTreeView
{
    public class AudioClipTreeViewITem : TreeViewBaseItem
    {
        internal AudioClipTreeViewITem(AudioInfo info): base(info)
        {
            
        }
        
        internal Color GetColor(AudioClipListTree.SortOption sortOption)
        {
            if (asset is AudioInfo info)
            {
                return itemColor;
            }
            return m_color;
        }
    }
}