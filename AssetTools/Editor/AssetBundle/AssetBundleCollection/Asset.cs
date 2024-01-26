using UnityEditor;

namespace AssetTools.Editor.AssetBundle.AssetBundleCollection
{
   /// <summary>
   /// 资源
   /// </summary>
   public class Asset
   {

      private Asset(string guid, Resource resource)
      {
         Guid = guid;
         Resource = resource;
      }

      public string Guid { get; private set; }

      public Resource Resource { get; private set; }

      public string Name
      {
         get { return AssetDatabase.GUIDToAssetPath(Guid); }
      }

      public void SetAssetBundle(Resource resource)
      {
         Resource = resource;
      }

      public static Asset Create(string guid)
      {
         return new Asset(guid, null);
      }

      public static Asset Create(string guid, Resource resource)
      {
         return new Asset(guid, resource);
      }
   }
}
