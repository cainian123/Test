using System.Collections.Generic;
using UnityEngine;


namespace AssetTools.Editor.AssetBundle.AssetBundleConfig
{
    [CreateAssetMenu(fileName = "AssetBundleConfig", menuName = "Tools/AssetBundleConfig", order = 0)]
    public class AssetBundleConfig :ScriptableObject
    {
        [SerializeField] public bool shaderOnePack;
        [SerializeField] public bool autoPack;
        [SerializeField]
        public List<AssetBundleConfigData> configData = new List<AssetBundleConfigData>();
        
       
    }
}
