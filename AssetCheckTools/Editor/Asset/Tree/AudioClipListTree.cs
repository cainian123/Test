using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AssetCheckTools.Editor.Asset.Data;
using AssetCheckTools.Editor.Asset.ItemTreeView;
using AssetCheckTools.Editor.Asset.View;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetCheckTools.Editor.Asset.Tree
{
    public class AudioClipListTree:AssetListBaseTree
    {
        internal enum SortOption
        {
            Asset,
            Length,
            LoadType,
            Format,
            Size,
            ForceToMono
        }
        SortOption[] m_SortOptions =
        {
            SortOption.Asset,
            SortOption.Length,
            SortOption.LoadType,
            SortOption.Format,
            SortOption.Size,
            SortOption.ForceToMono
        };
        
        private List<AudioInfo> m_SourceBundles = new List<AudioInfo>();
        public AudioClipListTree(TreeViewState state, BaseTab ctrl) : base(state, ctrl)
        {
            m_SourceBundles = new List<AudioInfo>();
            string[] paths = FindAsset("t:AudioClip",new string[]{"Assets"});
            foreach (var path in paths)
            {
                AudioInfo info = new AudioInfo(path);
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
            };
            
            retVal[0].headerContent = new GUIContent("Asset", "资源名称");
            retVal[0].minWidth = 200;
            retVal[0].width = 250;
            retVal[0].maxWidth = 300;
            retVal[0].headerTextAlignment = TextAlignment.Center;
            retVal[0].canSort = false;
            retVal[0].autoResize = true;
            
            retVal[1].headerContent = new GUIContent("Length", "音效时长");
            retVal[1].minWidth = 80;
            retVal[1].width = 100;
            retVal[1].maxWidth = 120;
            retVal[1].headerTextAlignment = TextAlignment.Center;
            retVal[1].canSort = true;
            retVal[1].autoResize = true;
            
            retVal[2].headerContent = new GUIContent("LoadType", "加载方式");
            retVal[2].minWidth = 120;
            retVal[2].width = 150;
            retVal[2].maxWidth = 180;
            retVal[2].headerTextAlignment = TextAlignment.Center;
            retVal[2].canSort = false;
            retVal[2].autoResize = true;
            
            retVal[3].headerContent = new GUIContent("Format", "音效格式");
            retVal[3].minWidth = 80;
            retVal[3].width = 100;
            retVal[3].maxWidth = 120;
            retVal[3].headerTextAlignment = TextAlignment.Center;
            retVal[3].canSort = false;
            retVal[3].autoResize = true;
            
            retVal[4].headerContent = new GUIContent("Size", "硬盘上大小");
            retVal[4].minWidth = 80;
            retVal[4].width = 100;
            retVal[4].maxWidth = 120;
            retVal[4].headerTextAlignment = TextAlignment.Center;
            retVal[4].canSort = true;
            retVal[4].autoResize = true;
            
            retVal[5].headerContent = new GUIContent("ForceToMono", "是否开启Force To Mono");
            retVal[5].minWidth = 80;
            retVal[5].width = 100;
            retVal[5].maxWidth =120;
            retVal[5].headerTextAlignment = TextAlignment.Center;
            retVal[5].canSort = true;
            retVal[5].autoResize = true;

            return retVal;
        }

        protected override TreeViewItem GetBuildRoot()
        {
            var root = new TreeViewBaseItem();

            foreach (var info in m_SourceBundles)
            {
                root.AddChild(new AudioClipTreeViewITem(info));
            }
            return root;
        }

        protected override void CellGUI(Rect cellRect, TreeViewItem item, int column, ref RowGUIArgs args)
        {
            AudioClipTreeViewITem viewItem = item as AudioClipTreeViewITem;
            AudioInfo info = viewItem?.asset as AudioInfo;
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
                    if (viewItem.icon != null)
                        GUI.DrawTexture(iconRect, viewItem.icon, ScaleMode.ScaleToFit);
                    DefaultGUI.Label(
                        new Rect(cellRect.x + iconRect.xMax + 1, cellRect.y, cellRect.width - iconRect.width,
                            cellRect.height),
                        viewItem.displayName,
                        args.selected,
                        args.focused);
                }
                    break;
                case SortOption.Length:
                    DefaultGUI.Label(cellRect, info.Length.ToString(CultureInfo.InvariantCulture), args.selected,
                        args.focused);
                    break;
                case SortOption.LoadType:
                    DefaultGUI.Label(cellRect, info.LoadType.ToString(), args.selected, args.focused);
                    break;
                case SortOption.Format:
                    DefaultGUI.Label(cellRect, info.Format, args.selected, args.focused);
                    break;
                case SortOption.Size:
                    DefaultGUI.Label(cellRect,info.GetSizeString(),args.selected,args.focused);
                    break;
                case SortOption.ForceToMono:
                    GUI.enabled = false;
                    EditorGUI.Toggle(cellRect, info.ForceToMono);
                    GUI.enabled = true;
                    break;
            }
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

            List<AudioClipTreeViewITem> assetList = new List<AudioClipTreeViewITem>();
            foreach(var item in rootItem.children)
            {
                assetList.Add(item as AudioClipTreeViewITem);
            }
            var orderedItems = InitialOrder(assetList, sortedColumns);

            rootItem.children = orderedItems.Cast<TreeViewItem>().ToList();
        }
        
        IOrderedEnumerable<AudioClipTreeViewITem> InitialOrder(IEnumerable<AudioClipTreeViewITem> myTypes, int[] columnList)
        {
            SortOption sortOption = m_SortOptions[columnList[0]];
            bool ascending = multiColumnHeader.IsSortedAscending(columnList[0]);
            switch (sortOption)
            {
                case SortOption.Asset:
                    return myTypes.Order(l => l.displayName, ascending);
                case SortOption.Size:
                   return myTypes.Order(l => l.asset.fileSize, ascending);
                case SortOption.Length:
                    return myTypes.Order(l =>
                    {
                        if (l.asset is AudioInfo info)
                            return info.Length;
                        return 0;
                    } , ascending);
                case SortOption.ForceToMono:
                    return myTypes.Order(l =>
                    {
                        if (l.asset is AudioInfo info)
                            return info.ForceToMono ? 1 : 0;
                        return 0;
                    } , ascending);
                default:
                    return myTypes.Order(l => l.displayName, ascending);
            }
            
        }
    }
}