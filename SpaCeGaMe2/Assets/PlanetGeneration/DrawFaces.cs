using System.Linq;
using ScriptableObjects;
using UnityEngine;

namespace PlanetGeneration
{
    [RequireComponent(typeof(MeshFilter))]
    public class DrawFaces : MonoBehaviour
    {
        public PlanetFaces planetFaces;

        int cur = 0;
        bool terrainColors = false;

        void Start()
        {
            planetFaces.Subscribe(name, OnRefresh);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                cur = (cur + 1) % planetFaces.Faces.Length;
                OnRefresh(planetFaces.Faces);
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                terrainColors = !terrainColors;
                OnRefresh(planetFaces.Faces);
            }
        }

        void OnRefresh(Face[] faces)
        {
            var face = faces[cur];
            var mesh = GetComponent<MeshFilter>().sharedMesh;
            mesh.Clear();
            mesh.vertices = face.vertices.Select(v => new Vector3(v.x, v.y, 0)).ToArray();
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
                var colVal = vertices[i].y;
                if (terrainColors)
                {
                    // TODO: Instead of incrementing between 0 and 1, go between min and max verts[i].z value.
                    if (colVal < .3f)
                        colors[i] = Color.blue;
                    else if (colVal < .6f)
                        colors[i] = Color.green;
                    else if (colVal < .9f)
                        colors[i] = Color.grey;
                    else
                        colors[i] = Color.white;
                }
                else
                // TODO: Crush values to value between 0 and 1 using min and max verts
                    colors[i] = new Color(colVal, colVal, colVal);
            }
            return colors;
        }

        Vector2[] GetUVs(Vector3[] vertices)
        {
            var uvs = new Vector2[vertices.Length];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
            }
            return uvs;
        }
    }
}
