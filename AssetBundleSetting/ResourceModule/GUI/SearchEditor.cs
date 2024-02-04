using System;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.TreeView;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.GUI
{
    [Serializable]
    public class SearchEditor
    {
        [SerializeField]
        TreeViewState m_TreeState;
        [SerializeField]
        MultiColumnHeaderState m_Mchs;
        [SerializeField]
        public ResourceModuleBrowserMain window;

        SearchField m_SearchField;
        private SearchAssetTreeView m_EntryTree;
        public SearchEditor(ResourceModuleBrowserMain w)
        {
            window = w;
            OnEnable();
        }

        private void OnEnable()
        {
            m_SearchField = new SearchField();
         
        }

        float searchHeight = 20f;
        public bool OnGUI(Rect pos)
        {
            if (m_EntryTree == null)
                InitialiseEntryTree();
            
            m_EntryTree.OnGUI(new Rect(pos.x, pos.y+searchHeight, pos.width-6f, pos.height-searchHeight));
            OnGUISearchBar(new Rect(pos.x, pos.y, pos.width-6f, searchHeight)); 
            
            return false;
        }
        
        
        internal SearchAssetTreeView InitialiseEntryTree()
        {
            if (m_TreeState == null)
                m_TreeState = new TreeViewState();

            var headerState = SearchAssetTreeView.CreateDefaultMultiColumnHeaderState();
            if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_Mchs, headerState))
                MultiColumnHeaderState.OverwriteSerializedFields(m_Mchs, headerState);
            m_Mchs = headerState;
            
            m_EntryTree = new SearchAssetTreeView(m_TreeState, m_Mchs);
            m_EntryTree.Reload();
            return m_EntryTree;
        }
        
        void OnGUISearchBar(Rect rect)
        {
            m_EntryTree.searchString = m_SearchField.OnToolbarGUI(rect, m_EntryTree.searchString);
            m_EntryTree.searchString = m_EntryTree.searchString;
        }
        
        public void OnDisable()
        {
            
        }
    }
}