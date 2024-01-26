using System.Collections.Generic;
using AssetCheckTools.Editor.Font.ItemTreeView;
using AssetCheckTools.Editor.Font.View;
using NUnit.Framework;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetCheckTools.Editor.Font.Tree
{
    public class LabelListTree : TreeView
    {
        private List<string> m_FindErrorLabel;
        private FontBaseTab m_baseTab;
        public LabelListTree(TreeViewState state,FontBaseTab ctr) : base(state)
        {
            m_baseTab = ctr;
            m_FindErrorLabel = new List<string>();
            showBorder = true;
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            return base.BuildRows(root);
        }

        protected override TreeViewItem BuildRoot()
        {
            var root= new FontTreeViewItem();
            foreach (var info in m_FindErrorLabel)
            {
                root.AddChild(new LabelTreeViewItem(info));
            }
            return root;
        }

        public void SetFindErrorLabel(List<string> paths,bool clear = false)
        {
            if(clear)
                m_FindErrorLabel.Clear();
            if(paths == null)
                return;
            foreach (var path in paths)
            {
                m_FindErrorLabel.Add(path);
            }

        }
        
        protected override void RowGUI(RowGUIArgs args)
        {
            var fontTreeViewItem = (args.item as LabelTreeViewItem);
            if(fontTreeViewItem == null)
                return;
            base.RowGUI(args);
         
        }
    }
}