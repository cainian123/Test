using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using AssetTools.Editor.AssetBundle.AssetBundleCollection;
using UnityEditor;
using UnityEngine;
using AssetTools.Editor.Util;

namespace AssetTools.Editor.AssetBundle.AssetBundleAnalyzer
{
    public partial class AssetBundleAnalyzerController
    {
        private const string BuildReportName = "AnalyzerReport.xml";
        private const string RedundancyReportName = "RedundancyReport.xml";
        private AssetBundleCollection.AssetBundleCollection m_AssetBundleCollection;
        private readonly HashSet<Stamp> m_AnalyzedStamps;
        private readonly Dictionary<string, List<Asset>> m_ScatteredAssets;  //key  引用的没有ab的离散资源  value 谁引用的
        private readonly Dictionary<string, DependencyData> m_DependencyDatas;
        private readonly SortedDictionary<string, Asset> m_Assets;
        private SortedDictionary<string, Resource> m_AssetBundles;

        public AssetBundleAnalyzerController(string configPath)
        {
            if(!string.IsNullOrEmpty(configPath))
                m_AssetBundleCollection = new AssetBundleCollection.AssetBundleCollection(configPath);
            m_DependencyDatas = new Dictionary<string, DependencyData>(StringComparer.Ordinal);
            m_ScatteredAssets = new Dictionary<string, List<Asset>>(StringComparer.Ordinal);
            m_AnalyzedStamps = new HashSet<Stamp>();
        }

        public AssetBundleAnalyzerController(Asset[] assets,SortedDictionary<string, Resource> assetbundles) : this(string.Empty)
        {
            if (assets != null && assets.Length > 0)
            {
                m_AssetBundles = assetbundles;
                m_Assets = new SortedDictionary<string, Asset>();
                for (int i = 0; i < assets.Length; i++)
                {
                    m_Assets.Add(assets[i].Name,assets[i]);
                }
                Analyzer(assets);   
            }
        }

        public void StartAnalyzer()
        {
            if(m_AssetBundleCollection==null)
                return;
            m_AssetBundleCollection.Load();
            Asset[] assets = m_AssetBundleCollection.GetAssets();
            Analyzer(assets);
        }
        

