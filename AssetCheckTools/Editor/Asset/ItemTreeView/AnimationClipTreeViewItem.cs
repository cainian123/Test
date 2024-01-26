using AssetCheckTools.Editor.Asset.Data;
using AssetCheckTools.Editor.Asset.Tree;
using UnityEngine;

namespace AssetCheckTools.Editor.Asset.ItemTreeView
{
    public class AnimationClipTreeViewItem : TreeViewBaseItem
    {
        internal AnimationClipTreeViewItem(AnimationClipInfo info): base(info)
        {
            
        }
        
        internal Color GetColor(AnimationClipListTree.SortOption num)
        {
            if (asset is AnimationClipInfo info)
            {
                if (num == AnimationClipListTree.SortOption.FrameRate)
                {
                    return info.GetFrameRateColor();
                }
                
                return itemColor;
            }
            return m_color;
        }
    }
}