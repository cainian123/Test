using UnityEngine;

using System;
using UnityEngine.Serialization;

namespace AssetTools.Editor.AssetBundle.AssetBundleConfig
{
    public enum AssetBundlePackType : int
    {
        OneFolderPack = 0,  //每个文件夹一个
        OneItemPack = 1,    //每个资源一个
        ChildFolderPack = 2, //子文件夹一个
        SomeNameAndVariant = 3,
    }
    
    [Serializable]
    public class AssetBundleConfigData
    {
        [SerializeField]
        public string path;
        [SerializeField] 
        public string disName;
        [SerializeField]
        public AssetBundlePackType packType = AssetBundlePackType.OneFolderPack;
        [SerializeField]
        public string sourceAssetUnionTypeFilter;
        [SerializeField]
        public bool isStreamAsset;

        [SerializeField] public bool isBaseBundle;
        [SerializeField] public bool isCompress = true;
    }
}