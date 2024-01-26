using UnityEditor;
using UnityEngine;

namespace AssetCheckTools.Editor.Asset.Data
{
    public class MeshInfo : AssetBaseInfo
    {
        internal long Vertex;
        internal long Triangles;
        internal long Normal;
        internal long Colors;
        internal long Tangents;
        internal bool IsOpenRead;
        public MeshInfo(string guid)
        {
            isFolder = false;
            isScene = false;
            fullAssetName = AssetDatabase.GUIDToAssetPath(guid);
            LoadAsset(fullAssetName);
        }

        private void LoadAsset(string path)
        {
            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (mesh)
            {
                Vertex = mesh.vertexCount;
                Triangles = mesh.triangles.Length/3;
                Normal = mesh.normals.Length;
                Colors = mesh.colors.Length;
                Tangents = mesh.tangents.Length;
                IsOpenRead = mesh.isReadable;
                m_DisplayName = $"{m_DisplayName}/{mesh.name}";
            }
        }
        
        internal virtual Color GetVertexColor()
        {
            if (Vertex <= 500)
                return base.GetColor();
            if(Vertex <= 800)
                return Color.yellow;
            return Color.red;
        }
    }
}