using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using System.Linq;
using AssetCheckTools.Editor.Asset.Data;
using AssetCheckTools.Editor.Asset.ItemTreeView;
using AssetCheckTools.Editor.Asset.View;

namespace AssetCheckTools.Editor.Asset.Tree
{
    public class TextureListTree : AssetListBaseTree
    {
        internal enum SortOption
        {
            Asset,
            WidthAndHeight,
            AndroidFormat,
            IosFormat,
            Size,
            MipMap,
            ReadWrite
        }
        SortOption[] m_SortOptions =
        {
            SortOption.Asset,
            SortOption.WidthAndHeight,
            SortOption.AndroidFormat,
            SortOption.IosFormat,
            SortOption.Size,
            SortOption.MipMap,
            SortOption.ReadWrite
        };
        
        private List<TextureInfo> m_SourceBundles = new List<TextureInfo>();
        internal TextureListTree(TreeViewState state, BaseTab ctrl) : base(state,ctrl)
        {
            m_SourceBundles.Clear();
            string[] paths = FindAsset("t:texture",new string[]{"Assets"});
            foreach (var guid in paths)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if(!CanCheck(path))
                    continue;
                TextureInfo info = new TextureInfo(path);
                m_SourceBundles.Add(info);
            }
        }

        private bool CanCheck(string path)
        {
            if (path.Contains("Dragon/Common/skybox") || path.Contains("Resource/AtlasImage") ||
                path.Contains("_Script/KingdomMap2")
                || path.Contains("Plugins/Editor") || path.Contains("__RawArtData/_Slice") ||
                path.Contains("__RawArtData/AtlasSource")
                || path.Contains("__RawArtData/_Resources/TextAsset")
                || path.Contains("_Script/UI/UIOptimize")
                || path.Contains("_Script/Watchtower")
                || path.Contains("_UIData/FXUISlice")
                || path.Contains("Assets/Editor")
                || path.Contains("_Script/PVP")
                || path.Contains("_Script/Anniversary")
                || path.Contains("__RawUIDateNew/NewAtlasSource") 
                || path.Contains("__RawArtData/_Resources/Prefab")
                || path.Contains("__UIData/_Resources/Prefab")
                || path.Contains("ReflectionProbe-")
                || path.Contains("Lightmap-")
                || path.Contains("__RawArtData/NewAtlasSource/Atlas_Dragon/DrangonAtlas")
                || path.Contains("__RawArtData/NewAtlasSource/Atlas_Dragon/giantdragon_stone")
                || path.Contains("Assets/Plugins")
                || path.Contains("Assets/Gizmos")
                || path.Contains("Assets/DeltaDNA")
                || path.Contains("Assets/Helpshift")
                || path.Contains("Assets/AssetBundles-Browser")
                || path.Contains("Assets/AppsFlyer")
                || path.Contains("GUI/TrainTroop/Resource")
                || path.Contains("__RawArtData/_UnitySlice")
                || path.Contains("__ArtData/Atlas")
                || path.Contains("__RawArtData/_UnitySlice/City/animation"))
                return false;
            return true;

        }

        protected override MultiColumnHeaderState.Column[] GetColumns()
        {
            var retVal = new MultiColumnHeaderState.Column[] {
                new MultiColumnHeaderState.Column(),
                new MultiColumnHeaderState.Column(),
                new MultiColumnHeaderState.Column(),
                new MultiColumnHeaderState.Column(),
                new MultiColumnHeaderState.Column(),
                new MultiColumnHeaderState.Column(),
                new MultiColumnHeaderState.Column(),
            };
            retVal[0].headerContent = new GUIContent("Asset", "资源名称");
            retVal[0].minWidth = 100;
            retVal[0].width = 200;
            retVal[0].maxWidth = 300;
            retVal[0].headerTextAlignment = TextAlignment.Center;
            retVal[0].canSort = false;
            retVal[0].autoResize = true;
            
            retVal[1].headerContent = new GUIContent("Width x Height", "宽度 x 高度");
            retVal[1].minWidth = 100;
            retVal[1].width = 150;
            retVal[1].maxWidth = 200;
            retVal[1].headerTextAlignment = TextAlignment.Center;
            retVal[1].canSort = true;
            retVal[1].autoResize = true;
            
            retVal[2].headerContent = new GUIContent("AndroidFormat", "安卓压缩格式");
            retVal[2].minWidth = 150;
            retVal[2].width = 180;
            retVal[2].maxWidth = 210;
            retVal[2].headerTextAlignment = TextAlignment.Center;
            retVal[2].canSort = false;
            retVal[2].autoResize = true;
            
            retVal[3].headerContent = new GUIContent("IosFormat", "iphone压缩格式");
            retVal[3].minWidth = 150;
            retVal[3].width = 180;
            retVal[3].maxWidth = 210;
            retVal[3].headerTextAlignment = TextAlignment.Center;
            retVal[3].canSort = false;
            retVal[3].autoResize = true;
            
            retVal[4].headerContent = new GUIContent("size", "在硬盘上大小");
            retVal[4].minWidth = 60;
            retVal[4].width = 100;
            retVal[4].maxWidth = 120;
            retVal[4].headerTextAlignment = TextAlignment.Center;
            retVal[4].canSort = true;
            retVal[4].autoResize = true;
            
            retVal[5].headerContent = new GUIContent("MipMap", "是否开启mipmap");
            retVal[5].minWidth = 50;
            retVal[5].width = 60;
            retVal[5].maxWidth =80;
            retVal[5].headerTextAlignment = TextAlignment.Center;
            retVal[5].canSort = true;
            retVal[5].autoResize = true;
            
            retVal[6].headerContent = new GUIContent("Read/Write", "是否开启Read/Write");
            retVal[6].minWidth = 60;
            retVal[6].width = 80;
            retVal[6].maxWidth =100;
            retVal[6].headerTextAlignment = TextAlignment.Center;
            retVal[6].canSort = true;
            retVal[6].autoResize = true;
            return retVal;
        }

