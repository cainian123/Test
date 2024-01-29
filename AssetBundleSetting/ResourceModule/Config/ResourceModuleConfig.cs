using System.Collections.Generic;
using UnityEngine;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.Config
{
    public class ResourceModuleConfig : ScriptableObject
    {
        [SerializeField] public AssetPackageEnum assetPackageEnum;
        [SerializeField] public string resourceModuleName;
        [SerializeField] public List<AssetInfoConfig> assetConfigs;
    }
}