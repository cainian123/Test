using System.Collections.Generic;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.Config;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.Data
{
    public class ResourceModuleData
    {
        private AssetPackageEnum m_AssetPackageEnum;
        private string m_ResourceModuleName;
        private List<AssetBaseInfo> m_AssetConfigDatas;

        public List<AssetBaseInfo> AssetConfigDatas => m_AssetConfigDatas;

        public string ResourceModuleName => m_ResourceModuleName;

        public ResourceModuleData(AssetPackageEnum packageEnum,string resourceModuleName,List<AssetInfoConfig> configs)
        {
            m_AssetPackageEnum = packageEnum;
            m_ResourceModuleName = resourceModuleName;
            LoadAssetInfo(configs);
        }

        private void LoadAssetInfo(List<AssetInfoConfig> configs)
        {
            m_AssetConfigDatas = new List<AssetBaseInfo>();
            if (configs != null && configs.Count > 0)
            {
                foreach (var config in configs)
                {
                    AssetBaseInfo info = new AssetBaseInfo(config.FullPath, m_ResourceModuleName, m_AssetPackageEnum, null);
                    info.LoadChild(configs, config.InvalidChildConfigs);
                    m_AssetConfigDatas.Add(info);   
                }
            }
        }

        public void ForceLoadAssetInfo(List<AssetInfoConfig> configs)
        {
            LoadAssetInfo(configs);
        }

        /*private void LoadAssetInfo(List<AssetInfoConfig> configs)
        {
            m_AssetConfigDatas = new List<AssetBaseInfo>();
            if (configs != null && configs.Count > 0)
            {
                
                foreach (var config in configs)
                {
                    List<AssetInfoConfig> temp = new List<AssetInfoConfig>();
                    config.GetEfficientPath(ref temp);
                    if (temp.Count > 0)
                    {
                        foreach (var assetInfoConfig in temp)
                        {
                            AssetBaseInfo info = new AssetBaseInfo(assetInfoConfig.FullPath, m_ResourceModuleName, m_AssetPackageEnum, null, true);
                            info.LoadChild(assetInfoConfig.ChildConfigs);
                            m_AssetConfigDatas.Add(info);   
                        }
                    }
                   
                }
            }
        }

        public void ForceLoadAssetInfo(List<AssetInfoConfig> configs)
        {
            LoadAssetInfo(configs);
        }
*/
        public void RenamePackage(string newName)
        {
            m_ResourceModuleName = newName;
            foreach (var assetConfigData in m_AssetConfigDatas)
            {
                assetConfigData.RenamePackage(newName);
            }
        }
        
        public void ChangePackageType(string newName)
        {
            m_ResourceModuleName = newName;
            foreach (var assetConfigData in m_AssetConfigDatas)
            {
                assetConfigData.RenamePackage(newName);
            }
        }
    }
}