using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssetTools.Editor.AssetBundle.AssetBundleCollection
{
    public class AssetBundleNameInfo
    {
        private string _path;

        private List<AssetBundleNameInfo> _childAssetBundleInfos;
        

        public AssetBundleNameInfo(string path)
        {
            _path = path;
        }

        public bool AddPath(string path)
        {
            if (path == _path)
            {
                Debug.Log("some path  "+_path);
                return true;
            }

            if (_path.Contains(path))
            {
                var temp = new AssetBundleNameInfo(_path);
                if (_childAssetBundleInfos != null && _childAssetBundleInfos.Count > 0)
                {
                    AssetBundleNameInfo[] copyInfos= new AssetBundleNameInfo[_childAssetBundleInfos.Count];
                    _childAssetBundleInfos.CopyTo(copyInfos);
                    for (int i = 0; i < copyInfos.Length; i++)
                    {
                        temp.AddToChild(copyInfos[i]);
                    }
                }
                _path = path;
                AddToChild(temp);
                return true;
            }

            if(_childAssetBundleInfos == null)
                _childAssetBundleInfos = new List<AssetBundleNameInfo>();
            
            if (_childAssetBundleInfos.Count > 0)
            {
                foreach (var bundleInfo in _childAssetBundleInfos)
                {
                    if (bundleInfo.AddPath(path))
                        return true;
                }
            }
            else
            {
                if (path.Contains(_path))
                {
                    _childAssetBundleInfos.Add(new AssetBundleNameInfo(path));
                    return true;
                }
            }

            return false;
        }

        private void AddToChild(AssetBundleNameInfo nameInfo)
        {
            if(_childAssetBundleInfos == null)
                _childAssetBundleInfos = new List<AssetBundleNameInfo>();
            _childAssetBundleInfos.Add(nameInfo);
        }


        public void StartCollectionAsset(Action<string,AssetBundleNameInfo> Func)
        {
            Func?.Invoke(_path,this);
            if (_childAssetBundleInfos != null)
            {
                foreach (var info in _childAssetBundleInfos)
                {
                    info.StartCollectionAsset(Func);
                }
            }
        }

        /// <summary>
        /// 判断是我自己的还是孩子的
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool IsMyAsset(string path)
        {
            if (path == null)
                return false;
            if (string.Equals(path, _path))
                return true;
            if (_childAssetBundleInfos != null)
            {
                foreach (var info in _childAssetBundleInfos)
                {
                    if (info.IsMyAsset(path))
                        return false;
                }
            }

            if(path.Contains(_path))
                return true;
            return false;
        }
        
        public void PrintLog()
        {
            Debug.Log(_path);
            if (_childAssetBundleInfos != null)
            {
                foreach (var bundleInfo in _childAssetBundleInfos)
                {
                    bundleInfo.PrintLog();
                }
            }
        }
    }
}