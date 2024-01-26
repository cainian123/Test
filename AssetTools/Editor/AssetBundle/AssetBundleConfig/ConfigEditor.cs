using System;
using System.Collections.Generic;
using AssetTools.Editor.AssetBundle.AssetBundleAnalyzer;
using AssetTools.Editor.AssetBundle.AssetBundleCollection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using AssetTools.Editor.Util;


namespace AssetTools.Editor.AssetBundle.AssetBundleConfig
{
    [CustomEditor(typeof(AssetBundleConfig))]
    public class ConfigEditor : UnityEditor.Editor
    {
        private ReorderableList _configArray;
        private void OnEnable()
        {

            _configArray = new ReorderableList(serializedObject, serializedObject.FindProperty("configData"),
                true, true, true, true) {drawHeaderCallback = rect => { GUI.Label(rect, "ABName Config"); }};


            _configArray.drawElementCallback = (rect, index, active, focused) =>
            {
                SerializedProperty item = _configArray.serializedProperty.GetArrayElementAtIndex(index);
                rect.height -= 4;
                rect.y += 2;
                EditorGUI.PropertyField(rect, item, new GUIContent("Index " + index));
            };
            
            _configArray.onRemoveCallback = delegate(ReorderableList list) {
                if (EditorUtility.DisplayDialog("警告", "确定要删除?", "是", "否"))
                {
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                }
            };
            
            _configArray.onChangedCallback = delegate(ReorderableList list) {
                
            };

            _configArray.elementHeight = 90;
        }

        public override void OnInspectorGUI()
        {
            GUI.color = Color.red;
            GUILayout.Box("场景不能和Prefab打在一起");
            GUI.color = Color.white;
            AssetBundleConfig ctr = target as AssetBundleConfig;
            ctr.shaderOnePack=GUILayout.Toggle(ctr.shaderOnePack,"所有shader打成一个包");
            GUILayout.Space(5f);
            ctr.autoPack = GUILayout.Toggle(ctr.autoPack, "冗余资源是否自动打包");
            GUILayout.Space(10f);
            serializedObject.Update();
            _configArray.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            GUILayout.Space(10f);
            
            if (GUILayout.Button("设置AB"))
            {
                CollectionBundle();
            }

            GUILayout.Space(10f);
            
            if (GUILayout.Button("资源分析"))
            {
                Analyzer();
            }
            
            GUILayout.Space(10f);
            
            if (GUILayout.Button("Build"))
            {
               TestAnalyzer();
            }
        }

        private void CollectionBundle()
        {
            string path=AssetDatabase.GetAssetPath(target);
            AssetBundleCollection.AssetBundleCollection collection = new AssetBundleCollection.AssetBundleCollection(path);
            collection.Load();
            Asset[] allAssets=collection.GetAssets();
            for (int i = 0; i < allAssets.Length; i++)
            {
                AssetImporter importer = AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(allAssets[i].Guid));
                if (importer != null)
                {
                    var assetBundle = allAssets[i].Resource;
                    if (assetBundle != null)
                    {
                        importer.assetBundleName = allAssets[i].Resource?.Name;
                        if (assetBundle.Variant != null)
                            importer.assetBundleVariant = assetBundle.Variant;
                    }
                }
                
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void Analyzer()
        {
            AssetBundleAnalyzerController analyzerController = new AssetBundleAnalyzerController(AssetDatabase.GetAssetPath(target));
            analyzerController.StartAnalyzer();
            string path = Application.dataPath;
            int endPos=path.LastIndexOf("/", StringComparison.Ordinal);
            path = path.Substring(0, endPos+1);
            path = Utility.Path.GetCombinePath(path, "AnalyzerReport");
            analyzerController.PrintAnalyzer(path);
        }

        private void TestAnalyzer()
        {
            try
            {
                List<Asset> allAssets = new List<Asset>();
                SortedDictionary<string, Resource> assetBundles = new SortedDictionary<string, Resource>();
                string[] allAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();
                int i = 0;
                foreach (var assetBundleName in allAssetBundleNames)
                {
                    i++;
                    EditorUtility.DisplayProgressBar("Analyzer AssetBundle", Utility.Text.Format("Analyzing Asset Info, {0}/{1}", i, allAssetBundleNames.Length), (float)i / allAssetBundleNames.Length);

                    Resource resource = Resource.Create(assetBundleName,String.Empty, false,false,false);
                    string[] assetPaths=AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
                    foreach (var path in assetPaths)
                    {
                    
                        Asset asset = Asset.Create(AssetDatabase.AssetPathToGUID(path));
                        allAssets.Add(asset);
                        resource.AssignAsset(asset,IsScene(path));
                    }
                    assetBundles.Add(assetBundleName,resource);
               
                }
            
                AssetBundleAnalyzerController analyzerController = new AssetBundleAnalyzerController(allAssets.ToArray(),assetBundles);
                string pritePath = Application.dataPath;
                int endPos=pritePath.LastIndexOf("/", StringComparison.Ordinal);
                pritePath = pritePath.Substring(0, endPos+1);
          
                pritePath = Utility.Path.GetCombinePath(pritePath, "AnalyzerReport");
                analyzerController.PrintAnalyzer(pritePath);
            }
            catch (Exception e)
            { 
                Debug.Log(e.StackTrace);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
           
            
        }
        
        private bool IsScene(string assetName)
        {
            return assetName.EndsWith(".unity");
        }
    }
}