using System.Collections.Generic;
using UnityEngine;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.Config
{
    public class ResourceModuleConfig : ScriptableObject
    {
        [SerializeField] public AssetPackageEnum assetPackageEnum;
        [SerializeField] public string resourceModuleName;
        [SerializeField] public List<AssetInfoConfig> assetConfigs;
        
        /*public bool AddAsset(string[] assetPaths)
        {
            if (assetPaths != null && assetPaths.Length > 0)
            {
                if (assetConfigs == null)
                {
                    assetConfigs = new List<AssetInfoConfig>();
                }

                bool isAdd = false;
                foreach (var path in assetPaths)
                {
                    var tempPaths = ConvertPathToTree(path);
                    if (tempPaths != null && tempPaths.Count > 0)
                    {
                        string startPath = tempPaths[0];
                        string[] newPaths = new string[tempPaths.Count - 1];
                        for (int i = 1; i < tempPaths.Count; i++)
                        {
                            newPaths[i - 1] = tempPaths[i];
                        }
                        AssetInfoConfig config = IsHaveAssetPath(startPath);
                        if (config == null)
                        {
                            config = new AssetInfoConfig(startPath);
                            isAdd |=config.AddNewAsset(path,newPaths);   
                            assetConfigs.Add(config);
                        }
                        else
                        {
                            isAdd |=config.AddNewAsset(path,newPaths);   
                        }
                    }
                }

                return isAdd;
            }

            return false;
        }
        
        private AssetInfoConfig IsHaveAssetPath(string path)
        {
            if (assetConfigs != null)
            {
                foreach (var config in assetConfigs)
                {
                    if (config.FullPath.Equals(path))
                        return config;
                }
            }

            return null;
        }*/

        /*AssetInfoConfig-->
 * 
 *A/B effect
 *A/B/C (删除A/B/C) ---> invalidChildConfigs
 *A/B/D
 *想要添加A/B/C/a
 * 
 */
        public bool AddAssetInfo(string[] assetPaths)
        {
            bool isAdd = false;
            if (assetPaths != null && assetPaths.Length > 0)
            {
                foreach (var path in assetPaths)
                {
                    bool isExit = CheckIsExit(path);
                    if (!isExit)
                    {
                        if (assetConfigs == null)
                        {
                            assetConfigs = new List<AssetInfoConfig>();
                        }

                        AssetInfoConfig config = new AssetInfoConfig(path);
                        assetConfigs.Add(config);
                        isAdd = true;
                    }
                }
            }

            return isAdd;
        }

        public bool RemoveAssetInfo(List<string> paths)
        {
            return false;
        }
        
        private bool CheckIsExit(string path)
        {
            if (assetConfigs != null)
            {
                foreach (var configPath in assetConfigs)
                {
                    if (configPath.FullPath.Equals(path))
                        return true;
                }
            }
            return false;
        }
        
        
        
        /*public bool AddAssetInfo(string[] assetPaths)
        {
            if (assetPaths != null && assetPaths.Length > 0)
            {
                bool isChange = false;
                if (assetConfigs == null)
                {
                    assetConfigs = new List<AssetInfoConfig>();
                }

                foreach (var path in assetPaths)
                {
                    bool isFind = false;
                    var tempPaths = ConvertPathToTree(path);
                    if (tempPaths != null && tempPaths.Count > 0)
                    {
                        foreach (var temp in tempPaths)
                        {
                            var assetInfo = IsHaveAsset(temp);
                            if (assetInfo != null)
                            {
                                assetInfo.RemoveInvalidChild(path);
                                isFind = true;
                                isChange = true;
                                break;
                            }
                        }
                    }

                    if (!isFind)
                    {
                        AssetInfoConfig config = new AssetInfoConfig();
                        config.fullPath = path;
                        assetConfigs.Add(config);    
                        isChange = true;
                    }
                    
                }

                if (isChange)
                {
                    CheckAssetConfigs();
                    return true;
                }
            }

            return false;
        }

        private void CheckAssetConfigs()
        {
            if (assetConfigs != null)
            {
                
            }
        }

        private AssetInfoConfig IsHaveAsset(string path)
        {
            if (assetConfigs != null)
            {
                foreach (var config in assetConfigs)
                {
                    if (config.fullPath.Equals(path))
                        return config;
                }
            }

            return null;
        }*/
        
        private  List<string> ConvertPathToTree(string path)
        {
            List<string> result = new List<string>();
            string[] pathSegments = path.Split('/');
            
            for (int i = 0; i < pathSegments.Length; i++)
            {
                if (result.Count <= 0)
                {
                    result.Add(pathSegments[i]);
                }
                else
                {
                    result.Add(Util.Util.Path.GetCombinePath(result[result.Count-1], pathSegments[i]));
                }
            }

            List<string> finalResult = new List<string>();
            bool startAdd = false;
            foreach (var tempPath in result)
            {
                //if (tempPath.EndsWith("_Resources"))
                {
                    startAdd = true;
                }

                if (startAdd)
                {
                    finalResult.Add(tempPath);
                }
            }
            
            return finalResult;
        }
    }
}