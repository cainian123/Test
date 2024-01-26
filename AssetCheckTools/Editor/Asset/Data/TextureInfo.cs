using System.Collections.Generic;
using UIOptimize;
using UnityEditor;
using UnityEngine;

namespace AssetCheckTools.Editor.Asset.Data
{
    public class TextureInfo : AssetBaseInfo
    {
        internal long Width;
        internal long Height;
        internal string AndroidFormat;
        internal string IosFormat;
        internal bool IsOpenMipMap;
        internal bool IsReadWrite;
        private List<string> textureAutoSet = new List<string>();
        public TextureInfo(string guid)
        {
            textureAutoSet.Add("Assets/__RawArtData/_Resources/Texture");
            textureAutoSet.Add("Assets/__ArtData/Spine");
            isFolder = false;
            isScene = false;
            fullAssetName = guid;
            LoadAsset(fullAssetName);
        }

        private void LoadAsset(string path)
        {
            Texture texture = (Texture)AssetDatabase.LoadAssetAtPath(path,typeof(Texture));
            if (texture)
            {
                Width = texture.width;
                Height = texture.height;
                IsReadWrite=texture.isReadable;
            }
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            
            if (textureImporter != null)
            {
                IsOpenMipMap = textureImporter.mipmapEnabled;

                var androidTextureSettings=textureImporter.GetPlatformTextureSettings_Android();
                AndroidFormat = androidTextureSettings.format.ToString();

                var iosTextureSettings = textureImporter.GetPlatformTextureSettings_iOS();
                IosFormat = iosTextureSettings.format.ToString();
            }
        }

        internal string GetSize()
        {
            return $"{Width} x {Height}";
        }
        
        internal  Color GetAndroidFormatColor()
        {
            if(AndroidFormat==null || AndroidFormat.Contains("ETC") || IsAutoSetting(fullAssetName))
                return Color.white;
            return Color.red;
        }
        
        internal  Color GetIOSFormatColor()
        {
            if(IsAutoSetting(fullAssetName) || IosFormat == null || IosFormat.Contains("ASTC"))
                return Color.white;
            return Color.red;
        }

        internal Color GetSizeColor()
        {
            if(Width%4 > 0 || Height%4 >0)
                return Color.red;
            return Color.white;
        }

        private bool IsAutoSetting(string path)
        {
            foreach (var var in textureAutoSet)
            {
                if (path.Contains(var))
                    return true;
            }

            return false;
        }
    }
}