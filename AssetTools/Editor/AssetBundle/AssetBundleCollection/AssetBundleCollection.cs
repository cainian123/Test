using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AssetTools.Editor.AssetBundle.AssetBundleConfig;
using AssetTools.Editor.Util;
using UnityEditor;
using UnityEngine;

namespace AssetTools.Editor.AssetBundle.AssetBundleCollection
{
    public class AssetBundleCollection
    {
        private const string PostfixOfScene = ".unity";
        private const string MetaItem = ".meta";
        private const string Shader = ".shader";
        private const string ShaderBundle = "shader_bundle";
        private static readonly Regex AssetBundleNameRegex = new Regex(@"^([A-Za-z0-9\._-]+/)*[A-Za-z0-9\._-]+$");
        private static readonly Regex AssetBundleVariantRegex = new Regex(@"^[a-z0-9_-]+$");
        private readonly string _sourceAssetUnionTypeFilter =
            "t:Scene t:Prefab t:Shader t:Model t:Material t:Texture t:AudioClip t:AnimationClip t:AnimatorController t:Font t:TextAsset t:ScriptableObject";

        private readonly string sourceAssetExceptTypeFilter = "t:Script";
        private  string _configPath ;
        private List<AssetBundleNameInfo> _allAssetBundleInfos;
        private AssetBundleConfig.AssetBundleConfig _assetBundleConfig;
        private readonly SortedDictionary<string, Resource> m_AssetBundles;  //key assetBundle Name  value asset
        private readonly SortedDictionary<string, Asset> m_Assets;
        private RedundantData _redundantData;

        public SortedDictionary<string, Resource> AssetBundles => m_AssetBundles;

        public AssetBundleCollection(string configPath)
        {
            _configPath = configPath;
            m_AssetBundles = new SortedDictionary<string, Resource>();
            m_Assets = new SortedDictionary<string, Asset>();
            _redundantData = new RedundantData();
        }
        
