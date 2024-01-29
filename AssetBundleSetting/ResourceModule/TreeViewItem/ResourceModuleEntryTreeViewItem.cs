using AssetStream.Editor.AssetBundleSetting.ResourceModule.Config;
using UnityEditor;
using UnityEngine;

namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.TreeViewItem
{
    public class ResourceModuleEntryTreeViewItem:UnityEditor.IMGUI.Controls.TreeViewItem
    {
        private ResourceModuleInfo m_ResourceModuleInfo;
        public ResourceModuleEntryTreeViewItem(ResourceModuleInfo e, int d) : base(e == null ? 0 : (e.packagePath).GetHashCode(), d, e == null || !e.IsHaveExit ? "[Missing Reference]" : e.packageName)
        {
            m_ResourceModuleInfo = e;
        }

        public bool IsMissing
        {
            get
            {
                if (m_ResourceModuleInfo != null  && m_ResourceModuleInfo.IsHaveExit)
                    return false;
                return true;
            }
        }
        
        public ResourceModuleInfo ResourceModuleInfo
        {
            get { return m_ResourceModuleInfo; }
        }
        
    
    }
}