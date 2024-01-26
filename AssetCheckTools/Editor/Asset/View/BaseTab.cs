using AssetCheckTools.Editor.Asset.Tree;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetCheckTools.Editor.Asset.View
{
    public abstract class BaseTab
    {
        [SerializeField]
        float m_HorizontalSplitterPercent;
        [SerializeField]
        float m_VerticalSplitterPercent;
        [SerializeField]
        protected TreeViewState m_AssetListState;
        [SerializeField]
        protected  MultiColumnHeaderState m_AssetListMCHState;
        
        SearchField m_searchField;

        EditorWindow m_Parent = null;
        
        Rect m_Position;
        
        Rect m_HorizontalSplitterRect, m_VerticalSplitterRect;
        
        const float k_SplitterWidth = 3f;
        
        [SerializeField]protected AssetListBaseTree m_AssetList;

        protected BaseTab()
        {
            m_HorizontalSplitterPercent = 0.85f;
            m_VerticalSplitterPercent = 0.85f;
        }
        
        internal void OnEnable(Rect pos, EditorWindow parent)
        {
            m_Parent = parent;
            m_Position = pos;
            m_VerticalSplitterRect = new Rect(
                m_Position.x,
                m_Position.y,
                m_Position.width,
                m_Position.height-k_SplitterWidth);
            m_searchField = new SearchField();
        }
        
        internal void OnGUI(Rect rect)
        {
            m_Position = rect;
            m_VerticalSplitterRect = new Rect(
                rect.x,
                rect.y,
                rect.width,
                rect.height-k_SplitterWidth);
            if (m_AssetList == null)
            {
                if(m_AssetListState == null)
                    m_AssetListState = new TreeViewState();
                CreateHeader();
                var headerState = m_AssetList.CreateDefaultMultiColumnHeaderState();
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_AssetListMCHState, headerState))
                    MultiColumnHeaderState.OverwriteSerializedFields(m_AssetListMCHState, headerState);
                m_AssetListMCHState = headerState;
                m_AssetList.InitMultiColumnHeaderState(new MultiColumnHeader(m_AssetListMCHState));
                m_AssetList.Reload();
                m_Parent.Repaint();
            }

            float searchHeight = 20f;
            OnGUISearchBar(new Rect(m_VerticalSplitterRect.x, m_VerticalSplitterRect.y, m_VerticalSplitterRect.width, searchHeight));
            m_AssetList.OnGUI(new Rect(
                m_VerticalSplitterRect.x,
                m_VerticalSplitterRect.y+searchHeight,
                m_VerticalSplitterRect.width,
                m_VerticalSplitterRect.height-searchHeight -10f));
        }

        protected abstract void CreateHeader();
        
        
        void OnGUISearchBar(Rect rect)
        {
            m_AssetList.searchString = m_searchField.OnGUI(rect, m_AssetList.searchString);
            m_AssetList.searchString = m_AssetList.searchString;
        }
    }
}