        public void Load()
        {
            EditorUtility.DisplayProgressBar("Load AssetBundle Info","Load AssetBundle Prepare",1);
            try
            {
                LoadConfigInfo();
                StartCollectionItem();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
         
        }
        
        private void LoadConfigInfo()
        {
            Debug.LogFormat("Load Config Data,Path {0}",_configPath);
            _assetBundleConfig=AssetDatabase.LoadAssetAtPath<AssetBundleConfig.AssetBundleConfig>(_configPath);
            if (_assetBundleConfig)
            {
                _allAssetBundleInfos = new List<AssetBundleNameInfo>();
                int i = 1;
                foreach (var configData in _assetBundleConfig.configData)
                {
                    
                    EditorUtility.DisplayProgressBar("Load AssetBundle Info", Utility.Text.Format("Analyzing AssetBundle Config Info, {0}/{1} analyzed.", i, _assetBundleConfig.configData.Count), (float)i / _assetBundleConfig.configData.Count);

                    AddPath(configData.path);
                    i++;
                }
                //PrintLog();
            }
            Debug.Log("Load Config Data Over");
        }

        private void AddPath(string path)
        {
            if(string.IsNullOrEmpty(path))
                return;
            if (_allAssetBundleInfos.Count <= 0)
            {
                _allAssetBundleInfos.Add(new AssetBundleNameInfo(path));
                return;
            }
            foreach (var bundleInfo in _allAssetBundleInfos)
            {
                if(bundleInfo.AddPath(path))
                    return;
            }
            _allAssetBundleInfos.Add(new AssetBundleNameInfo(path));
        }

        private AssetBundleConfigData GetBundleConfig(string path)
        {
            if (_assetBundleConfig != null && _assetBundleConfig.configData != null)
            {
                foreach (var configData in _assetBundleConfig.configData)
                {
                    if (configData.path == path)
                        return configData;
                }
            }

            return null;
        }
        private void StartCollectionItem()
        {
            Debug.Log("Start Collection Asset");
            if (_allAssetBundleInfos != null)
            {
                for (int i = 0; i < _allAssetBundleInfos.Count; i++)
                {
                    EditorUtility.DisplayProgressBar("Load AssetBundle Info", Utility.Text.Format("Collection AssetBundle, {0}/{1} .", i+1, _allAssetBundleInfos.Count), (float)i+1 / _allAssetBundleInfos.Count);

                    _allAssetBundleInfos[i].StartCollectionAsset(CollectionItem);
                }
                EditorUtility.ClearProgressBar();
            }
            Debug.Log("Start Collection Asset Over");
            AutoCollectionItemToPack();
        }

        private void CollectionItem(string path,AssetBundleNameInfo nameInfo)
        {
            AssetBundleConfigData configData = GetBundleConfig(path);
            if (configData == null)
            {
                Debug.LogFormat("{0} not find ConfigData",path);
                return;
            }

            string filter = string.IsNullOrEmpty(configData.sourceAssetUnionTypeFilter) ? _sourceAssetUnionTypeFilter : configData.sourceAssetUnionTypeFilter;
            CollectionItem(path,nameInfo,configData.packType,filter,configData.isStreamAsset,configData.isBaseBundle,configData.isCompress);
        }

        private void CollectionItem(string path,AssetBundleNameInfo nameInfo, AssetBundlePackType packType, string sourceTypeFilter, bool isEnterPack,bool baseBundle,bool isCompress)
        {
            switch (packType)
            {
                case AssetBundlePackType.OneItemPack:
                    CollectionItemByEveryOnePack(path,nameInfo,sourceTypeFilter,isEnterPack,baseBundle,isCompress);
                    break;
                case AssetBundlePackType.OneFolderPack:
                    CollectionItemByOneFolderPack(path,nameInfo,sourceTypeFilter,isEnterPack,baseBundle,isCompress);
                    break;
                case AssetBundlePackType.ChildFolderPack:
                    CollectionItemByFolderPack(path,nameInfo,sourceTypeFilter,isEnterPack,baseBundle,isCompress);
                    break;
                case AssetBundlePackType.SomeNameAndVariant:
                    CollectionItemBySomeNameAddVariant(path, nameInfo, sourceTypeFilter, isEnterPack, baseBundle,
                        isCompress);
                    break;
                default:
                    Debug.LogFormat("{0} is error configData",path);
                    break;
            }
        }
        
        /// <summary>
        /// 一个文件夹下每一个item打成一个包
        /// </summary>
        /// <param name="path"></param>
        ///
        private void CollectionItemByEveryOnePack(string path,AssetBundleNameInfo nameInfo,string sourceTypeFilter,bool isEnterPack,bool isBaseBundle,bool isCompress)
        {
            string[] tempGuids=AssetDatabase.FindAssets(sourceTypeFilter, new string[] {Utility.Path.GetRegularPath(path)});
            for (int i = 0; i < tempGuids.Length; i++)
            {
                string guid = tempGuids[i];
                string fullPath = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.IsValidFolder(fullPath) || !nameInfo.IsMyAsset(fullPath) || IsScript(fullPath))
                {
                    // 文件夹不处理  或者不是我的 由于有可能和孩子的搜索类型不同 不处理
                    continue;
                }

                if (_assetBundleConfig.shaderOnePack && CheckShader(fullPath))
                {
                    AddAssetBundle(guid, fullPath, ShaderBundle, null, true, true);
                    continue;
                }
                string assetBundleName = GetAssetBundleName(fullPath);
                assetBundleName=assetBundleName.Replace(" ", "_");
                assetBundleName = Utility.Path.GetRegularPath(Path.Combine(GetAssetBundleName(path, true), assetBundleName));
                AddAssetBundle(guid, fullPath, assetBundleName, null, isEnterPack, isBaseBundle,isCompress);
            }
        }
        
