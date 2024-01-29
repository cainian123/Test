
using System.Collections.Generic;
using System.IO;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.Config;
using AssetStream.Editor.AssetBundleSetting.ResourceModule.Data;
using UnityEditor;
using UnityEngine;


namespace AssetStream.Editor.AssetBundleSetting.ResourceModule
{
    [ExecuteInEditMode]
    public partial class ResourceModuleDataManager
    {
        
        private static ResourceModuleDataManager _instance;

        public static ResourceModuleDataManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ResourceModuleDataManager();
                return _instance;
            }
        }

        private const string ResourceModuleControllerName = "ResourceModuleController";
        
        private const string ResourceModuleControllerPath = "Assets/ResourceModule";

        private const string ResourceModulePath = "Assets/ResourceModule/ResourceModuleData";

        private const string ResourceModuleDefaultName = "Default-Package";

        private const string ExtensionName = ".asset";
        
        private string ResourceModuleControllerFullPath
        {
            get
            {
                string path = Util.Util.Path.GetCombinePath(ResourceModuleControllerPath, ResourceModuleControllerName);
                return $"{path}{ExtensionName}";
            }
        }
        
        private ResourceModuleManagerConfig m_ResourceModuleManagerConfig;
        
        public ResourceModuleManagerConfig ResourceModuleManagerConfig => m_ResourceModuleManagerConfig;

        private Dictionary<string, ResourceModuleConfig> m_ResourceModuleConfigs;

        //public Dictionary<string, ResourceModuleConfig> ResourceModuleConfigs => m_ResourceModuleConfigs;

        private List<ResourceModuleData> m_ResourceModuleDatas;   //所有有效的
        public void LoadData()
        {
            m_ResourceModuleConfigs = null;
            m_ResourceModuleManagerConfig = null;
            m_ResourceModuleDatas = null;
            string fullPath = ResourceModuleControllerFullPath;
            if (File.Exists(fullPath))
            {
                m_ResourceModuleManagerConfig = LoadScriptableObject<ResourceModuleManagerConfig>(ResourceModuleControllerFullPath);
                if (m_ResourceModuleManagerConfig != null && m_ResourceModuleManagerConfig.resourceModuleConfigs != null)
                {
                    foreach (var moduleInfo in m_ResourceModuleManagerConfig.resourceModuleConfigs)
                    {
                        var scriptableObject =
                            LoadScriptableObject<ResourceModuleConfig>(moduleInfo.packagePath);
                        AddResourceModule(moduleInfo.packageName, scriptableObject);
                    }
                }
            }
        }

        public bool CreateResourceModule(string name = "",bool immediatelySave = true)
        {
            bool isCreateSuccess = false;
            bool isCanCreate = true;
           
            if (m_ResourceModuleManagerConfig == null)
            {
                m_ResourceModuleManagerConfig = CreateScriptableObject<ResourceModuleManagerConfig>(ResourceModuleControllerPath,ResourceModuleControllerName,ExtensionName,out var finalName,out var fullPath);
                if (m_ResourceModuleManagerConfig == null)
                    isCanCreate = false;
            }

            if (isCanCreate)
            {
                var scriptableObject=CreateScriptableObject<ResourceModuleConfig>(ResourceModulePath,name,ExtensionName,out var finalName,out var fullPath);
                isCreateSuccess = scriptableObject != null;
                if (isCreateSuccess)
                {
                    ResourceModuleInfo moduleInfo = new ResourceModuleInfo();
                    moduleInfo.packageName = finalName;
                    moduleInfo.packagePath = fullPath;
                    m_ResourceModuleManagerConfig.resourceModuleConfigs.Add(moduleInfo);
                    AddResourceModule(finalName,scriptableObject);
                    if (immediatelySave)
                    {
                        SaveScriptableObjectData(m_ResourceModuleManagerConfig);
                    }
                }
            }
            
            return isCreateSuccess;
        }
        
        public bool CanUseResourceModule(string name)
        {
            if (m_ResourceModuleConfigs != null && m_ResourceModuleConfigs.TryGetValue(name, out var data))
            {
                return false;
            }

            return true;
        }

        public AssetPackageEnum GetPackageEnum(string packageName)
        {
            if(m_ResourceModuleConfigs != null && m_ResourceModuleConfigs.TryGetValue(packageName, out var data))
            {
                return data.assetPackageEnum;
            }

            return AssetPackageEnum.None;
        }
        
        private void AddResourceModule(string packageName,ResourceModuleConfig scriptableObject)
        {
            if (scriptableObject != null)
            {
                ResourceModuleData moduleData = new ResourceModuleData(scriptableObject.assetPackageEnum, scriptableObject.resourceModuleName,
                    scriptableObject.assetConfigs);
                if (m_ResourceModuleDatas == null)
                {
                    m_ResourceModuleDatas = new List<ResourceModuleData>();
                }

                m_ResourceModuleDatas.Add(moduleData);
            }

            if (m_ResourceModuleConfigs == null)
                m_ResourceModuleConfigs = new Dictionary<string, ResourceModuleConfig>();
            m_ResourceModuleConfigs.Add(packageName,scriptableObject);
        }

        private T CreateScriptableObject<T>(string path,string name,string extensionName,out string finalName,out string fullPath) where T : ScriptableObject
        {
            fullPath = "";
            finalName = GetScriptableObjectName(name);
            if (!string.IsNullOrEmpty(finalName))
            {
                string finalPath = Util.Util.Path.GetCombinePath(path, finalName);
                if (!string.IsNullOrEmpty(path))
                {
                    string outPath = Util.Util.Path.GetRegularPath(path);
                    if (!Directory.Exists(outPath))
                        Directory.CreateDirectory(outPath);

                    T asset = ScriptableObject.CreateInstance<T>();
                    fullPath = $"{finalPath}{extensionName}";

                    if (asset is ResourceModuleConfig temp)
                    {
                        temp.resourceModuleName = finalName;
                    }
                    AssetDatabase.CreateAsset(asset, fullPath);

                    AssetDatabase.SaveAssets();
                    return asset;
                }
            }

            return null;
        }

        private string GetScriptableObjectName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return GetDefaultName();
            }

            if (m_ResourceModuleConfigs != null )
            {
                if (m_ResourceModuleConfigs.TryGetValue(name, out var data))
                {
                    return "";
                }
                
            }

            return name;
        }

        private string GetDefaultName()
        {
            string useName = ResourceModuleDefaultName;
            if (m_ResourceModuleManagerConfig != null && m_ResourceModuleManagerConfig.resourceModuleConfigs != null &&
                m_ResourceModuleManagerConfig.resourceModuleConfigs.Count > 0)
            {
                long i = 0;
                foreach (var moduleInfo in m_ResourceModuleManagerConfig.resourceModuleConfigs)
                {
                    i++;
                    if (string.Equals(moduleInfo.packageName, useName))
                    {
                        useName = $"{useName}-{i}";
                    }
                }
            }

            return useName;
        }
        
        private T LoadScriptableObject<T>(string path) where T : ScriptableObject
        {
            var myScriptableObject = AssetDatabase.LoadAssetAtPath<T>(path);
            if (myScriptableObject == null)
            {
                Debug.Log("Failed to load ScriptableObject at path: " + path);
            }

            return myScriptableObject;
        }

        private void SaveScriptableObjectData(ScriptableObject scriptableObject)
        {
            EditorUtility.SetDirty(scriptableObject);
            AssetDatabase.SaveAssets();
        }
        
    }
}