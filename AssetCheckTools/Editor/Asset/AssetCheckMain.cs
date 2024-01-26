using System;
using System.Collections.Generic;
using AssetCheckTools.Editor.Asset.View;
using UnityEditor;
using UnityEngine;

namespace AssetCheckTools.Editor.Asset
{
    public class AssetCheckMain: EditorWindow, IHasCustomMenu, ISerializationCallbackReceiver
    {
        
        [MenuItem("Window/Asset Check/Asset", priority = 2100)]
        static void ShowWindow()
        {
            s_instance = null;
            instance.titleContent = new GUIContent("AssetCheck");
            instance.Show();
        }
        
        enum Mode
        {
            Texture = 0,
            Mesh =1,
            AnimationClip,
            AudioClip,
            Max,
        }
        
        private static AssetCheckMain s_instance = null;
        internal static AssetCheckMain instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = GetWindow<AssetCheckMain>();
                return s_instance;
            }
        }
        
        const float k_ToolbarPadding = 15;
        const float k_MenubarPadding = 35;

        private TextureTab m_textureTab;
        
        [SerializeField]
        Mode m_Mode;

        private Dictionary<Mode, BaseTab> m_allTab = null;
        private string[] labels;
        private void OnEnable()
        {
            RegisterTab();
            Rect rect = GetSubWindowArea();
            foreach (var tab in m_allTab.Values)
            {
                tab.OnEnable(rect,this);
            }
        }

        private void RegisterTab()
        {
            if (m_allTab == null)
            {
                m_allTab = new Dictionary<Mode, BaseTab>((int) Mode.Max);
                m_allTab.Add(Mode.Texture, new TextureTab());
                m_allTab.Add(Mode.Mesh,new MeshTab());
                m_allTab.Add(Mode.AnimationClip,new AnimationClipTab());
                m_allTab.Add(Mode.AudioClip,new AudioTab());
            }
            labels = new string[(int)Mode.Max];
            string[] names=Enum.GetNames(typeof(Mode));
            for (int i = 0; i < names.Length-1; i++)
            {
                labels[i] = names[i];
            }
        }

        private void OnDisable()
        {
            
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