        /// <summary>
        /// 同一个文件夹下 打成一个包
        /// </summary>
        /// <param name="path"></param>
        private void CollectionItemByOneFolderPack(string path,AssetBundleNameInfo nameInfo,string sourceTypeFilter,bool isEnterPack,bool isBaseBundle,bool isCompress,string assetBundleName = null,string assetBundleVariant = null)
        {
            assetBundleName ??= GetAssetBundleName(path,true);
            assetBundleName=assetBundleName.Replace(" ", "_");
            string[] tempGuids=AssetDatabase.FindAssets(sourceTypeFilter, new string[] {Utility.Path.GetRegularPath(path)});
            for (int i = 0; i < tempGuids.Length; i++)
            {
                string guid = tempGuids[i];
                string fullPath = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.IsValidFolder(fullPath) || !nameInfo.IsMyAsset(fullPath) || IsScript(fullPath))
                {
                    // 文件夹不处理 
                    continue;
                }
                if (_assetBundleConfig.shaderOnePack && CheckShader(fullPath))
                {
                    AddAssetBundle(guid, fullPath, ShaderBundle, null, true, true);
                    continue;
                }
                AddAssetBundle(guid, fullPath, assetBundleName, assetBundleVariant, isEnterPack, isBaseBundle,isCompress);
            }
            
        }
        
        /// <summary>
        /// 文件夹下的 第一层子文件夹为一个包(如果是文件 每个文件打成一个)
        /// </summary>
        /// <param name="path"></param>
        private void CollectionItemByFolderPack(string path,AssetBundleNameInfo nameInfo,string sourceTypeFilter,bool isEnterPack,bool isBaseBundle,bool isCompress)
        {
            string folderName = GetAssetBundleName(path, true);
            DirectoryInfo theFolder = new DirectoryInfo(path);
            DirectoryInfo[] dirInfo = theFolder.GetDirectories();
            List<string> tempDirPath = new List<string>(dirInfo.Length);
            foreach (var dir in dirInfo)
            {
                string assetBundleName = Utility.Path.GetRegularPath(Path.Combine(folderName, dir.Name)).ToLower();
                assetBundleName=assetBundleName.Replace(" ", "_");
                CollectionItemByOneFolderPack(Utility.Path.GetRegularPath(Path.Combine(path,dir.Name)),nameInfo,sourceTypeFilter,isEnterPack,isBaseBundle,isCompress,assetBundleName);
                int startIndex =dir.FullName.IndexOf("Assets/", StringComparison.Ordinal);
                tempDirPath.Add(dir.FullName.Substring(startIndex));
            }
            
            string[] tempGuids=AssetDatabase.FindAssets(sourceTypeFilter, new string[] {Utility.Path.GetRegularPath(path)});
            string fullPath;
            for (int i = 0; i < tempGuids.Length; i++)
            {
                string guid = tempGuids[i];
                fullPath=AssetDatabase.GUIDToAssetPath(guid);
                bool isContinue = false;
                foreach (var dir in tempDirPath)
                {
                    if (fullPath.Contains(dir))
                    {
                        isContinue = true;
                        break;
                    }
                }
                
                if (isContinue || AssetDatabase.IsValidFolder(fullPath) || !nameInfo.IsMyAsset(fullPath) || IsScript(fullPath))
                {
                    // 文件夹不处理 
                    continue;
                }
                if (_assetBundleConfig.shaderOnePack && CheckShader(fullPath))
                {
                    AddAssetBundle(guid, fullPath, ShaderBundle, null, true, true);
                    continue;
                }
                string assetBundleName = GetAssetBundleName(fullPath);
                assetBundleName=assetBundleName.Replace(" ", "_");
                assetBundleName = Utility.Path.GetRegularPath(Path.Combine(GetAssetBundleName(path, true), assetBundleName));
                AddAssetBundle(guid, fullPath, assetBundleName, null, isEnterPack, isBaseBundle,isCompress);
            }
        }

        private void CollectionItemBySomeNameAddVariant(string path,AssetBundleNameInfo nameInfo,string sourceTypeFilter,bool isEnterPack,bool isBaseBundle,bool isCompress)
        {
            string assetBundleName = GetAssetBundleName(path, true);
            DirectoryInfo theFolder = new DirectoryInfo(path);
            DirectoryInfo[] dirInfo = theFolder.GetDirectories();
            foreach (var dir in dirInfo)
            {
                string assetVariant = dir.Name;
                CollectionItemByOneFolderPack(Utility.Path.GetRegularPath(Path.Combine(path,dir.Name)),
                    nameInfo,sourceTypeFilter,isEnterPack,isBaseBundle,isCompress,assetBundleName?.ToLower(),assetVariant?.ToLower());
            }
            
        }
        
