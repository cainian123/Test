using UnityEditor;
using UnityEngine;

namespace AssetCheckTools.Editor.Asset.Data
{
    public class AudioInfo : AssetBaseInfo
    {
        internal float Length;
        internal bool ForceToMono;
        internal AudioClipLoadType LoadType;
        internal string Format;
        
        public AudioInfo(string guid)
        {
            isFolder = false;
            isScene = false;
            fullAssetName = AssetDatabase.GUIDToAssetPath(guid);
            LoadAsset(fullAssetName);
        }
        
        private void LoadAsset(string path)
        {
            AudioClip audio = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (audio)
            {
                Length = audio.length;
                LoadType=audio.loadType;
                Format =path.Substring(path.LastIndexOf('.'));
                AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
                if (audioImporter != null) ForceToMono = audioImporter.forceToMono;
            }
        }
    }
}