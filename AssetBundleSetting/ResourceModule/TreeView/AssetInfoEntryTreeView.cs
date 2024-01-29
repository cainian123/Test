using System.Collections.Generic;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.GUI;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.TreeViewItem;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.TreeView
{
    public class AssetInfoEntryTreeView : UnityEditor.IMGUI.Controls.TreeView
    {
        enum ColumnId
        {
            Id,
            Path,
            Type,
        }
        
        private AssetInfoEditor m_ModuleGroupEditor;
        public AssetInfoEntryTreeView(TreeViewState state) : base(state)
        {
        }

        public AssetInfoEntryTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
        }

        public AssetInfoEntryTreeView(TreeViewState state, MultiColumnHeaderState mchs, AssetInfoEditor ed) : base(state,
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
                new MultiColumnHeaderState.Column(),
            };
            
            int counter = 0;

            retVal[counter].headerContent = new GUIContent("Path", "Current Path of asset");
            retVal[counter].minWidth = 100;
            retVal[counter].width = 260;
            retVal[counter].maxWidth = 10000;
            retVal[counter].headerTextAlignment = TextAlignment.Left;
            retVal[counter].canSort = true;
            retVal[counter].autoResize = true;
            counter++;
            
            retVal[counter].headerContent = new GUIContent(EditorGUIUtility.FindTexture("FilterByType"), "Asset type");
            retVal[counter].minWidth = 20;
            retVal[counter].width = 20;
            retVal[counter].maxWidth = 20;
            retVal[counter].headerTextAlignment = TextAlignment.Left;
            retVal[counter].canSort = false;
            retVal[counter].autoResize = true;
            counter++;

            return retVal;
        }
        
        void OnSortingChanged(MultiColumnHeader mch)
        {
           
        }

        protected override UnityEditor.IMGUI.Controls.TreeViewItem BuildRoot()
        {
            var root = new UnityEditor.IMGUI.Controls.TreeViewItem(-1, -1);
            root.children = new List<UnityEditor.IMGUI.Controls.TreeViewItem>();
            if (ResourceModuleDataManager.Instance.ResourceModuleManagerConfig)
            {
            }

            return null;
        }
    }
}