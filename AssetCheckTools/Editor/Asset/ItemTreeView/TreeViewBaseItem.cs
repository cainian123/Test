using AssetCheckTools.Editor.Asset.Data;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetCheckTools.Editor.Asset.ItemTreeView
{
    public class TreeViewBaseItem:TreeViewItem
    {
        private AssetBaseInfo m_asset;
        internal AssetBaseInfo asset
        {
            get { return m_asset; }
        }
        
        internal TreeViewBaseItem() : base(-1, -1) { }
        
        internal TreeViewBaseItem(AssetBaseInfo a) : base(a?.fullAssetName.GetHashCode() ?? Random.Range(int.MinValue, int.MaxValue), 0, a != null ? a.displayName : "failed")
        {
            m_asset = a;
            if (a != null)
                icon = AssetDatabase.GetCachedIcon(a.fullAssetName) as Texture2D;
        }
        
        protected Color m_color = new Color(0, 0, 0, 0);
        internal Color itemColor
        {
            get
            {
                if (m_color.a == 0.0f && m_asset != null)
                {
                    m_color = m_asset.GetColor();
                }
                return m_color;
            }
            set { m_color = value; }
        }
        
       
        
        
    }
}