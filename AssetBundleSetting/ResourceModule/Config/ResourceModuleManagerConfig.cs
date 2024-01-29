using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.Config
{
    [Serializable]
    public class ResourceModuleInfo
    {
        public string packageName;
        public string packagePath;

        public bool IsHaveExit
        {
            get
            {
                if (string.IsNullOrEmpty(packagePath))
                    return false;
                return File.Exists(packagePath);
            }
        }
    }
    
    public class ResourceModuleManagerConfig : ScriptableObject
    {
        [SerializeField] public List<ResourceModuleInfo> resourceModuleConfigs = new List<ResourceModuleInfo>();
    }
}