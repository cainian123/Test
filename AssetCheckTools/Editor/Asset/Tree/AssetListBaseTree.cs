using System.Collections.Generic;
using System.Linq;
using AssetCheckTools.Editor.Asset.ItemTreeView;
using AssetCheckTools.Editor.Asset.View;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using UnityEngine;

namespace AssetCheckTools.Editor.Asset.Tree
{
    public abstract class AssetListBaseTree : TreeView
    {
        protected BaseTab m_baseTab;
        internal AssetListBaseTree(TreeViewState state, BaseTab ctrl) : base(state)
        {
            m_baseTab = ctrl;
            showBorder = true;
            showAlternatingRowBackgrounds = true;
        }

        void OnSortingChanged(MultiColumnHeader multiColumnHeader)
        {
            SortIfNeeded(rootItem, GetRows());
        }

        protected virtual void SortIfNeeded(TreeViewItem root, IList<TreeViewItem> rows)
        {
            
        }

        public void InitMultiColumnHeaderState(MultiColumnHeader header)
        {
            multiColumnHeader = header;
            multiColumnHeader.sortingChanged += OnSortingChanged;
        }

        internal  MultiColumnHeaderState CreateDefaultMultiColumnHeaderState()
        {
            return new MultiColumnHeaderState(GetColumns());
        }

        protected  abstract MultiColumnHeaderState.Column[] GetColumns();
        
        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
            {
                SetSelection(new int[0], TreeViewSelectionOptions.FireSelectionChanged);
            }
        }
        
        protected override TreeViewItem BuildRoot()
        {
            return GetBuildRoot();
        }

        protected abstract TreeViewItem GetBuildRoot();

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            return base.BuildRows(root);
        }
        
        protected override void RowGUI(RowGUIArgs args)
        {
            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
                CellGUI(args.GetCellRect(i), args.item , args.GetColumn(i), ref args);
        }

        protected abstract void CellGUI(Rect cellRect, TreeViewItem item, int column, ref RowGUIArgs args);
        
        protected override void DoubleClickedItem(int id)
        {
            if (FindItem(id, rootItem) is TreeViewBaseItem assetItem)
            {
                Object o = AssetDatabase.LoadAssetAtPath<Object>(assetItem.asset.fullAssetName);
                EditorGUIUtility.PingObject(o);
                Selection.activeObject = o;
            }
        }

        protected string[] FindAsset(string filter,string[] paths)
        {
            return AssetDatabase.FindAssets(filter,paths);
        }
        
    }
    
    static class MyExtensionMethods
    {
        internal static IOrderedEnumerable<T> Order<T, TKey>(this IEnumerable<T> source, System.Func<T, TKey> selector, bool ascending)
        {
            if (ascending)
            {
                return source.OrderBy(selector);
            }
            else
            {
                return source.OrderByDescending(selector);
            }
        }

        internal static IOrderedEnumerable<T> ThenBy<T, TKey>(this IOrderedEnumerable<T> source, System.Func<T, TKey> selector, bool ascending)
        {
            if (ascending)
            {
                return source.ThenBy(selector);
            }
            else
            {
                return source.ThenByDescending(selector);
            }
        }
    }
}