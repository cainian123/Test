using System;
using System.Collections.Generic;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.Config
{
    [Serializable]
    public class AssetInfoConfig
    {
        public string fullPath;
        //public bool isEfficient = true;
        public List<string> invalidChildConfigs;
    }
}