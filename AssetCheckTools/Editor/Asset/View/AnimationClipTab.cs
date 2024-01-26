using AssetCheckTools.Editor.Asset.Tree;

namespace AssetCheckTools.Editor.Asset.View
{
    public class AnimationClipTab : BaseTab
    {
        protected override void CreateHeader()
        {
            if (m_AssetList == null)
            {
                m_AssetList = new AnimationClipListTree(m_AssetListState,this);
            }
        }
    }
}