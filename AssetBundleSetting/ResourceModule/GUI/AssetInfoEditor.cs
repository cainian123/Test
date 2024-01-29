using AssetStream.Editor.AssetBundleSetting.ResourceModule.TreeView;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.GUI
{
    public class AssetInfoEditor
    {
        private ResourceModuleBrowserMain window;

        private AssetInfoEntryTreeView m_EntryTree;
        
        [SerializeField]
        TreeViewState m_TreeState;
        [SerializeField]
        MultiColumnHeaderState m_Mchs;
        
        bool m_ResizingVerticalSplitter;
        public AssetInfoEditor(ResourceModuleBrowserMain w)
        {
            window = w;
            OnEnable();
        }
        
        public void OnEnable()
        {
            
        }
        
        public bool OnGUI(Rect pos)
        {
            if (m_EntryTree == null)
                InitialiseEntryTree();
            m_EntryTree.OnGUI(pos);
            return m_ResizingVerticalSplitter;
        }


        internal AssetInfoEntryTreeView InitialiseEntryTree()
        {
            if (m_TreeState == null)
                m_TreeState = new TreeViewState();

            var headerState = AssetInfoEntryTreeView.CreateDefaultMultiColumnHeaderState();
            if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_Mchs, headerState))
                MultiColumnHeaderState.OverwriteSerializedFields(m_Mchs, headerState);
            m_Mchs = headerState;
            
            m_EntryTree = new AssetInfoEntryTreeView(m_TreeState, m_Mchs, this);
            m_EntryTree.Reload();
            return m_EntryTree;
        }
        
        public void OnDisable()
        {
            
        }
    }
}