        /// <summary>
        /// 冗余资源打包
        /// </summary>
        private void AutoCollectionItemToPack()
        {
            Debug.Log("Auto Collection Item To Pack");
            if (_assetBundleConfig != null && (_assetBundleConfig.shaderOnePack || _assetBundleConfig.autoPack))
            {
                if (m_Assets != null && m_Assets.Count > 0)
                {
                    foreach (var fullPath in m_Assets.Keys)
                    {
                        CheckDependencies(fullPath,m_Assets[fullPath].Resource.Name);
                    }
                }

                if (_redundantData.AllAssetRedundant != null && _redundantData.AllAssetRedundant.Count > 0)
                {
                    foreach (var fullPath in _redundantData.AllAssetRedundant.Keys)
                    {
                        string guid = AssetDatabase.AssetPathToGUID(fullPath);
                        if (_assetBundleConfig.shaderOnePack && CheckShader(fullPath))
                        {
                            AddAssetBundle(guid, fullPath, ShaderBundle, null, true, true);
                            continue;
                        }
                       
                        int startPos = fullPath.LastIndexOf('/',2) + 1;
                        int endPos = fullPath.LastIndexOf('.');
                     
                        string assetBundleName = fullPath.Substring(startPos,endPos-startPos);
                      
                        assetBundleName = Utility.Path.GetRegularPath(assetBundleName);
                        AddAssetBundle(guid, fullPath, assetBundleName, null, true, false);
                    }
                }
            }
            Debug.Log("Auto Collection Item To Pack Over");
        }

        private void CheckDependencies(string fullPath,string assetBundleName)
        {
            string[] paths=AssetDatabase.GetDependencies(fullPath,true);
            for (int i = 0; i < paths.Length; i++)
            {
                if(IsScript(paths[i]))
                    continue;
                if (_assetBundleConfig.shaderOnePack && CheckShader(paths[i]))
                {
                    assetBundleName = ShaderBundle;
                    string guid = AssetDatabase.AssetPathToGUID(paths[i]);
                    AddAssetBundle(guid, fullPath, assetBundleName, null, true, true);
                    continue;
                }

                if (_assetBundleConfig.autoPack && !IsHaveAsset(paths[i]))
                {
                    _redundantData.AddAsset(paths[i],assetBundleName);   
                }
            }
        }
       
        
        private bool CheckShader(string fullName)
        {
            
            return fullName.EndsWith(Shader);
        }

        private bool IsScript(string fullName)
        {
            return fullName.EndsWith(".cs");
        }
        
        private string GetAssetBundleName(string path,bool isFold = false)
        {
            string assetBundleName;
            int startPos = path.LastIndexOf('/') + 1;
            if (isFold)
            {
                assetBundleName = path.Substring(startPos);
            }
            else
            {
                int endPos = path.LastIndexOf('.');
                assetBundleName = path.Substring(startPos,endPos-startPos);
            }
            return assetBundleName.ToLower();

        }

