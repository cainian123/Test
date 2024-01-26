namespace AssetCheckTools.Editor.Font.View
{
    public class LoadingUiTab : FontBaseTab
    {
       
        protected override string[] ForceReloadData()
        {
           return FindAsset("t:prefab", new []{"Assets/__UIData/Resources"});
        }
    }
}