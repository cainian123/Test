using System.Collections.Generic;
using UnityEngine;

namespace AssetTools.Editor.AssetBundle.AssetBundleCollection
{
    public struct Redundant
    {
        public List<string> AssetBundleName;
        public long Count;

    }
    public class RedundantData
    {
        private Dictionary<string, Redundant> _allAssetRedundant;

        public Dictionary<string, Redundant> AllAssetRedundant => _allAssetRedundant;

        public RedundantData()
        {
            _allAssetRedundant = new Dictionary<string, Redundant>();
        }

        public void AddAsset(string path, string assetBundleName)
        {
            if (_allAssetRedundant.ContainsKey(path))
            {
                var redundant = _allAssetRedundant[path];

                if (!redundant.AssetBundleName.Contains(assetBundleName))
                {
                    redundant.AssetBundleName.Add(assetBundleName);
                    redundant.Count += 1;
                }
            }
            else
            {
                _allAssetRedundant.Add(path,new Redundant{Count =  1,AssetBundleName = new List<string>{assetBundleName}});
            }
        }
        
    }
}