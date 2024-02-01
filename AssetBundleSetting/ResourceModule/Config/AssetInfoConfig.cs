using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.Config
{
    [Serializable]
    public class AssetInfoConfig
    {
        [SerializeField]
        private string fullPath;
        //[SerializeField]
        //private bool isEfficient = false;
        [SerializeField]
        private List<string> invalidChildConfigs;
        //[SerializeField]
        //private List<AssetInfoConfig> childConfigs;

        public string FullPath
        {
            get
            {
                return fullPath;
            }
        }

        public List<string> InvalidChildConfigs
        {
            get
            {
                return invalidChildConfigs;
            }
        }

        /*public List<AssetInfoConfig> ChildConfigs
        {
            get
            {
                return childConfigs;
            }
        }*/

        public AssetInfoConfig(string currentPath)
        {
            fullPath = currentPath;
        }

        /*public bool AddNewAsset(string assetPath,string[] paths,bool checkEfficient = true)
        {
            if (assetPath.Equals(fullPath))
            {
                if(checkEfficient)
                    isEfficient = true;
                if (CheckIsFolder)
                {
                    return AddChildAssets();
                }

                childConfigs = new List<AssetInfoConfig>(0);
                return true;
            }
           
            return AddToChildAsset(assetPath,paths);
        }

        public bool IsEfficient
        {
            get
            {
                return isEfficient;
            }
        }

        public void GetEfficientPath(ref List<AssetInfoConfig> tempConfig)
        {
            if (tempConfig == null)
                tempConfig = new List<AssetInfoConfig>();
            if (isEfficient)
            {
                tempConfig.Add(this);
                return;
            }
            if (childConfigs != null && childConfigs.Count > 0)
            {
                foreach (var assetInfoConfig in childConfigs)
                {
                     assetInfoConfig.GetEfficientPath(ref tempConfig);
                }
            }
        }
        
        /// <summary>
        /// 添加文件加下的资源
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        private bool AddChildAssets()
        {
            bool isAdd = false;
            isAdd |= LoadFolder();
            isAdd |= LoadAsset();
            return isAdd;
        }

        /// <summary>
        /// 向子文件夹添加资源
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        private bool AddToChildAsset(string assetPath,string[] paths,bool checkEfficient = true)
        {
            if (paths != null && paths.Length > 0)
            {
                if (childConfigs == null)
                {
                    childConfigs = new List<AssetInfoConfig>();
                }

                bool isAdd = false;
               
                string startPath = paths[0];
                string[] newPaths = new string[paths.Length - 1];
                for (int i = 1; i < paths.Length; i++)
                {
                    newPaths[i - 1] = paths[i];
                }
                AssetInfoConfig config = IsHaveAssetPath(startPath);
                if (config == null)
                {
                    config = new AssetInfoConfig(startPath);
                    isAdd |=config.AddNewAsset(assetPath,newPaths,checkEfficient);   
                    childConfigs.Add(config);
                }
                else
                {
                    if (!checkEfficient && config.isEfficient)
                    {
                        
                    }
                    else
                    {
                        if( newPaths.Length > 0)
                            isAdd |= config.AddNewAsset(assetPath, newPaths,checkEfficient);
                    }
                }
             

                return isAdd;
            }

            return false;
            
        }
        
        private bool LoadFolder()
        {
            bool isAdd = false;
            string[] subDirectories = Directory.GetDirectories(fullPath);
            if (subDirectories.Length > 0)
            {
                for (int i = 0; i < subDirectories.Length; i++)
                {
                    string path = subDirectories[i];
                    path = Util.Util.Path.GetRegularPath(path);
                    if (CanAdd(path))
                    {
                        isAdd |= AddToChildAsset(path,ConvertPathToTree(path),false);
                    }
                }
            }

            return isAdd;
        }

        private bool LoadAsset()
        {
            bool isAdd = false;
            string[] files = Directory.GetFiles(fullPath);
            if (files.Length > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    string path = files[i];
                    path = Util.Util.Path.GetRegularPath(path);
                    if (CanAdd(path))
                    {
                        isAdd |=AddToChildAsset(path,ConvertPathToTree(path),false);
                    }
                }
            }

            return isAdd;
        }
        
        private bool CanAdd(string path)
        {
            if (path.EndsWith(".meta"))
                return false;
           
            return true;
        }
        
        private  string[] ConvertPathToTree(string path)
        {
            string regularPath=Util.Util.Path.GetRegularPath(path);
            int index =regularPath.IndexOf(fullPath, StringComparison.Ordinal);
            if (index < 0)
                return null;
            regularPath=regularPath.Substring(index+fullPath.Length+1);
            
            string[] pathSegments = regularPath.Split('/');
            string[] finalResult = null;
            for (int i = 0; i < pathSegments.Length; i++)
            {
                if (finalResult == null)
                {
                    finalResult = new string[pathSegments.Length];
                    finalResult[i] = Util.Util.Path.GetCombinePath(fullPath,pathSegments[i]);
                }
                else
                {
                    finalResult[i] =Util.Util.Path.GetCombinePath(finalResult[finalResult.Length-1], pathSegments[i]);
                }
            }
            
            
            return finalResult;
        }
        
        private AssetInfoConfig IsHaveAssetPath(string path)
        {
            if (childConfigs != null)
            {
                foreach (var config in childConfigs)
                {
                    if (config.fullPath.Equals(path))
                        return config;
                }
            }

            return null;
        }
        
        private bool IsAddToChildAsset(string assetPath)
        {
            if (childConfigs != null && childConfigs.Count > 0)
            {
                foreach (var config in childConfigs)
                {
                    if (config.fullPath.Equals(assetPath))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        private bool CheckIsFolder
        {
            get
            {
                if(!string.IsNullOrEmpty(fullPath))
                    return AssetDatabase.IsValidFolder(fullPath);
                return false;
            }
        }*/
        
        
        /*public void RemoveInvalidChild(string childPath)
        {
            if (invalidChildConfigs != null && invalidChildConfigs.Count > 0)
            {
                if(invalidChildConfigs.Contains(childPath))
                    invalidChildConfigs.Remove(childPath);
            }
        }

        public void AddInvalidChild(string path)
        {
            if (invalidChildConfigs == null)
                invalidChildConfigs = new List<string>();
            if(!invalidChildConfigs.Contains(path))
                invalidChildConfigs.Add(path);
        }*/

    }
}