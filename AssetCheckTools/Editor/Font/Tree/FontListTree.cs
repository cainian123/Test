using System.Collections.Generic;
using AssetCheckTools.Editor.Font.ItemTreeView;
using AssetCheckTools.Editor.Font.View;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetCheckTools.Editor.Font.Tree
{
    public class FontListTree : TreeView
    {
        private List<FontInfo> m_AllFontInfo;
        private FontBaseTab m_baseTab;
        public FontListTree(TreeViewState state,FontBaseTab ctr,string[] paths) : base(state)
        {
            m_baseTab = ctr;
            LoadData(paths);
            showBorder = true;
            showAlternatingRowBackgrounds = true;
        }

        
        protected override TreeViewItem BuildRoot()
        {
            var root= new FontTreeViewItem();
            foreach (var info in m_AllFontInfo)
            {
                root.AddChild(new FontTreeViewItem(info));
            }
            return root;
        }

        private void LoadData(string[] paths)
        {
            if(m_AllFontInfo == null)
                m_AllFontInfo = new List<FontInfo>();
            m_AllFontInfo.Clear();
            for (int i = 0; i < paths.Length; i++)
            {
                m_AllFontInfo.Add(new FontInfo(paths[i],m_baseTab is LoadingUiTab));
            }
            m_AllFontInfo.Sort(((info, fontInfo) => { return fontInfo.ErrorLabelCount - info.ErrorLabelCount; }));
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var fontTreeViewItem = (args.item as FontTreeViewItem);
            if(fontTreeViewItem == null)
                return;
            if (args.item.icon == null)
                extraSpaceBeforeIconAndLabel = 16f;
            else
                extraSpaceBeforeIconAndLabel = 0f;

            Color old = GUI.color;
            if (fontTreeViewItem.FontInfo != null)
                GUI.color = fontTreeViewItem.FontInfo.GetColor();
            base.RowGUI(args);
            GUI.color = old;
        }
        
        protected override void DoubleClickedItem(int id)
        {
            if (FindItem(id, rootItem) is FontTreeViewItem assetItem)
            {
                Object o = AssetDatabase.LoadAssetAtPath<Object>(assetItem.FontInfo.fullAssetName);
                EditorGUIUtility.PingObject(o);
                Selection.activeObject = o;
            }
        }
        
        protected override void SelectionChanged( IList<int> selectedIds )
        {
            m_baseTab.CleanLabel();
            base.SelectionChanged(selectedIds);
            for( int i = 0; i < selectedIds.Count; ++i )
            {
                FontTreeViewItem item=FindItem(selectedIds[i], rootItem) as FontTreeViewItem;
                if (item != null)
                {
                    m_baseTab.SetErrorLabel(item.FontInfo.MFindErrorLabel);
                }
            }
            
           
        }
        
    }
}