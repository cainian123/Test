namespace AssetCheckTools.Editor.Font.View
{
    public class GameUiTab : FontBaseTab
    {
        
        protected override string[] ForceReloadData()
        {
            return FindAsset("t:prefab", new []{"Assets/__UIData/_Resources/Prefab"});
        }
    }
}