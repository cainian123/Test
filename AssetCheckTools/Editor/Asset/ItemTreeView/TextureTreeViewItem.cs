using AssetCheckTools.Editor.Asset.Data;
using AssetCheckTools.Editor.Asset.Tree;
using UnityEngine;

namespace AssetCheckTools.Editor.Asset.ItemTreeView
{
    public class TextureTreeViewItem : TreeViewBaseItem
    {
        internal TextureTreeViewItem(TextureInfo info): base(info)
        {
            
        }
        
        internal Color GetColor(TextureListTree.SortOption num)
        {
            if (asset is TextureInfo info)
            {
                if (num == TextureListTree.SortOption.AndroidFormat)
                {
                    return info.GetAndroidFormatColor();
                }
                else if(num == TextureListTree.SortOption.IosFormat)
                {
                    return info.GetIOSFormatColor();    
                }
                else if (num == TextureListTree.SortOption.WidthAndHeight)
                {
                    return info.GetSizeColor();
                }
                return itemColor;
            }
            return m_color;
        }
    }
}