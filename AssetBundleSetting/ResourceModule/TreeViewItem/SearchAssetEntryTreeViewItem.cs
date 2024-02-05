using System.Collections.Generic;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.TreeViewItem
{
    public class SearchAssetEntryTreeViewItem : BaseTreeViewItem
    {
        public string resourceModule;
        public SearchAssetEntryTreeViewItem(int id, int depth, string displayName) : base(id, depth, displayName)
        {
        }
        
        public SearchAssetEntryTreeViewItem(int id, int depth, string displayName, List<string> resourceModules) : base(id, depth, displayName)
        {
            if (resourceModules != null && resourceModules.Count > 0)
            {
                foreach (var name in resourceModules)
                {
                    if(string.IsNullOrEmpty(resourceModule))
                        resourceModule = name;
                    else
                        resourceModule = $"{resourceModule},{name}";
                }
            }
        }
    }
}