
using AssetCheckTools.Editor.Asset.Tree;

namespace AssetCheckTools.Editor.Asset.View
{
    public class AudioTab:BaseTab
    {
        protected override void CreateHeader()
        {
            m_AssetList = new AudioClipListTree(m_AssetListState,this);
        }
    }
}