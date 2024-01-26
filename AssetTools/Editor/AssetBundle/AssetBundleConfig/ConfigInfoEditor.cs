using System;
using System.IO;
using AssetTools.Editor.Util;
using UnityEditor;
using UnityEngine;

namespace AssetTools.Editor.AssetBundle.AssetBundleConfig
{
    [CustomPropertyDrawer(typeof(AssetBundleConfigData))]
    public class ConfigInfoEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                EditorGUIUtility.labelWidth = 60;
                position.height = EditorGUIUtility.singleLineHeight;
                
                var folderPathRect = new Rect(position)
                {
                    width    =  40,
                };
                
                var disNameRect = new Rect(position)
                {
                    x = position.x + folderPathRect.width,
                    width = 350,
                };
                
                var folderBtnRect = new Rect(position)
                {
                    x = position.x + folderPathRect.width + disNameRect.width +5,
                    y = folderPathRect.y ,
                    width = 20,
                };
                
                var packTypeRect = new Rect(position)
                {
                    y = folderPathRect.y + folderPathRect.height +10,
                    width = 230
                };
                
                var streamAssetRect = new Rect(position)
                {
                    x= packTypeRect.x + packTypeRect.width +10,
                    y= packTypeRect.y,
                    width = 50,
                };
                
                var baseBundleRect = new Rect(position)
                {
                    x= streamAssetRect.x + streamAssetRect.width +40,
                    y= packTypeRect.y,
                    width = 80,
                };
                
                var sourceFilterRect = new Rect(position)
                {
                    y= packTypeRect.y + packTypeRect.height+10,
                    width = 300,
                };
                
                var compressRect = new Rect(sourceFilterRect)
                {
                    x = sourceFilterRect.width + sourceFilterRect.x+30,
                    width = 80,
                };
               
                var disName = property.FindPropertyRelative("disName");
                var path = property.FindPropertyRelative("path");
                var packType = property.FindPropertyRelative("packType");
                var sourceAssetUnionTypeFilter = property.FindPropertyRelative("sourceAssetUnionTypeFilter");
                var isStreamAsset = property.FindPropertyRelative("isStreamAsset");
                var isBaseBundle = property.FindPropertyRelative("isBaseBundle");
                var isComp = property.FindPropertyRelative("isCompress");
                
                EditorGUI.LabelField(folderPathRect,"Path :");
                EditorGUI.BeginDisabledGroup(true);
                string showPath;
                if (string.IsNullOrEmpty(disName.stringValue))
                {
                    showPath = "select file path";
                }
                else
                {
                    showPath = disName.stringValue;
                }
                EditorGUI.TextField(disNameRect, "",showPath);
                EditorGUI.EndDisabledGroup();
                
                if(GUI.Button(folderBtnRect,EditorGUIUtility.IconContent("Folder Icon")))
                {
                    string dirPath=Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "_Resource"));
                    string selectPath = EditorUtility.OpenFolderPanel("Select Directory", dirPath, "");
                    if (!string.Equals(dirPath, selectPath) && selectPath.Contains(dirPath))
                    {
                        int start=selectPath.IndexOf("Assets/", StringComparison.Ordinal);
                        path.stringValue=selectPath.Substring(start);
                        start=path.stringValue.IndexOf("_Resource/", StringComparison.Ordinal);
                        disName.stringValue = path.stringValue.Substring(start);
                        
                    }
                }

                packType.enumValueIndex=(int)(AssetBundlePackType)EditorGUI.EnumPopup(packTypeRect,"打包类型 :", (AssetBundlePackType)packType.enumValueIndex);

               isStreamAsset.boolValue=EditorGUI.Toggle(streamAssetRect, "是否进包:",isStreamAsset.boolValue);

               isBaseBundle.boolValue = EditorGUI.Toggle(baseBundleRect, "基础资源:",isBaseBundle.boolValue);

               sourceAssetUnionTypeFilter.stringValue =
                   EditorGUI.TextField(sourceFilterRect, "搜索类型 :", sourceAssetUnionTypeFilter.stringValue);

               isComp.boolValue = EditorGUI.Toggle(compressRect, "是否压缩:",isComp.boolValue);
            }
        }
    }
}