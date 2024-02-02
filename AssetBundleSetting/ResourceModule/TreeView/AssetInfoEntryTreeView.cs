using System.Collections.Generic;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.GUI;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.TreeViewItem;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.TreeView
{
    public class AssetInfoEntryTreeView : BaseTreeView
    {
        internal enum SortOption
        {
         
            Path,
            Type,
          
        }
        SortOption[] m_SortOptions =
        {
       
            SortOption.Path,
            SortOption.Type,
         
        };
        
        private AssetInfoEditor m_ModuleGroupEditor;
        
        private string m_showPackageName;
        
        public AssetInfoEntryTreeView(TreeViewState state) : base(state)
        {
        }

        public AssetInfoEntryTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
        }

        public AssetInfoEntryTreeView(TreeViewState state, MultiColumnHeaderState mchs, AssetInfoEditor ed,string showPackageName) : base(state,
            new MultiColumnHeader(mchs))
        {
            showBorder = true;
            m_ModuleGroupEditor = ed;
            columnIndexForTreeFoldouts = 0;
            m_showPackageName = showPackageName;
            multiColumnHeader.sortingChanged += OnSortingChanged;
            showAlternatingRowBackgrounds = true;
            //extraSpaceBeforeIconAndLabel = 20;
            //customFoldoutYOffset = (20 - EditorGUIUtility.singleLineHeight) * 0.5f; 
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
                //new MultiColumnHeaderState.Column(),
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

            return retVal;
        }

        public void ShowAssetList(string packageName)
        {
            m_showPackageName = packageName;
            Reload();
        }
        
        protected override UnityEditor.IMGUI.Controls.TreeViewItem GetBuildRoot()
        {
            var root = new UnityEditor.IMGUI.Controls.TreeViewItem(-1, -1);
            root.children = new List<UnityEditor.IMGUI.Controls.TreeViewItem>();
            if (m_showPackageName != null)
            {
                var moduleData = ResourceModuleDataManager.Instance.GetModuleData(m_showPackageName);
                if (moduleData != null && moduleData.AssetConfigDatas != null)
                {
                    foreach (var assetBase in moduleData.AssetConfigDatas)
                    {
                        assetBase.AddToTree(ref root);
                    }
                }
            
            }
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }
        
        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
        }
        
        protected override void RowGUI(RowGUIArgs args)
        {
            if (m_LabelStyle == null)
            {
                m_LabelStyle = new GUIStyle("PR Label");
                if (m_LabelStyle == null)
                    m_LabelStyle = UnityEngine.GUI.skin.GetStyle("Label");
            }
            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
                CellGUI(args.GetCellRect(i), args.item , args.GetColumn(i), ref args);
        }

        GUIStyle m_LabelStyle;
        protected void CellGUI(Rect cellRect, UnityEditor.IMGUI.Controls.TreeViewItem item, int column, ref RowGUIArgs args)
        {
            AssetInfoEntryTreeViewItem viewItem = item as AssetInfoEntryTreeViewItem;
            Color oldColor = UnityEngine.GUI.color;
            //CenterRectUsingSingleLineHeight(ref cellRect);

            switch (m_SortOptions[column])
            {
                case SortOption.Path:
                    if (Event.current.type == EventType.Repaint)
                    {
                        float indent = GetContentIndent(item) + extraSpaceBeforeIconAndLabel;
                        cellRect.xMin += indent;
                        
                        var path = viewItem.entry.FullAssetName;
                        if (string.IsNullOrEmpty(path))
                            path =  "Missing File";
                        m_LabelStyle.Draw(cellRect, path, false, false, args.selected, args.focused);
                    }
                    break;   
                case SortOption.Type:
                {
                    
                    if (viewItem.assetIcon != null)
                        UnityEngine.GUI.DrawTexture(cellRect, viewItem.assetIcon, ScaleMode.ScaleToFit, true);
                }
                    break;
            }
            UnityEngine.GUI.color = oldColor;
        }


        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            DragAndDropVisualMode visualMode = DragAndDropVisualMode.None;

            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0 && m_showPackageName!=null)
            {
                bool canAdd = true;
                foreach (var path in DragAndDrop.paths)
                {
                    if (!path.Contains("_Resources"))
                    {
                        canAdd = false;
                        break;
                    }
                }

                //if (canAdd)
                {
                    visualMode = DragAndDropVisualMode.Copy;
                    if (args.performDrop)
                    {
                        if (ResourceModuleDataManager.Instance.AddAssetToResourceModule(m_showPackageName, DragAndDrop.paths,
                                ResourceModuleBrowserMain.instance.isAutoSave))
                            Reload();
                    }
                }
            }
            
            return visualMode;
        }
        
        protected override void ContextClickedItem(int id)
        {
            List<AssetInfoEntryTreeViewItem> selectedNodes = new List<AssetInfoEntryTreeViewItem>();
            foreach (var nodeId in GetSelection())
            {
                var item = FindItemInVisibleRows(nodeId); 
                if (item != null)
                    selectedNodes.Add(item);
            }
            if (selectedNodes.Count == 0)
                return;
            
            var node = selectedNodes[0];
            if (node != null)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Remove"), false, RemoveItem, selectedNodes);
                menu.ShowAsContext();
            }
        }

        private void RemoveItem(object context)
        {
            if (context is List<AssetInfoEntryTreeViewItem> selectedNodes && selectedNodes.Count > 0)
            {
                Dictionary<string, string> paths = new Dictionary<string, string>();
                foreach (var item in selectedNodes)
                {
                    if (item != null)
                    {
                        var parent=item.entry.GetRootParent();
                        if (parent != null)
                        {
                            paths.Add(item.entry.FullAssetName, parent.FullAssetName);  
                        }
                        else
                        {
                            paths.Add(item.entry.FullAssetName, null);  
                        }
                    }
                }
                
                if (ResourceModuleDataManager.Instance.RemoveAssetFromResourceModule(m_showPackageName, paths,
                            ResourceModuleBrowserMain.instance.isAutoSave))
                {
                    Reload();
                }
            }
        }
        
        AssetInfoEntryTreeViewItem FindItemInVisibleRows(int id)
        {
            var rows = GetRows();
            foreach (var r in rows)
            {
                if (r.id == id)
                {
                    return r as AssetInfoEntryTreeViewItem;
                }
            }
            return null;
        }
        
    }
}