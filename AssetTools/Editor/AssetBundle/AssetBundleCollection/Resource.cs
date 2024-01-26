
using System.Collections.Generic;
using AssetTools.Editor.Util;
namespace AssetTools.Editor.AssetBundle.AssetBundleCollection
{
   /// <summary>
   /// 资源包
   /// </summary>
   public class Resource
   {
      private readonly List<Asset> m_Assets;
  

      private Resource(string name, string variant, bool packed,bool basePacked,bool isCompress)
      {
         m_Assets = new List<Asset>();
         Name = name.ToLower();
         Variant = variant?.ToLower();
       
         Packed = packed;
         BasePacked = basePacked;
         Compress = isCompress;
      }

      public string Name { get; private set; }

      public string Variant { get; private set; }

      public string FullName
      {
         get { return Variant != null ? Utility.Text.Format("{0}.{1}", Name, Variant) : Name; }
      }
      

      public bool Packed { get; private set; }
      
      public bool BasePacked { get; private set; }
      
      public bool Compress { get; private set; }

      public static Resource Create(string name, string variant, bool packed,bool basePacked,bool isCompress)
      {
         return new Resource(name, variant, packed,basePacked,isCompress);
      }

      public Asset[] GetAssets()
      {
         return m_Assets.ToArray();
      }

      public void Rename(string name, string variant)
      {
         Name = name;
         Variant = variant;
      }
      

      public void SetPacked(bool packed)
      {
         Packed = packed;
      }

      public void AssignAsset(Asset asset, bool isScene)
      {
  
         asset.SetAssetBundle(this);
         m_Assets.Add(asset);
         //m_Assets.Sort(AssetComparer);
      }

      public void Unassign(Asset asset)
      {
         asset.SetAssetBundle(null);
         m_Assets.Remove(asset);
       
      }
      
      
      public void Clear()
      {
         foreach (Asset asset in m_Assets)
         {
            asset.SetAssetBundle(null);
         }

         m_Assets.Clear();
      }

      private int AssetComparer(Asset a, Asset b)
      {
         return a.Guid.CompareTo(b.Guid);
      }
   }
}
