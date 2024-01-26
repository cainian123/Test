

using AssetCheckTools.Editor.Asset.Tree;

namespace AssetCheckTools.Editor.Asset.View
{
    public class TextureTab : BaseTab
    {
        protected override void CreateHeader()
        {
            if (m_AssetList == null)
            {
                m_AssetList = new TextureListTree(m_AssetListState,this);
                
            }
        }
    }
}