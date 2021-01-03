using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace PlanetGeneration
{
    public class TerrainGenerator : MonoBehaviour
    {
        [Range(1, 50)]
        public int width = 10;
        // number of vertices per unit of width
        [Range(1, 5)]
        public int definition = 1;
        [Range(1, 10)]
        public float noiseIntensity = 1;
        public Directions directions;
        public PlanetFaces planetFaces;

        void Refresh()
        {
            var faces = GenerateFaces(width, definition);
            SetNeighbors(faces);
            GetNoisy(faces);

            planetFaces.Refresh(faces);
        }

        void Start()
        {
            Refresh();
        }

        void OnValidate()
        {
            if (Application.isPlaying)
                Refresh();
        }

        Face[] GenerateFaces(int width, int definition)
        {
            var pos = transform.position;
            var faces = new Face[directions.directions.Length];
            for (int i = 0; i < faces.Length; i++)
            {
                faces[i] = new Face
                {
                    width = width,
                    definition = definition,
                    direction = directions.directions[i],
                    vertices = MeshGenerationUtils.GeneratePlaneVertices(transform.position, width, definition),
                    triangles = MeshGenerationUtils.GeneratePlaneTriangles(width, definition),
                };
            }
            return faces;
        }

        void SetNeighbors(Face[] faces)
        {
            for (int i = 0; i < faces.Length; i++)
            {
                var cur = faces[i];
                var neighbors = new List<Face>();
                for (int j = 0; j < faces.Length; j++)
                {
                    if (i == j)
                        continue;

                    var neighbor = faces[j];
                    if (i != j && neighbor.direction + cur.direction != Vector3.zero)
                        neighbors.Add(neighbor);
                }
                cur.neighbors = neighbors.ToArray();

                faces[i] = cur;
            }
        }

        void GetNoisy(Face[] faces)
        {
            var vertWidth = (width+1) * 2;
            for (int i = 0; i < faces.Length; i++)
            {
                var verts = faces[i].vertices;
                for (int j = 0; j < verts.Length; j++)
                {
                    var x = NormalizedNoise(verts[j].x);
                    var y = NormalizedNoise(verts[j].y);
                    verts[j].z = Mathf.PerlinNoise(x,y);
                }
                faces[i].vertices = verts;
            }
        }

        float NormalizedNoise(float value)
        {
            // TODO: What to do with noise intensity.
            var vWidth = MeshGenerationUtils.GetVerticesWidth(width, definition);
            return Mathf.Pow(Mathf.Abs(value / vWidth), noiseIntensity);
        }
    }
}
