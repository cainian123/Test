

using AssetCheckTools.Editor.Asset.Tree;

namespace AssetCheckTools.Editor.Asset.View
{
    public class MeshTab : BaseTab
    {
        protected override void CreateHeader()
        {
            if (m_AssetList == null)
            {
                m_AssetList = new MeshListTree(m_AssetListState,this);
                
            }
        }
    }
}