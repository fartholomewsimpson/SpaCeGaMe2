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
        public Directions directions;
        public Face[] faces;

        void Start()
        {
            faces = GenerateFaces(width, definition);
            SetNeighbors(faces);
            GetNoisy(faces);
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
            }
        }

        void GetNoisy(Face[] faces)
        {
            for (int i = 0; i < faces.Length; i++)
            {
                var verts = faces[i].vertices;
                for (int j = 0; j < verts.Length; j++)
                {
                    // verts[j].
                }
            }
        }
    }
}
