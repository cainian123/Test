using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;


namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.TreeView
{
    public abstract class BaseTreeView : UnityEditor.IMGUI.Controls.TreeView
    {
        public BaseTreeView(TreeViewState state) : base(state)
        {
        }

        public BaseTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {

        }

        protected virtual void OnSortingChanged(MultiColumnHeader mch)
        {
            SortIfNeeded(rootItem, GetRows());
        }

        protected virtual void SortIfNeeded(UnityEditor.IMGUI.Controls.TreeViewItem root, IList<UnityEditor.IMGUI.Controls.TreeViewItem> rows)
        {

        }

        protected override UnityEditor.IMGUI.Controls.TreeViewItem BuildRoot()
        {
            return GetBuildRoot();
        }

        protected abstract UnityEditor.IMGUI.Controls.TreeViewItem GetBuildRoot();
        
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