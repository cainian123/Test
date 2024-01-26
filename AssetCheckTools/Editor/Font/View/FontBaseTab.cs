using System.Collections.Generic;
using AssetCheckTools.Editor.Font.Tree;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetCheckTools.Editor.Font.View
{
    public abstract class FontBaseTab
    {
        [SerializeField]
        float m_VerticalSplitterPercentRight;
        [SerializeField]
        float m_VerticalSplitterPercentLeft;
        [SerializeField]
        float m_HorizontalSplitterPercent;
        
        EditorWindow m_Parent = null;
        Rect m_Position;
        Rect m_VerticalSplitterRectRight, m_VerticalSplitterRectLeft;
        const float k_SplitterWidth = 3f;
        const float k_SplitterHight = 3f;

        protected FontListTree m_LeftTree;
        protected LabelListTree m_RightTree;
        
        protected TreeViewState m_LefttState;
        protected TreeViewState m_RightState;
        internal FontBaseTab()
        {
            m_HorizontalSplitterPercent = 0.4f;
        }

        internal void OnEnable(Rect pos, EditorWindow parent)
        {
            m_Parent = parent;
            m_Position = pos;
            
            m_VerticalSplitterRectLeft = new Rect(
                m_Position.x,
                m_Position.y,
                (int)(m_Position.width * m_HorizontalSplitterPercent),
                m_Position.height-k_SplitterHight);
            
            m_VerticalSplitterRectRight = new Rect(
                m_Position.x + m_VerticalSplitterRectLeft.width,
                m_Position.y,
                (m_Position.width - m_VerticalSplitterRectLeft.width) - k_SplitterWidth,
                m_Position.height-k_SplitterHight);
        }

        protected abstract string[] ForceReloadData();
        
        internal void OnGUI(Rect rect)
        {
            m_Position = rect;
            m_VerticalSplitterRectLeft = new Rect(
                m_Position.x,
                m_Position.y,
                (int)(m_Position.width * m_HorizontalSplitterPercent),
                m_Position.height-k_SplitterHight);
            
            m_VerticalSplitterRectRight = new Rect(
                m_Position.x + m_VerticalSplitterRectLeft.width,
                m_Position.y,
                (m_Position.width - m_VerticalSplitterRectLeft.width) - k_SplitterWidth,
                m_Position.height-k_SplitterHight);
            if (m_LeftTree == null)
            {
                if (m_RightState == null)
                    m_RightState = new TreeViewState();
                m_RightTree = new LabelListTree(m_RightState, this);
                m_RightTree.Reload();
             
                if (m_LefttState == null)
                    m_LefttState = new TreeViewState();
                m_LeftTree = new FontListTree(m_LefttState,this,ForceReloadData());
                m_LeftTree.Reload();
                m_Parent.Repaint();
            }
            
            m_LeftTree.OnGUI(m_VerticalSplitterRectLeft);
            m_RightTree.OnGUI(m_VerticalSplitterRectRight);
                
        }

        public void CleanLabel()
        {
            m_RightTree.SetFindErrorLabel(null,true);
        }

        public void SetErrorLabel(List<string> paths)
        {
            m_RightTree.SetFindErrorLabel(paths);
            m_RightTree.Reload();
        }
        
        protected string[] FindAsset(string filter,string[] paths)
        {
            return AssetDatabase.FindAssets(filter,paths);
        }
    }
}