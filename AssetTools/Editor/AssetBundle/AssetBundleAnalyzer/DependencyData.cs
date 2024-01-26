using System;
using System.Collections.Generic;
using AssetTools.Editor.AssetBundle.AssetBundleCollection;

namespace AssetTools.Editor.AssetBundle.AssetBundleAnalyzer
{
    public class DependencyData
    {
  
        private List<AssetBundleCollection.Resource> m_DependencyResources;
        private List<Asset> m_DependencyAssets;
        private List<string> m_ScatteredDependencyAssetNames;

        public DependencyData()
        {
            m_DependencyResources = new List<AssetBundleCollection.Resource>();
            m_DependencyAssets = new List<Asset>();
            m_ScatteredDependencyAssetNames= new List<string>();
        }
        
        public int DependencyResourceCount => m_DependencyResources.Count;

        public int DependencyAssetCount => m_DependencyAssets.Count;

        public int ScatteredDependencyAssetCount => m_ScatteredDependencyAssetNames.Count;

        public void AddDependencyAsset(Asset asset)
        {
            if (!m_DependencyResources.Contains(asset.Resource))
            {
                m_DependencyResources.Add(asset.Resource);
            }

            m_DependencyAssets.Add(asset);
        }
        
        public void AddScatteredDependencyAsset(string dependencyAssetName)
        {
            m_ScatteredDependencyAssetNames.Add(dependencyAssetName);
        }

        public AssetBundleCollection.Resource[] GetDependencyResources()
        {
            return m_DependencyResources.ToArray();
        }

        public Asset[] GetDependencyAssets()
        {
            return m_DependencyAssets.ToArray();
        }

        public string[] GetScatteredDependencyAssetNames()
        {
            return m_ScatteredDependencyAssetNames.ToArray();
        }
        
        public void RefreshData()
        {
            m_DependencyResources.Sort(DependencyResourcesComparer);
            m_DependencyAssets.Sort(DependencyAssetsComparer);
            m_ScatteredDependencyAssetNames.Sort();
        }

        private int DependencyResourcesComparer(AssetBundleCollection.Resource a, AssetBundleCollection.Resource b)
        {
            return String.Compare(a.FullName, b.FullName, StringComparison.Ordinal);
        }

        private int DependencyAssetsComparer(Asset a, Asset b)
        {
            return String.Compare(a.Name, b.Name, StringComparison.Ordinal);
        }
        
    }
}