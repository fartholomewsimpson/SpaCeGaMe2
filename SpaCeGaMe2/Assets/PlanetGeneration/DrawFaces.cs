using ScriptableObjects;
using UnityEngine;

namespace PlanetGeneration
{
    [RequireComponent(typeof(MeshFilter))]
    public class DrawFaces : MonoBehaviour
    {
        public PlanetFaces planetFaces;

        int cur = 0;

        void Start()
        {
            if (Application.isPlaying)
                planetFaces.Subscribe(name, OnRefresh);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                cur = (cur + 1) % planetFaces.Faces.Length;
            }
        }

        void OnRefresh(Face[] faces)
        {
            var face = faces[cur];
            var mesh = GetComponent<MeshFilter>().sharedMesh;
            mesh.Clear();
            mesh.vertices = face.vertices;
            mesh.triangles = face.triangles;
            mesh.colors = GetColorValues(face.vertices);
            mesh.uv = GetUVs(face.vertices);
            mesh.RecalculateNormals();
        }

        Color[] GetColorValues(Vector3[] vertices)
        {
            var colors = new Color[vertices.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(vertices[i].z, vertices[i].z, vertices[i].z);
            }
            return colors;
        }

        Vector2[] GetUVs(Vector3[] vertices)
        {
            var uvs = new Vector2[vertices.Length];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
            }
            return uvs;
        }
    }
}
