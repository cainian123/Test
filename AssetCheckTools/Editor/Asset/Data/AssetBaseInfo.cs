using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AssetCheckTools.Editor.Asset.Data
{
    public abstract class AssetBaseInfo
    {
        internal bool isScene { get; set; }
        internal bool isFolder { get; set; }
        internal long fileSize;

        private HashSet<string> m_Parents;
        private string m_AssetName;
        protected string m_DisplayName;
        private string m_BundleName;

        internal string fullAssetName
        {
            get { return m_AssetName; }
            set
            {
                m_AssetName = value;
                m_DisplayName = System.IO.Path.GetFileNameWithoutExtension(m_AssetName);

                //TODO - maybe there's a way to ask the AssetDatabase for this size info.
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(m_AssetName);
                if (fileInfo.Exists)
                    fileSize = fileInfo.Length;
                else
                    fileSize = 0;
            }
        }
        
        internal string displayName
        {
            get { return m_DisplayName; }
        }
        
        internal string GetSizeString()
        {
            if (fileSize == 0)
                return "--";
            return EditorUtility.FormatBytes(fileSize);
        }
        internal virtual  Color GetColor()
        {
            return Color.white;
        }
        
    }
}