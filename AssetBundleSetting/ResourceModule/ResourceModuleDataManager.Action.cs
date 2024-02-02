using System.Collections.Generic;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.Config;
using UnityEditor;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule
{
    public partial class ResourceModuleDataManager
    {
        public void RenameResourceModule(string oldName,string newName,bool autoSave = true)
        {
            if (m_ResourceModuleConfigs != null && m_ResourceModuleConfigs.TryGetValue(oldName, out var data))
            {
                data.resourceModuleName = newName;
                if(autoSave)
                    SaveScriptableObjectData(data);
                string path = Util.Util.Path.GetCombinePath(ResourceModulePath, $"{oldName}{ExtensionName}");
                AssetDatabase.RenameAsset(path, $"{newName}{ExtensionName}");
                
                m_ResourceModuleConfigs.Remove(oldName);
                m_ResourceModuleConfigs.Add(newName,data);

                if (m_ResourceModuleDatas != null)
                {
                    foreach (var moduleData in m_ResourceModuleDatas)
                    {
                        if (moduleData.ResourceModuleName == oldName)
                        {
                            moduleData.RenamePackage(newName);
                            break;
                        }
                    }
                }
                
                if (m_ResourceModuleManagerConfig != null && m_ResourceModuleManagerConfig.resourceModuleConfigs != null)
                {
                    foreach (var moduleInfo in m_ResourceModuleManagerConfig.resourceModuleConfigs)
                    {
                        if (moduleInfo.packageName == oldName)
                        {
                            moduleInfo.packageName = newName;
                            moduleInfo.packagePath = Util.Util.Path.GetCombinePath(ResourceModulePath, $"{newName}{ExtensionName}");;
                            break;
                        }
                    }
                    if(autoSave)
                        SaveScriptableObjectData(m_ResourceModuleManagerConfig);
                }
            }
        }

        public void RemoveResourceModule(string packageName,bool autoSave = true)
        {
            if (m_ResourceModuleConfigs != null && m_ResourceModuleConfigs.TryGetValue(packageName, out var data))
            {
                string path = "";
                
                if (m_ResourceModuleDatas != null)
                {
                    
                    for (int i = 0; i < m_ResourceModuleDatas.Count; i++)
                    {
                        var moduleData = m_ResourceModuleDatas[i];
                        if (moduleData.ResourceModuleName == packageName)
                        {
                            m_ResourceModuleDatas.RemoveAt(i);
                            break;
                        }
                    }
                }
                
                if(m_ResourceModuleManagerConfig !=null && m_ResourceModuleManagerConfig.resourceModuleConfigs != null)
                {
                    foreach (var moduleInfo in m_ResourceModuleManagerConfig.resourceModuleConfigs)
                    {
                        if (moduleInfo.packageName == packageName)
                        {
                            path = moduleInfo.packagePath;
                            m_ResourceModuleManagerConfig.resourceModuleConfigs.Remove(moduleInfo);
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(path))
                    {
                        AssetDatabase.DeleteAsset(path);
                        //AssetDatabase.Refresh();
                    }
                }
                
                m_ResourceModuleConfigs.Remove(packageName);
                if(autoSave)
                    SaveScriptableObjectData(m_ResourceModuleManagerConfig);
            }
        }

        public void ChangePackageType(string packageName,AssetPackageEnum newPackageEnum,bool autoSave)
        {
            if(m_ResourceModuleConfigs != null && m_ResourceModuleConfigs.TryGetValue(packageName, out var data))
            {
                data.assetPackageEnum = newPackageEnum;
                
                if (m_ResourceModuleDatas != null)
                {
                    foreach (var moduleData in m_ResourceModuleDatas)
                    {
                        if (moduleData.ResourceModuleName == packageName)
                        {
                            moduleData.ChangePackageType(packageName);
                            break;
                        }
                    }
                }
                
                if(autoSave)
                    SaveScriptableObjectData(data);
            }
        }
        
        public bool AddAssetToResourceModule(string packageName,string[] assetPaths,bool autoSave = true)
        {
            if (m_ResourceModuleConfigs != null && m_ResourceModuleConfigs.TryGetValue(packageName, out var data))
            {
                if (data.AddAssetInfo(assetPaths))
                {
                    var moduleData = GetModuleData(packageName);
                    if (moduleData != null)
                    {
                        moduleData.ForceLoadAssetInfo(data.assetConfigs);
                    }
                    if(autoSave)
                        SaveScriptableObjectData(data);
                    return true;
                }
            }

            return false;
        }
        
        public bool RemoveAssetFromResourceModule(string packageName,Dictionary<string, string> assetPaths,bool autoSave = true)
        {
            if (m_ResourceModuleConfigs != null && m_ResourceModuleConfigs.TryGetValue(packageName, out var data))
            {
                if (data.RemoveAssetInfo(assetPaths))
                {
                    var moduleData = GetModuleData(packageName);
                    if (moduleData != null)
                    {
                        moduleData.ForceLoadAssetInfo(data.assetConfigs);
                    }
                    if(autoSave)
                        SaveScriptableObjectData(data);
                    return true;
                }
            }

            return false;
        }
    }
}