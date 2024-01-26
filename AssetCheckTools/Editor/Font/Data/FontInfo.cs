using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.IMGUI.Controls
{
    public class FontInfo
    {
        private List<string> m_FindErrorLabel;
        private string m_name;
        public string fullAssetName;
        private bool m_isCheckLoadingUi = false;

        public int ErrorLabelCount => m_FindErrorLabel.Count;

        public List<string> MFindErrorLabel => m_FindErrorLabel;

        public string displayName
        {
            get { return m_name; }
        }
        public FontInfo(string guid,bool checkLoading = false)
        {
            m_FindErrorLabel = new List<string>();
            fullAssetName = AssetDatabase.GUIDToAssetPath(guid);
            m_name = System.IO.Path.GetFileNameWithoutExtension(fullAssetName);
            m_isCheckLoadingUi = checkLoading;
            LoadAsset(fullAssetName);
        }
        
        private void LoadAsset(string path)
        {
            Object obj=AssetDatabase.LoadMainAssetAtPath(path);
            if (obj && obj is UnityEngine.GameObject)
            {
                GameObject gameObject = obj as GameObject;
                UILabel[] allLabels=gameObject.GetComponentsInChildren<UILabel>(true);
                if (allLabels != null && allLabels.Length > 0)
                {
                    foreach (var label in allLabels)
                    {
                        CheckLabel(label);
                    }
                }

                if (m_isCheckLoadingUi)
                {
                    UISprite[] allSprite=gameObject.GetComponentsInChildren<UISprite>(true);
                    if (allSprite != null && allSprite.Length > 0)
                    {
                        foreach (var sprite in allSprite)
                        {
                            CheckAtlas(sprite);
                        }
                    }
                }
                
                UITexture[] allTexture=gameObject.GetComponentsInChildren<UITexture>(true);
                if (allTexture != null && allTexture.Length > 0)
                {
                    foreach (var texture in allTexture)
                    {
                        CheckUiTexture(texture);
                    }
                }
            }
        }

        private void CheckLabel(UILabel label)
        {
            if(label == null)
                return;
            if(label.bitmapFont == null && label.ambigiousFont == null)
                return;
            string fontName = label.bitmapFont == null ? label.ambigiousFont.name : label.bitmapFont.name;
            if (m_isCheckLoadingUi)
            {
                if (!fontName.Contains("_loading"))
                {
                    m_FindErrorLabel.Add(GetGameObjectPath(label.gameObject));
                }
            }
            else
            {
                if (fontName.Contains("_loading"))
                {
                    m_FindErrorLabel.Add(GetGameObjectPath(label.gameObject));
                }
            }
        }

        private void CheckAtlas(UISprite sprite)
        {
            if(sprite == null)
                return;
            if (sprite.atlas != null && !sprite.atlas.name.Contains("_loading"))
            {
                m_FindErrorLabel.Add(GetGameObjectPath(sprite.gameObject));
            }
        }

        private void CheckUiTexture(UITexture texture)
        {
            if(texture == null || texture.mainTexture==null)
                return;
            var path=AssetDatabase.GetAssetPath(texture.mainTexture);
            if (m_isCheckLoadingUi)
            {
                if (!path.Contains("__UIData/UISlice/Loading") && !path.Contains("__RawArtData/Texture/VNRes"))
                {
                    m_FindErrorLabel.Add(GetGameObjectPath(texture.gameObject));
                }
            }
            else
            {
                if (path.Contains("__UIData/UISlice/Loading") || path.Contains("__RawArtData/Texture/VNRes"))
                {
                    m_FindErrorLabel.Add(GetGameObjectPath(texture.gameObject));
                }
            }
        }
        
        private  string GetGameObjectPath(GameObject obj)
        {
            string path = obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = obj.name + "/" + path;
            }
            return path;
        }

        public Color GetColor()
        {
            if(m_FindErrorLabel.Count > 0)
                return Color.red;
            return Color.white;
        }
        
    }
}