namespace AssetStream.Editor.AssetBundleSetting.ResourceModule.Util
{
    public static class Util
    {
        public static class Path
        {
            /// <summary>
            /// 获取规范的路径
            /// </summary>
            /// <param name="path">路径</param>
            /// <returns>路径</returns>
            public static string GetRegularPath(string path)
            {
                return path?.Replace('\\', '/');
            }

            /// <summary>
            /// 获取连接后的路径
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public static string GetCombinePath(params string[] path)
            {
                if (path == null || path.Length < 1)
                    return null;
                string combinePath = path[0];
                for (int i = 1; i < path.Length; i++)
                {
                    combinePath = System.IO.Path.Combine(combinePath, path[i]);
                }

                return GetRegularPath(combinePath);
            }
        }
    }
}