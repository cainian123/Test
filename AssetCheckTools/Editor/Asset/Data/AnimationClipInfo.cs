using System;
using UnityEditor;
using UnityEngine;

namespace AssetCheckTools.Editor.Asset.Data
{
    public class AnimationClipInfo : AssetBaseInfo
    {
        internal float Length;

        internal float FrameRate;
        public AnimationClipInfo(string guid)
        {
            isFolder = false;
            isScene = false;
            fullAssetName = AssetDatabase.GUIDToAssetPath(guid);
            LoadAsset(fullAssetName);
        }

        private void LoadAsset(string path)
        {
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip)
            {
                Length = clip.length;
                FrameRate = clip.frameRate;
                if (clip.name != m_DisplayName)
                {
                    m_DisplayName = $"{m_DisplayName}/{clip.name}";
                }
            }
        }
        
        internal  Color GetFrameRateColor()
        {
            if(FrameRate<=30)
                return Color.white;
            return Color.red;
        }
    }
}