        private void Analyzer(Asset[] assets)
        { 
            EditorUtility.DisplayProgressBar("Analyzer AssetBundle Start","Analyzer Prepare",1);
            
            try
            {
                m_DependencyDatas.Clear();
                m_ScatteredAssets.Clear();
                m_AnalyzedStamps.Clear();
                HashSet<string> scriptAssetNames = GetFilteredAssetNames("t:Script");
                
                int count = assets.Length;
                for (int i = 0; i < count; i++)
                {
                    EditorUtility.DisplayProgressBar("Analyzer AssetBundle", Utility.Text.Format("Analyzing Asset Info, {0}/{1}", i, count), (float)i / count);

                    string assetName = assets[i].Name;
                    if (string.IsNullOrEmpty(assetName))
                    {
                        Debug.LogWarning(Utility.Text.Format("Can not find asset by guid '{0}'.", assets[i].Guid));
                        continue;
                    }
                
                    DependencyData dependencyData = new DependencyData();
                    AnalyzeAsset(assetName, assets[i], dependencyData, scriptAssetNames);
                    dependencyData.RefreshData();
                    m_DependencyDatas.Add(assetName, dependencyData);
                }
            
                foreach (List<Asset> scatteredAsset in m_ScatteredAssets.Values)
                {
                    
                    scatteredAsset.Sort((a, b) => a.Name.CompareTo(b.Name));
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
           
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void AnalyzeAsset(string assetName, Asset hostAsset, DependencyData dependencyData,
            HashSet<string> scriptAssetNames)
        {
            string[] dependencyAssetNames = AssetDatabase.GetDependencies(assetName, false);
            foreach (string dependencyAssetName in dependencyAssetNames)
            {
                if (scriptAssetNames.Contains(dependencyAssetName))
                {
                    continue;
                }

                if (dependencyAssetName == assetName)
                {
                    continue;
                }

                if (dependencyAssetName.EndsWith(".unity", StringComparison.Ordinal))
                {
                    // 忽略对场景的依赖
                    continue;
                }
                Stamp stamp = new Stamp(hostAsset.Name, dependencyAssetName);
                if (m_AnalyzedStamps.Contains(stamp))
                {
                    continue;
                }

                m_AnalyzedStamps.Add(stamp);
                string guid = AssetDatabase.AssetPathToGUID(dependencyAssetName);
                if (string.IsNullOrEmpty(guid))
                {
                    Debug.LogWarning(Utility.Text.Format("Can not find guid by asset '{0}'.", dependencyAssetName));
                    continue;
                }

                Asset asset = GetAsset(dependencyAssetName);
                if (asset != null)
                {
                    dependencyData.AddDependencyAsset(asset);
                }
                else
                {
                    dependencyData.AddScatteredDependencyAsset(dependencyAssetName);

                    List<Asset> scatteredAssets = null;
                    if (!m_ScatteredAssets.TryGetValue(dependencyAssetName, out scatteredAssets))
                    {
                        scatteredAssets = new List<Asset>();
                        m_ScatteredAssets.Add(dependencyAssetName, scatteredAssets);
                    }

                    scatteredAssets.Add(hostAsset);

                    AnalyzeAsset(dependencyAssetName, hostAsset, dependencyData, scriptAssetNames);
                }
            }
        }

        private Asset GetAsset(string path)
        {
            if (m_AssetBundleCollection == null)
            {
                if (m_Assets != null && m_Assets.ContainsKey(path))
                    return m_Assets[path];
                return null;
            }
            else
            {
                return m_AssetBundleCollection.GetAsset(path);
            }
        }

        private HashSet<string> GetFilteredAssetNames(string filter)
        {
            string[] filterAssetGuids = AssetDatabase.FindAssets(filter);
            HashSet<string> filterAssetNames = new HashSet<string>();
            foreach (string filterAssetGuid in filterAssetGuids)
            {
                filterAssetNames.Add(AssetDatabase.GUIDToAssetPath(filterAssetGuid));
            }

            return filterAssetNames;
        }

        public void PrintAnalyzer(string path)
        {
            try
            {
                SaveAnalyzer(path);
                SaveRedundancy(path);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private SortedDictionary<string, Resource> AssetBundles
        {
            get
            {
                if (m_AssetBundleCollection == null)
                    return m_AssetBundles;
                return m_AssetBundleCollection.AssetBundles;
            }
        }
        private void SaveAnalyzer(string path)
        {
            path = Utility.Path.GetRegularPath(path);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            path = Utility.Path.GetRegularPath(Utility.Path.GetCombinePath(path, BuildReportName));
            XmlElement xmlElement = null;
            XmlAttribute xmlAttribute = null;

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));

            XmlElement xmlRoot = xmlDocument.CreateElement("AssetBundleAnalyzer");
            xmlDocument.AppendChild(xmlRoot);
            
            XmlElement xmlBuildReport = xmlDocument.CreateElement("AnalyzerReport");
            xmlRoot.AppendChild(xmlBuildReport);

            var assetBundles = AssetBundles;
            XmlElement xmlResources = xmlDocument.CreateElement("Resources");
            xmlAttribute = xmlDocument.CreateAttribute("Count");
            xmlAttribute.Value = assetBundles.Count.ToString();
            xmlResources.Attributes.SetNamedItem(xmlAttribute);
            xmlBuildReport.AppendChild(xmlResources);

            if (assetBundles.Count > 0)
            {
                foreach (string key in assetBundles.Keys)
                {
                    XmlElement xmlResource = xmlDocument.CreateElement("Resource");
                    xmlAttribute = xmlDocument.CreateAttribute("Name");
                    xmlAttribute.Value = key;
                    xmlResource.Attributes.SetNamedItem(xmlAttribute);
                    xmlResources.AppendChild(xmlResource);

                    Resource assetData = assetBundles[key];
                    if (assetData?.GetAssets() != null)
                    {
                        Asset[] assets = assetData.GetAssets();
                        XmlElement xmlAssets = xmlDocument.CreateElement("Assets");
                        xmlAttribute = xmlDocument.CreateAttribute("Count");
                        xmlAttribute.Value = assets.Length.ToString();
                        xmlAssets.Attributes.SetNamedItem(xmlAttribute);
                        xmlResource.AppendChild(xmlAssets);

                        foreach (var asset in assets)
                        {
                            XmlElement xmlAsset = xmlDocument.CreateElement("Asset");
                            xmlAttribute = xmlDocument.CreateAttribute("Name");
                            xmlAttribute.Value = asset.Name;
                            xmlAsset.Attributes.SetNamedItem(xmlAttribute);
                            xmlAssets.AppendChild(xmlAsset);

                            if (m_DependencyDatas != null && m_DependencyDatas.ContainsKey(asset.Name))
                            {
                                DependencyData dependencyData = m_DependencyDatas[asset.Name];
                                if (dependencyData != null)
                                {
                                    Asset[] dependencyAssets = dependencyData.GetDependencyAssets();
                                    if (dependencyAssets.Length > 0)
                                    {
                                        XmlElement xmlDependencyAssets = xmlDocument.CreateElement("DependencyAssets");
                                        xmlAttribute = xmlDocument.CreateAttribute("Count");
                                        xmlAttribute.Value = dependencyAssets.Length.ToString();
                                        xmlDependencyAssets.Attributes.SetNamedItem(xmlAttribute);
                                        xmlAsset.AppendChild(xmlDependencyAssets);
                                        foreach (Asset dependencyAssetName in dependencyAssets)
                                        {
                                            XmlElement xmlDependencyAsset = xmlDocument.CreateElement("DependencyAsset");
                                            xmlAttribute = xmlDocument.CreateAttribute("Name");
                                            xmlAttribute.Value = dependencyAssetName.Name;
                                            xmlDependencyAsset.Attributes.SetNamedItem(xmlAttribute);
                                            xmlDependencyAssets.AppendChild(xmlDependencyAsset);
                                        }
                                    }
                                    
                                    string[] scatteredDependencyAssetNames = dependencyData.GetScatteredDependencyAssetNames();
                                    if (scatteredDependencyAssetNames.Length > 0)
                                    {
                                        XmlElement xmlDependencyAssets = xmlDocument.CreateElement("DependencyScatterAssets");
                                        xmlAttribute = xmlDocument.CreateAttribute("Count");
                                        xmlAttribute.Value = scatteredDependencyAssetNames.Length.ToString();
                                        xmlDependencyAssets.Attributes.SetNamedItem(xmlAttribute);
                                        xmlAsset.AppendChild(xmlDependencyAssets);
                                        foreach (string dependencyAssetName in scatteredDependencyAssetNames)
                                        {
                                            XmlElement xmlDependencyAsset = xmlDocument.CreateElement("DependencyAsset");
                                            xmlAttribute = xmlDocument.CreateAttribute("Name");
                                            xmlAttribute.Value = dependencyAssetName;
                                            xmlDependencyAsset.Attributes.SetNamedItem(xmlAttribute);
                                            xmlDependencyAssets.AppendChild(xmlDependencyAsset);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            xmlDocument.Save(path);
        }

        
        private Dictionary<string, Dictionary<string, List<Asset>>> m_tempScatteredResourcesList =
            new Dictionary<string, Dictionary<string, List<Asset>>>();

        private void GetRedundancyList()
        {
            m_tempScatteredResourcesList.Clear();
            if (m_ScatteredAssets.Count > 0)
            {
                foreach (var path in m_ScatteredAssets.Keys)
                {
                    if (m_ScatteredAssets[path].Count > 1)
                    {
                        if (!m_tempScatteredResourcesList.ContainsKey(path))
                        {
                            Dictionary<string,List<Asset>> tempAssets = new Dictionary<string, List<Asset>>();
                            m_tempScatteredResourcesList.Add(path,tempAssets);
                        }

                        foreach (var asset in m_ScatteredAssets[path])
                        {
                            if (m_tempScatteredResourcesList[path].ContainsKey(asset.Resource.FullName))
                            {
                                m_tempScatteredResourcesList[path][asset.Resource.FullName].Add(asset);
                            }
                            else
                            {
                                m_tempScatteredResourcesList[path].Add(asset.Resource.FullName,new List<Asset>{asset});
                            }
                        }
                    }
                }
            }

            List<string> tempKey = new List<string>();
            foreach (var key in m_tempScatteredResourcesList.Keys)
            {
                if (m_tempScatteredResourcesList[key].Count <= 1)
                {
                    tempKey.Add(key);
                }
            }

            foreach (var key in tempKey)
            {
                m_tempScatteredResourcesList.Remove(key);
            }
        }
  

        private void SaveRedundancy(string path)
        {

            GetRedundancyList();
            path = Utility.Path.GetRegularPath(path);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            path = Utility.Path.GetRegularPath(Utility.Path.GetCombinePath(path, RedundancyReportName));
            XmlElement xmlElement = null;
            XmlAttribute xmlAttribute = null;
            
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));

            XmlElement xmlRoot = xmlDocument.CreateElement("RedundancyAssets");
            xmlDocument.AppendChild(xmlRoot);
            
            XmlElement xmlBuildReport = xmlDocument.CreateElement("AnalyzerReport");
            xmlRoot.AppendChild(xmlBuildReport);
            
            XmlElement xmlResources = xmlDocument.CreateElement("Redundancy");
            xmlAttribute = xmlDocument.CreateAttribute("Count");
            xmlAttribute.Value = m_tempScatteredResourcesList.Count.ToString();
            xmlResources.Attributes.SetNamedItem(xmlAttribute);
            xmlBuildReport.AppendChild(xmlResources);

            if (m_tempScatteredResourcesList.Count > 0)
            {
                foreach (var key in m_tempScatteredResourcesList.Keys)
                {
                    if(m_tempScatteredResourcesList[key].Count <=1)
                        continue;
                    XmlElement xmlResource = xmlDocument.CreateElement("ScatteredAsset");
                    xmlAttribute = xmlDocument.CreateAttribute("Path");
                    xmlAttribute.Value = key;
                    xmlResource.Attributes.SetNamedItem(xmlAttribute);
                    xmlAttribute = xmlDocument.CreateAttribute("Count");
                    xmlAttribute.Value = m_tempScatteredResourcesList[key].Count.ToString();
                    xmlResource.Attributes.SetNamedItem(xmlAttribute);
                    xmlResources.AppendChild(xmlResource);

                    foreach (var assetBundleName in m_tempScatteredResourcesList[key].Keys)
                    {
                        XmlElement xmlAssetBundle = xmlDocument.CreateElement("AssetBundle");
                        xmlAttribute = xmlDocument.CreateAttribute("Name");
                        xmlAttribute.Value = assetBundleName;
                        xmlAssetBundle.Attributes.SetNamedItem(xmlAttribute);
                        xmlAttribute = xmlDocument.CreateAttribute("Count");
                        xmlAttribute.Value = m_tempScatteredResourcesList[key][assetBundleName].Count.ToString();
                        xmlAssetBundle.Attributes.SetNamedItem(xmlAttribute);
                        xmlResource.AppendChild(xmlAssetBundle);

                        foreach (var asset in m_tempScatteredResourcesList[key][assetBundleName])
                        {
                            XmlElement xmlAsset = xmlDocument.CreateElement("Asset");
                            xmlAttribute = xmlDocument.CreateAttribute("Path");
                            xmlAttribute.Value = asset.Name;
                            xmlAsset.Attributes.SetNamedItem(xmlAttribute);
                            xmlAssetBundle.AppendChild(xmlAsset);
                        }
                    }
                }
            }
            
            xmlDocument.Save(path);
        }
    }
}
