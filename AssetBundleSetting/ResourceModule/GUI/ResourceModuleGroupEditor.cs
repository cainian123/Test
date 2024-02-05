using System;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.TreeView;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.GUI
{
    [Serializable]
    public class ResourceModuleGroupEditor
    {
        private ResourceModuleBrowserMain _window;

        public ResourceModuleBrowserMain window
        {
            get
            {
                if (_window == null)
                    return ResourceModuleBrowserMain.instance;
                return _window;
            }
        }
        
        private ResourceModuleEntryTreeView m_EntryTree;
        
        bool m_ResizingVerticalSplitter;
        
        [SerializeField]
        TreeViewState m_TreeState;
        [SerializeField]
        MultiColumnHeaderState m_Mchs;
        public ResourceModuleGroupEditor(ResourceModuleBrowserMain w)
        {
            _window = w;
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

        public void Reload()
        {
            if (m_EntryTree != null)
                m_EntryTree.Reload();
        }

        internal ResourceModuleEntryTreeView InitialiseEntryTree()
        {
            if (m_TreeState == null)
                m_TreeState = new TreeViewState();

            var headerState = ResourceModuleEntryTreeView.CreateDefaultMultiColumnHeaderState();
            if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_Mchs, headerState))
                MultiColumnHeaderState.OverwriteSerializedFields(m_Mchs, headerState);
            m_Mchs = headerState;
            
            m_EntryTree = new ResourceModuleEntryTreeView(m_TreeState, m_Mchs, this);
            m_EntryTree.Reload();
            return m_EntryTree;
        }
        
        

        public void OnDisable()
        {
            
        }
    }
}