        protected override TreeViewItem GetBuildRoot()
        {
             var root = new TreeViewBaseItem();

             foreach (var info in m_SourceBundles)
             {
                root.AddChild(new TextureTreeViewItem(info));
             }
             return root;
        }

        protected override void CellGUI(Rect cellRect, TreeViewItem item, int column, ref RowGUIArgs args)
        {
            TextureTreeViewItem viewItem = item as TextureTreeViewItem;
            TextureInfo info = viewItem?.asset as TextureInfo;
            if(info == null)
                return;
            Color oldColor = GUI.color;
            CenterRectUsingSingleLineHeight(ref cellRect);
            
            GUI.color = viewItem.GetColor(m_SortOptions[column]);

            switch (m_SortOptions[column])
            {
                case SortOption.Asset:
                {
                    var iconRect = new Rect(cellRect.x + 1, cellRect.y + 1, cellRect.height - 2, cellRect.height - 2);
                    if(viewItem.icon != null)
                        GUI.DrawTexture(iconRect, viewItem.icon, ScaleMode.ScaleToFit);
                    DefaultGUI.Label(
                        new Rect(cellRect.x + iconRect.xMax + 1, cellRect.y, cellRect.width - iconRect.width, cellRect.height), 
                        viewItem.displayName, 
                        args.selected, 
                        args.focused);
                }
                    break;
                case SortOption.WidthAndHeight:
                    DefaultGUI.Label(cellRect, info.GetSize(), args.selected, args.focused);
                    break;
                case SortOption.AndroidFormat:
                    DefaultGUI.Label(cellRect,info.AndroidFormat,args.selected,args.focused);
                    break;
                case SortOption.IosFormat:
                    DefaultGUI.Label(cellRect,info.IosFormat,args.selected,args.focused);
                    break;
                case SortOption.Size:
                    DefaultGUI.Label(cellRect,info.GetSizeString(),args.selected,args.focused);
                    break;
                case SortOption.MipMap:
                    GUI.enabled = false;
                    Rect toggleRect = cellRect;
                    EditorGUI.Toggle(toggleRect, info.IsOpenMipMap);
                    GUI.enabled = true;
                    break;
                case SortOption.ReadWrite:
                    GUI.enabled = false;
                    EditorGUI.Toggle(cellRect, info.IsReadWrite);
                    GUI.enabled = true;
                    break;
                    
            }
            GUI.color = oldColor;
        }
        
        protected override void SortIfNeeded(TreeViewItem root, IList<TreeViewItem> rows)
        {
            base.SortIfNeeded(root, rows);
            if (rows.Count <= 1)
                return;

            if (multiColumnHeader.sortedColumnIndex == -1)
                return;

            SortByColumn();

            rows.Clear();
            for (int i = 0; i < root.children.Count; i++)
                rows.Add(root.children[i]);

            Repaint();
        }
        
        void SortByColumn()
        {
            var sortedColumns = multiColumnHeader.state.sortedColumns;

            if (sortedColumns.Length == 0)
                return;

            List<TextureTreeViewItem> assetList = new List<TextureTreeViewItem>();
            foreach(var item in rootItem.children)
            {
                assetList.Add(item as TextureTreeViewItem);
            }
            var orderedItems = InitialOrder(assetList, sortedColumns);

            rootItem.children = orderedItems.Cast<TreeViewItem>().ToList();
        }
        
        IOrderedEnumerable<TextureTreeViewItem> InitialOrder(IEnumerable<TextureTreeViewItem> myTypes, int[] columnList)
        {
            SortOption sortOption = m_SortOptions[columnList[0]];
            bool ascending = multiColumnHeader.IsSortedAscending(columnList[0]);
            switch (sortOption)
            {
                case SortOption.Asset:
                    return myTypes.Order(l => l.displayName, ascending);
                case SortOption.Size:
                   return myTypes.Order(l => l.asset.fileSize, ascending);
                case SortOption.MipMap:
                    return myTypes.Order(l =>
                    {
                        if (l.asset is TextureInfo info)
                            return info.IsOpenMipMap ? 1 : 0;
                        return 0;
                    } , ascending);
                case SortOption.ReadWrite:
                    return myTypes.Order(l =>
                    {
                        if (l.asset is TextureInfo info)
                            return info.IsReadWrite ? 1 : 0;
                        return 0;
                    } , ascending);
                case SortOption.WidthAndHeight:
                    return myTypes.Order(l =>
                    {
                        if (l.asset is TextureInfo info)
                            return info.Width % 4 == 0 && info.Height % 4 == 0 ? 0 : 1;
                        return 0;
                    } , ascending);
                default:
                    return myTypes.Order(l => l.displayName, ascending);
            }
            
        }
    }
}