        private void AddAssetBundle(string guid,string fullPath,string assetBundleName,string assetBundleVariant,bool assetBundlePacked,bool baseBundle,bool isCompress=true)
        {
         
            string bundleFullName=GetAssetBundleFullName(assetBundleName, assetBundleVariant);
            if (!m_AssetBundles.ContainsKey(bundleFullName))
            {
                AddAssetBundle(assetBundleName, assetBundleVariant, assetBundlePacked, baseBundle,isCompress);

            }
            AddAsset(guid, assetBundleName, assetBundleVariant, fullPath);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="assetBundleVariant"></param>
        /// <param name="assetBundlePacked">是否放到apk</param>
        /// <param name="basePacked">是否是基础ab</param>
        /// <returns></returns>
        private bool AddAssetBundle(string assetBundleName, string assetBundleVariant,
             bool assetBundlePacked,bool basePacked,bool compress)
        {
            if (!IsValidAssetBundleName(assetBundleName, assetBundleVariant))
            {
                return false;
            }

            if (!IsAvailableBundleName(assetBundleName, assetBundleVariant, null))
            {
                return false;
            }

     
            Resource resource = Resource.Create(assetBundleName, assetBundleVariant, assetBundlePacked,basePacked,compress);
            m_AssetBundles.Add(resource.FullName.ToLower(), resource);       
            
            return true;
        }
        
        private void AddAsset(string guid,string assetBundleName,string assetBundleVariant,string fullPath)
        {
            if (!IsHaveAsset(fullPath))
            {
                string bundleFullName = GetAssetBundleFullName(assetBundleName, assetBundleVariant);
                Asset asset = Asset.Create(guid,
                    m_AssetBundles[bundleFullName]);
                m_AssetBundles[bundleFullName]
                    .AssignAsset(asset, IsScene(fullPath));
                m_Assets.Add(fullPath, asset);
            }
        }
        
        private bool IsValidAssetBundleName(string assetBundleName, string assetBundleVariant)
        {
            if (string.IsNullOrEmpty(assetBundleName))
            {
                return false;
            }

            if (!AssetBundleNameRegex.IsMatch(assetBundleName))
            {
                Debug.LogErrorFormat("{0}=====is match error",assetBundleName);
                return false;
            }

            if (assetBundleVariant != null && !AssetBundleVariantRegex.IsMatch(assetBundleVariant))
                return false;
            return true;
        }
        
        private bool IsAvailableBundleName(string assetBundleName, string assetBundleVariant,
            Resource selfResource)
        {

            Resource findResource = GetAssetBundle(assetBundleName, assetBundleVariant);
            if (findResource != null)
                return findResource == selfResource;
            foreach (Resource assetBundle in m_AssetBundles.Values)
            {
                if (selfResource != null && assetBundle == selfResource)
                {
                    continue;
                }

                if (assetBundle.Name == assetBundleName)
                {
                    if (assetBundle.Variant == null && assetBundleVariant != null)
                    {
                        return false;
                    }

                    if (assetBundle.Variant != null && assetBundleVariant == null)
                    {
                        return false;
                    }
                }

                if (assetBundle.Name.Length > assetBundleName.Length &&
                    assetBundle.Name.IndexOf(assetBundleName, StringComparison.CurrentCultureIgnoreCase) == 0 &&
                    assetBundle.Name[assetBundleName.Length]=='/')
                {
                    return false;
                }
                
                if (assetBundleName.Length > assetBundle.Name.Length
                    && assetBundleName.IndexOf(assetBundle.Name, StringComparison.CurrentCultureIgnoreCase) == 0
                    && assetBundleName[assetBundle.Name.Length] == '/')
                {
                    return false;
                }
                
            }
            return true;
        }

        private bool IsHaveAsset(string fullPath)
        {
            if (m_Assets.ContainsKey(fullPath))
            {
                return true;
            }

            return false;
        }
        
        private string GetAssetBundleFullName(string assetBundleName, string assetBundleVariant)
        {
            return (!string.IsNullOrEmpty(assetBundleVariant)
                ? Utility.Text.Format("{0}.{1}", assetBundleName, assetBundleVariant)
                : assetBundleName).ToLower();
        }
        
        public Resource GetAssetBundle(string assetBundleName, string assetBundleVariant)
        {
            if (!IsValidAssetBundleName(assetBundleName, assetBundleVariant))
                return null;
            Resource resource = null;
            if (m_AssetBundles.TryGetValue(GetAssetBundleFullName(assetBundleName, assetBundleVariant).ToLower(),
                out resource))
                return resource;
            return null;
        }

        public Resource[] GetAssetBundles()
        {
            return m_AssetBundles.Values.ToArray();
        }
        
        public Asset[] GetAssets()
        {
            return m_Assets.Values.ToArray();
        }
        
        private bool IsScene(string assetName)
        {
            return assetName.EndsWith(PostfixOfScene);
        }

        public Asset GetAsset(string path)
        {
            if (m_Assets.ContainsKey(path))
                return m_Assets[path];
            return null;
        }

        private void PrintLog()
        {
            if (_allAssetBundleInfos != null)
            {
                foreach (var bundleInfo in _allAssetBundleInfos)
                {
                    bundleInfo.PrintLog();
                }
            }
        }
    }
}