using System.Collections.Generic;
using System.Linq;
using AssetCheckTools.Editor.Asset.Data;
using AssetCheckTools.Editor.Asset.ItemTreeView;
using AssetCheckTools.Editor.Asset.View;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetCheckTools.Editor.Asset.Tree
{
    public class MeshListTree : AssetListBaseTree
    {
        internal enum SortOption
        {
            Asset,
            Vertex,
            Triangles,
            Normal,
            Colors,
            Tangents,
            Size,
            ReadWrite
        }
        SortOption[] m_SortOptions =
        {
            SortOption.Asset,
            SortOption.Vertex,
            SortOption.Triangles,
            SortOption.Normal,
            SortOption.Colors,
            SortOption.Tangents,
            SortOption.Size,
            SortOption.ReadWrite
        };
        
        private List<MeshInfo> m_SourceBundles = new List<MeshInfo>();
        public MeshListTree(TreeViewState state, BaseTab ctrl) : base(state, ctrl)
        {
            m_SourceBundles = new List<MeshInfo>();
            string[] paths = FindAsset("t:mesh",new string[]{"Assets"});
            foreach (var path in paths)
            {
                MeshInfo info = new MeshInfo(path);
                m_SourceBundles.Add(info);
            }
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
                new MultiColumnHeaderState.Column(),
            };
            
            retVal[0].headerContent = new GUIContent("Asset", "资源名称");
            retVal[0].minWidth = 200;
            retVal[0].width = 250;
            retVal[0].maxWidth = 300;
            retVal[0].headerTextAlignment = TextAlignment.Center;
            retVal[0].canSort = false;
            retVal[0].autoResize = true;
            
            retVal[1].headerContent = new GUIContent("Vertex", "顶点数量");
            retVal[1].minWidth = 80;
            retVal[1].width = 100;
            retVal[1].maxWidth = 120;
            retVal[1].headerTextAlignment = TextAlignment.Center;
            retVal[1].canSort = true;
            retVal[1].autoResize = true;
            
            retVal[2].headerContent = new GUIContent("Triangles", "三角面数量");
            retVal[2].minWidth = 80;
            retVal[2].width = 100;
            retVal[2].maxWidth = 120;
            retVal[2].headerTextAlignment = TextAlignment.Center;
            retVal[2].canSort = true;
            retVal[2].autoResize = true;
            
            retVal[3].headerContent = new GUIContent("Normal", "法线数量");
            retVal[3].minWidth = 80;
            retVal[3].width = 100;
            retVal[3].maxWidth = 120;
            retVal[3].headerTextAlignment = TextAlignment.Center;
            retVal[3].canSort = true;
            retVal[3].autoResize = true;
            
            retVal[4].headerContent = new GUIContent("Colors", "顶点颜色数量");
            retVal[4].minWidth = 80;
            retVal[4].width = 100;
            retVal[4].maxWidth = 120;
            retVal[4].headerTextAlignment = TextAlignment.Center;
            retVal[4].canSort = true;
            retVal[4].autoResize = true;
            
            retVal[5].headerContent = new GUIContent("Tangents", "切线数量");
            retVal[5].minWidth = 80;
            retVal[5].width = 100;
            retVal[5].maxWidth =120;
            retVal[5].headerTextAlignment = TextAlignment.Center;
            retVal[5].canSort = true;
            retVal[5].autoResize = true;
            
            retVal[6].headerContent = new GUIContent("Size", "在硬盘上大小");
            retVal[6].minWidth = 80;
            retVal[6].width = 100;
            retVal[6].maxWidth =120;
            retVal[6].headerTextAlignment = TextAlignment.Center;
            retVal[6].canSort = true;
            retVal[6].autoResize = true;
            
            retVal[7].headerContent = new GUIContent("Read/Write", "是否开启Read/Write");
            retVal[7].minWidth = 60;
            retVal[7].width = 80;
            retVal[7].maxWidth =100;
            retVal[7].headerTextAlignment = TextAlignment.Center;
            retVal[7].canSort = true;
            retVal[7].autoResize = true;
            
            return retVal;
        }

        protected override TreeViewItem GetBuildRoot()
        {
            var root = new TreeViewBaseItem();

            foreach (var info in m_SourceBundles)
            {
                root.AddChild(new MeshTreeViewItem(info));
            }
            return root;
        }

        protected override void CellGUI(Rect cellRect, TreeViewItem item, int column, ref RowGUIArgs args)
        {
            MeshTreeViewItem viewItem = item as MeshTreeViewItem;
            MeshInfo info = viewItem?.asset as MeshInfo;
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
                case SortOption.Vertex:
                    DefaultGUI.Label(cellRect, info.Vertex.ToString(), args.selected, args.focused);
                    break;
                case SortOption.Triangles:
                    DefaultGUI.Label(cellRect,info.Triangles.ToString(),args.selected,args.focused);
                    break;
                case SortOption.Normal:
                    DefaultGUI.Label(cellRect,info.Normal.ToString(),args.selected,args.focused);
                    break;
                case SortOption.Colors:
                    DefaultGUI.Label(cellRect,info.Colors.ToString(),args.selected,args.focused);
                    break;
                case SortOption.Tangents:
                    DefaultGUI.Label(cellRect,info.Tangents.ToString(),args.selected,args.focused);
                    break;
                case SortOption.Size:
                    DefaultGUI.Label(cellRect,info.GetSizeString(),args.selected,args.focused);
                    break;
                case SortOption.ReadWrite:
                    GUI.enabled = false;
                    EditorGUI.Toggle(cellRect, info.IsOpenRead);
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

            List<MeshTreeViewItem> assetList = new List<MeshTreeViewItem>();
            foreach(var item in rootItem.children)
            {
                assetList.Add(item as MeshTreeViewItem);
            }
            var orderedItems = InitialOrder(assetList, sortedColumns);

            rootItem.children = orderedItems.Cast<TreeViewItem>().ToList();
        }
        
        IOrderedEnumerable<MeshTreeViewItem> InitialOrder(IEnumerable<MeshTreeViewItem> myTypes, int[] columnList)
        {
            SortOption sortOption = m_SortOptions[columnList[0]];
            bool ascending = multiColumnHeader.IsSortedAscending(columnList[0]);
            switch (sortOption)
            {
                case SortOption.Asset:
                    return myTypes.Order(l => l.displayName, ascending);
                case SortOption.Size:
                   return myTypes.Order(l => l.asset.fileSize, ascending);
                case SortOption.Vertex:
                    return myTypes.Order(l =>
                    {
                        if (l.asset is MeshInfo info)
                            return info.Vertex;
                        return 0;
                    } , ascending);
                case SortOption.Triangles:
                    return myTypes.Order(l =>
                    {
                        if (l.asset is MeshInfo info)
                            return info.Triangles;
                        return 0;
                    } , ascending);
                case SortOption.Normal:
                    return myTypes.Order(l =>
                    {
                        if (l.asset is MeshInfo info)
                            return info.Normal;
                        return 0;
                    } , ascending);
                case SortOption.Colors:
                    return myTypes.Order(l =>
                    {
                        if (l.asset is MeshInfo info)
                            return info.Colors;
                        return 0;
                    } , ascending);
                case SortOption.Tangents:
                    return myTypes.Order(l =>
                    {
                        if (l.asset is MeshInfo info)
                            return info.Tangents;
                        return 0;
                    } , ascending);
                case SortOption.ReadWrite:
                    return myTypes.Order(l =>
                    {
                        if (l.asset is MeshInfo info)
                            return info.IsOpenRead ? 1 : 0;
                        return 0;
                    } , ascending);
                default:
                    return myTypes.Order(l => l.displayName, ascending);
            }
            
        }
    }
}