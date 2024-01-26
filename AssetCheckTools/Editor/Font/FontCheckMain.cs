using System;
using System.Collections.Generic;
using AssetCheckTools.Editor.Font.View;
using UnityEditor;
using UnityEngine;

namespace AssetCheckTools.Editor.Font
{
    public class FontCheckMain : EditorWindow, IHasCustomMenu, ISerializationCallbackReceiver
    {
        
        enum Mode
        {
            LoadingUi = 0,
            GameUi =1,
            Max,
        }
        
        const float k_ToolbarPadding = 15;
        const float k_MenubarPadding = 35;
        
        private static FontCheckMain s_instance = null;
        internal static FontCheckMain instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = GetWindow<FontCheckMain>();
                return s_instance;
            }
        }
        
        [MenuItem("Window/Asset Check/Loading & Game Asset Check", priority = 2110)]
        static void ShowWindow()
        {
            s_instance = null;
            instance.titleContent = new GUIContent("AssetCheck");
            instance.Show();
        }
        
        Mode m_Mode;
        private Dictionary<Mode, FontBaseTab> m_allTab = null;
        private string[] labels;

        private void RegisterTab()
        {
            if (m_allTab == null)
            {
                m_allTab = new Dictionary<Mode, FontBaseTab>((int) Mode.Max);
                m_allTab.Add(Mode.LoadingUi, new LoadingUiTab());
                m_allTab.Add(Mode.GameUi,new GameUiTab());
            }
            labels = new string[(int)Mode.Max];
            string[] names=Enum.GetNames(typeof(Mode));
            for (int i = 0; i < names.Length-1; i++)
            {
                labels[i] = names[i];
            }
        }
        
        private void OnEnable()
        {
            RegisterTab();
            Rect rect = GetSubWindowArea();
            foreach (var tab in m_allTab.Values)
            {
                tab.OnEnable(rect,this);
            }
        }
        
        private void OnGUI()
        {
            ModeToggle();
            if (m_allTab != null && m_allTab.ContainsKey(m_Mode))
            {
                m_allTab[m_Mode].OnGUI(GetSubWindowArea());
            }
        }
        
        void ModeToggle()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(k_ToolbarPadding);
            float toolbarWidth = position.width - k_ToolbarPadding * 2;
         
            m_Mode = (Mode)GUILayout.Toolbar((int)m_Mode, labels, "LargeButton", GUILayout.Width(toolbarWidth) );
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        
        private Rect GetSubWindowArea()
        {
            float padding = k_MenubarPadding;
          
            Rect subPos = new Rect(k_ToolbarPadding, padding, position.width-k_ToolbarPadding*2, position.height-k_MenubarPadding);
            return subPos;
        }
        
        public void AddItemsToMenu(GenericMenu menu)
        {
           
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
           
        }
    }
}