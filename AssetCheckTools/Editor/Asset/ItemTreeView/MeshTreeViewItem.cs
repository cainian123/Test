using AssetCheckTools.Editor.Asset.Data;
using AssetCheckTools.Editor.Asset.Tree;
using UnityEngine;

namespace AssetCheckTools.Editor.Asset.ItemTreeView
{
    public class MeshTreeViewItem : TreeViewBaseItem
    {
        internal MeshTreeViewItem(MeshInfo info): base(info)
        {
            
        }
        
        internal Color GetColor(MeshListTree.SortOption sortOption)
        {
            if (asset is MeshInfo info)
            {
                if (sortOption == MeshListTree.SortOption.Vertex)
                {
                    return info.GetVertexColor();
                }
                
                return itemColor;
            }
            return m_color;
        }
    }
}