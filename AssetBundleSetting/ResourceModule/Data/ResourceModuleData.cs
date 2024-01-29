using System.Collections.Generic;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.Config;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.Data
{
    public class ResourceModuleData
    {
        private AssetPackageEnum m_AssetPackageEnum;
        private string m_ResourceModuleName;
        private List<AssetBaseInfo> m_AssetConfigDatas;
        
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
                    AssetBaseInfo info = new AssetBaseInfo(config.fullPath,m_ResourceModuleName,m_AssetPackageEnum,null);
                    info.LoadChild(config.invalidChildConfigs);
                    m_AssetConfigDatas.Add(info);
                }
            }
        }

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