using System.Collections.Generic;
using System.IO;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.Config;
using UnityEditor;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.Data
{

    public class AssetBaseInfo
    {

        private string m_AssetName;
     
        private string m_packageName;
  
        private AssetPackageEnum m_PackageEnum;
      
        private AssetBaseInfo m_Parent;

        private List<AssetBaseInfo> m_child;

        private bool m_isScene;
        private bool m_isFolder;

        public AssetBaseInfo(string assetPath, string packageName, AssetPackageEnum packageEnum,AssetBaseInfo parent)
        {
            m_AssetName = assetPath;
            m_packageName = packageName;
            m_PackageEnum = packageEnum;
            m_Parent = parent;
            m_isFolder = CheckIsFolder;
            m_isScene = CheckIsScene;
        }

        public void LoadChild(List<string> invalidChildConfigs)
        {
            if (m_isFolder)
            {
                LoadFolder(invalidChildConfigs);
                LoadAsset(invalidChildConfigs);
            }
        }

        private void LoadFolder(List<string> invalidChildConfigs)
        {
            string[] subDirectories = Directory.GetDirectories(FullAssetName);
            if (subDirectories.Length > 0)
            {
                for (int i = 0; i < subDirectories.Length; i++)
                {
                    string path = subDirectories[i];
                    if (CanAdd(path, invalidChildConfigs))
                    {
                        AddChild(path,invalidChildConfigs);
                    }
                }
            }
        }

        private void LoadAsset(List<string> invalidChildConfigs)
        {
            string[] files = Directory.GetFiles(FullAssetName);
            if (files.Length > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    string path = files[i];
                    if (CanAdd(path, invalidChildConfigs))
                    {
                        AddChild(path,invalidChildConfigs);
                    }
                }
            }
        }

        private bool CanAdd(string path, List<string> invalidChildConfigs)
        {
            if (path.EndsWith(".meta"))
                return false;
            if (invalidChildConfigs == null)
                return true;
            if (invalidChildConfigs.Contains(path))
                return false;
            return true;
        }

        private void AddChild(string path,List<string> invalidChildConfigs)
        {
            if (m_child == null)
            {
                m_child = new List<AssetBaseInfo>();
            }

            AssetBaseInfo info = new AssetBaseInfo(path,m_packageName,m_PackageEnum,this);
            info.LoadChild(invalidChildConfigs);
            m_child.Add(info);
        }

        public string FullAssetName
        {
            get { return m_AssetName; }
        }

        public string PackageName
        {
            get
            {
                return m_packageName;
            }
        }

        public AssetPackageEnum PackageEnum
        {
            get
            {
                return m_PackageEnum;
            }
        }
        
        private bool CheckIsScene
        {
            get
            {
                if (!string.IsNullOrEmpty(m_AssetName))
                {
                    return AssetDatabase.GetMainAssetTypeAtPath(m_AssetName) == typeof(SceneAsset);
                }

                return false;
            }
        }

        private bool CheckIsFolder
        {
            get
            {
                if(!string.IsNullOrEmpty(m_AssetName))
                    return AssetDatabase.IsValidFolder(m_AssetName);
                return false;
            }
        }

        public void RenamePackage(string newName)
        {
            m_packageName = newName;
            if (m_child != null && m_child.Count > 0)
            {
                foreach (var assetBaseInfo in m_child)
                {
                    assetBaseInfo.RenamePackage(newName);
                }
            }
        }
    }
}