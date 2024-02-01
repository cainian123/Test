using System.Collections.Generic;
using System.IO;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.Config;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.TreeViewItem;
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

        private int m_level;
        
        private bool m_isEfficient = false;

        public AssetBaseInfo(string assetPath, string packageName, AssetPackageEnum packageEnum,AssetBaseInfo parent,int level =0)
        {
            m_level = level;
            m_AssetName = assetPath;
            m_packageName = packageName;
            m_PackageEnum = packageEnum;
            m_Parent = parent;
            m_isFolder = CheckIsFolder;
            //m_isScene = CheckIsScene;
            //m_isEfficient = isEfficient;
        }

        /*public void LoadChild(List<AssetInfoConfig> childConfigs)
        {
            if (childConfigs != null && childConfigs.Count > 0)
            {
                foreach (var childConfig in childConfigs)
                {
                    AddChild(childConfig);
                }
            }
        }*/

        public void LoadChild(List<AssetInfoConfig> childConfigs,List<string> invalidChildConfigs)
        {
            if (m_isFolder)
            {
                AddChild(childConfigs,invalidChildConfigs);    
            }
        }

        private void AddChild(List<AssetInfoConfig> childConfigs, List<string> invalidChildConfigs)
        {
            if (m_child == null)
            {
                m_child = new List<AssetBaseInfo>();
            }

            LoadFolder(childConfigs,invalidChildConfigs);
            LoadAsset(childConfigs, invalidChildConfigs);
        }
        
        private void LoadFolder(List<AssetInfoConfig> childConfigs, List<string> invalidChildConfigs)
        {
            string[] subDirectories = Directory.GetDirectories(m_AssetName);
            if (subDirectories.Length > 0)
            {
                for (int i = 0; i < subDirectories.Length; i++)
                {
                    string path = subDirectories[i];
                    path = Util.Util.Path.GetRegularPath(path);
                    if (CanAdd(path,childConfigs,invalidChildConfigs))
                    {
                        AddToChildAsset(path,childConfigs,invalidChildConfigs);
                    }
                }
            }
        }
        
        
        private void LoadAsset(List<AssetInfoConfig> childConfigs, List<string> invalidChildConfigs)
        {
            string[] files = Directory.GetFiles(m_AssetName);
            if (files.Length > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    string path = files[i];
                    path = Util.Util.Path.GetRegularPath(path);
                    if (CanAdd(path,childConfigs,invalidChildConfigs))
                    {
                        AddToChildAsset(path,childConfigs,invalidChildConfigs);
                    }
                }
            }
        }

        private void AddToChildAsset(string path,List<AssetInfoConfig> childConfigs, List<string> invalidChildConfigs)
        {
            if (m_child == null)
            {
                m_child = new List<AssetBaseInfo>();
            }

            AssetBaseInfo info = new AssetBaseInfo(path,m_packageName,m_PackageEnum,this,m_level+1);
            info.LoadChild(childConfigs,invalidChildConfigs);
            m_child.Add(info);
        }
        
        private bool CanAdd(string path,List<AssetInfoConfig> childConfigs, List<string> invalidChildConfigs)
        {
            if(path.EndsWith(".meta"))
                return false;
            if (childConfigs != null)
            {
                foreach (var config in childConfigs)
                {
                    if (config.FullPath.Equals(path))
                        return false;
                }
            }

            if (invalidChildConfigs != null)
            {
                if(invalidChildConfigs.Contains(path))
                    return false;
            }

            return true;
        }
        

        /*private void AddChild(AssetInfoConfig assetInfoConfig)
        {
            if (m_child == null)
            {
                m_child = new List<AssetBaseInfo>();
            }

            AssetBaseInfo info = new AssetBaseInfo(assetInfoConfig.FullPath,m_packageName,m_PackageEnum,this,assetInfoConfig.IsEfficient,m_level+1);
            info.LoadChild(assetInfoConfig.ChildConfigs);
            m_child.Add(info);
        }*/

        public void AddToTree(ref UnityEditor.IMGUI.Controls.TreeViewItem root)
        {
            UnityEditor.IMGUI.Controls.TreeViewItem viewItem = new AssetInfoEntryTreeViewItem(this, m_level);
          
            
            if (m_child != null && m_child.Count > 0)
            {
                foreach (var assetBaseInfo in m_child)
                {
                    assetBaseInfo.AddToTree(ref viewItem);
                }
            }
            root.AddChild(viewItem);
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