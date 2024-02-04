using System.Collections.Generic;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.TreeViewItem;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.TreeView
{
    public class SearchAssetTreeView : BaseTreeView
    {
        public SearchAssetTreeView(TreeViewState state) : base(state)
        {
        }

        public SearchAssetTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
        }
        
        public SearchAssetTreeView(TreeViewState state, MultiColumnHeaderState mchs) : base(state,
            new MultiColumnHeader(mchs))
        {
            showBorder = true;
            showAlternatingRowBackgrounds = true;
            columnIndexForTreeFoldouts = 0;
            multiColumnHeader.sortingChanged += OnSortingChanged;
        }
        
        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState()
        {
            return new MultiColumnHeaderState(GetColumns());
        }

        static MultiColumnHeaderState.Column[] GetColumns()
        {
            var retVal = new[]
            {
                new MultiColumnHeaderState.Column(),
                //new MultiColumnHeaderState.Column(),
            };

            int counter = 0;
            retVal[counter].headerContent = new GUIContent("Search Result", "Asset Name");
            retVal[counter].minWidth = 200;
            retVal[counter].width = 200;
            retVal[counter].maxWidth = 200;
            retVal[counter].headerTextAlignment = TextAlignment.Center;
            retVal[counter].canSort = true;
            retVal[counter].autoResize = true;

            return retVal;
        }

        protected override UnityEditor.IMGUI.Controls.TreeViewItem GetBuildRoot()
        {
            var root = new UnityEditor.IMGUI.Controls.TreeViewItem(-1, -1);
            root.children = new List<UnityEditor.IMGUI.Controls.TreeViewItem>();
            return root;
        }

        protected override IList<UnityEditor.IMGUI.Controls.TreeViewItem> BuildRows(UnityEditor.IMGUI.Controls.TreeViewItem root)
        {
            root.children.Clear();
            if (!string.IsNullOrEmpty(searchString))
            {
                var allAssetDatas = Search(searchString);
                if (allAssetDatas != null)
                {
                    foreach (var key in allAssetDatas.Keys)
                    {
                        var item = new SearchAssetEntryTreeViewItem(key.GetHashCode(), 0, key);
                        item.children = new List<UnityEditor.IMGUI.Controls.TreeViewItem>();
                        foreach (var path in allAssetDatas[key])
                        {
                            item.children.Add(new AssetInfoEntryTreeViewItem(path.GetHashCode(), 1, path));
                        }
                        root.AddChild(item);
                    }
                }
            }
            
            return base.BuildRows(root);
        }

        private Dictionary<string,List<string>> Search(string searchString)
        {
            var allAssets =ResourceModuleDataManager.Instance.GetAllAssetInfo();
            if (allAssets != null && allAssets.Count > 0)
            {
                Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
                foreach (var key in allAssets.Keys)
                {
                    var list = allAssets[key];
                    if (list != null && list.Count > 0)
                    {
                        foreach (var path in list)
                        {
                            if (path.ToLower().Contains(searchString))
                            {
                                if(tempData.TryGetValue(key, out var value))
                                    value.Add(path);
                                else
                                {
                                    tempData.Add(key,new List<string>(){path});
                                }
                            }
                                
                        }
                    }
                }

                return tempData;
            }

            return null;
        }
    }
}