using System.Collections.Generic;
using System.Linq;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.Config;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.GUI;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.TreeViewItem;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.TreeView
{
    public class ResourceModuleEntryTreeView : UnityEditor.IMGUI.Controls.TreeView
    {
        internal enum SortOption
        {
            PackageName,
            PackageType,
          
        }
        SortOption[] m_SortOptions =
        {
            SortOption.PackageName,
            SortOption.PackageType,
         
        };

        private ResourceModuleGroupEditor m_ModuleGroupEditor;
        public ResourceModuleEntryTreeView(TreeViewState state) : base(state)
        {
        }

        public ResourceModuleEntryTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
        }

        public ResourceModuleEntryTreeView(TreeViewState state, MultiColumnHeaderState mchs, ResourceModuleGroupEditor ed) : base(state,
            new MultiColumnHeader(mchs))
        {
            showBorder = true;
            m_ModuleGroupEditor = ed;
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
                new MultiColumnHeaderState.Column(),
            };

            int counter = 0;
            retVal[counter].headerContent = new GUIContent("Group Name", "Asset Group Name");
            retVal[counter].minWidth = 200;
            retVal[counter].width = 200;
            retVal[counter].maxWidth = 200;
            retVal[counter].headerTextAlignment = TextAlignment.Center;
            retVal[counter].canSort = true;
            retVal[counter].autoResize = true;

            counter++;
            retVal[counter].headerContent = new GUIContent("Package Type", "Asset Group Type");
            retVal[counter].minWidth = 200;
            retVal[counter].width = 200;
            retVal[counter].maxWidth = 200;
            retVal[counter].headerTextAlignment = TextAlignment.Center;
            retVal[counter].canSort = true;
            retVal[counter].autoResize = true;
            
            return retVal;
        }

        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
        }
        
        void OnSortingChanged(MultiColumnHeader mch)
        {
            
        }

        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            Debug.Log("---->SingleClickedItem: "+id);
            //选中
        }

        protected override void ContextClicked()
        {
            //Debug.Log("---->ContextClicked: ");
            m_ContextOnItem = false;
        }

        private bool m_ContextOnItem;
        protected override void ContextClickedItem(int id)
        {
            List<ResourceModuleEntryTreeViewItem> selectedNodes = new List<ResourceModuleEntryTreeViewItem>();
            foreach (var nodeId in GetSelection())
            {
                var item = FindItemInVisibleRows(nodeId); 
                if (item != null)
                    selectedNodes.Add(item);
            }
            if (selectedNodes.Count == 0)
                return;
            m_ContextOnItem = true;
            bool isMissingPath = false;
            var node = selectedNodes[0];
            if (node != null)
            {
                GenericMenu menu = new GenericMenu();
                isMissingPath = node.IsMissing;
                if (isMissingPath)
                {
                    menu.AddItem(new GUIContent("Clear"), false, ClearMiss, selectedNodes);
                }
                else
                {
                    menu.AddItem(new GUIContent("Rename"), false, RenameItem, selectedNodes);
                    menu.AddItem(new GUIContent("Remove"), false, RemoveItem, selectedNodes);
                }
                
                menu.ShowAsContext();
            }

        }
        
        protected override bool CanMultiSelect(UnityEditor.IMGUI.Controls.TreeViewItem item)
        {
            return false;
        }
    
        protected void ClearMiss(object context)
        {
            RemoveItem(context);
        }
        
        protected void RemoveItem(object context)
        {
            List<ResourceModuleEntryTreeViewItem> selectedNodes = context as List<ResourceModuleEntryTreeViewItem>;
            if (selectedNodes != null && selectedNodes.Count >= 1)
            {
                var item = selectedNodes.First();
                if (item != null)
                {
                    ResourceModuleDataManager.Instance.RemoveResourceModule(item.ResourceModuleInfo.packageName,ResourceModuleBrowserMain.instance.isAutoSave);
                    Reload();
                }
            }
        }
        
        protected void RenameItem(object context)
        {
            RenameItemImpl(context);
        }

        protected override bool CanRename(UnityEditor.IMGUI.Controls.TreeViewItem item)
        {
            return true;
        }

        internal void RenameItemImpl(object context)
        {
            List<ResourceModuleEntryTreeViewItem> selectedNodes = context as List<ResourceModuleEntryTreeViewItem>;
            if (selectedNodes != null && selectedNodes.Count >= 1)
            {
                var item = selectedNodes.First();
                if (CanRename(item))
                    BeginRename(item);
            }
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
            if (!args.acceptedRename)
                return;
            if (args.originalName == args.newName)
                return;
            if (!ResourceModuleDataManager.Instance.CanUseResourceModule(args.newName))
            {
                return;
            }
            var item = FindItemInVisibleRows(args.itemID);
            if (item != null)
            {
                ResourceModuleDataManager.Instance.RenameResourceModule(args.originalName,args.newName,ResourceModuleBrowserMain.instance.isAutoSave);
                Reload();
            }
        }
        
        ResourceModuleEntryTreeViewItem FindItemInVisibleRows(int id)
        {
            var rows = GetRows();
            foreach (var r in rows)
            {
                if (r.id == id)
                {
                    return r as ResourceModuleEntryTreeViewItem;
                }
            }
            return null;
        }

        protected override UnityEditor.IMGUI.Controls.TreeViewItem BuildRoot()
        {
            var root = new UnityEditor.IMGUI.Controls.TreeViewItem(-1, -1);
            root.children = new List<UnityEditor.IMGUI.Controls.TreeViewItem>();
            var configs = ResourceModuleDataManager.Instance.ResourceModuleManagerConfig;
            if (configs != null && configs.resourceModuleConfigs!=null)
            {
                foreach (var moduleInfo in configs.resourceModuleConfigs)
                {
                    ResourceModuleEntryTreeViewItem item = new ResourceModuleEntryTreeViewItem(moduleInfo,0);
                    root.AddChild(item);   
                }
            }
            return root;
        }
        
        protected override void RowGUI(RowGUIArgs args)
        {
            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
                CellGUI(args.GetCellRect(i), args.item , args.GetColumn(i), ref args);
        }

        protected void CellGUI(Rect cellRect, UnityEditor.IMGUI.Controls.TreeViewItem item, int column, ref RowGUIArgs args)
        {
            ResourceModuleEntryTreeViewItem viewItem = item as ResourceModuleEntryTreeViewItem;
            Color oldColor = UnityEngine.GUI.color;
            CenterRectUsingSingleLineHeight(ref cellRect);

            switch (m_SortOptions[column])
            {
                case SortOption.PackageName:
                {
                    DefaultGUI.Label(
                        cellRect,
                        viewItem.displayName,
                        args.selected,
                        args.focused);
                } 
                    break;
                case SortOption.PackageType:
                {
                    var old = ResourceModuleDataManager.Instance.GetPackageEnum(viewItem.ResourceModuleInfo.packageName);
                    UnityEngine.GUI.color = GetPackageTypeColor(old);
                    var result = (AssetPackageEnum)EditorGUI.EnumPopup(cellRect,old);
                    if (result != old)
                    {
                        ResourceModuleDataManager.Instance.ChangePackageType(viewItem.ResourceModuleInfo.packageName,result,ResourceModuleBrowserMain.instance.isAutoSave);
                    }
                    break;
                }
            }
            UnityEngine.GUI.color = oldColor;
        }

        private Color GetPackageTypeColor(AssetPackageEnum packageEnum)
        {
            switch (packageEnum)
            {
                case AssetPackageEnum.BuildIn:
                    return Color.red;
                case AssetPackageEnum.Loading:
                    return Color.green;
                default:
                    return Color.gray;
            }
        }
    }
}