namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.TreeViewItem
{
    public abstract class BaseTreeViewItem : UnityEditor.IMGUI.Controls.TreeViewItem
    {
        public BaseTreeViewItem(int id, int depth, string displayName) : base(id, depth, displayName)
        {
        }
    }
}