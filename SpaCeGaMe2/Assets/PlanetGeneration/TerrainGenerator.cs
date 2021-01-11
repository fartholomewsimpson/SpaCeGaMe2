using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace PlanetGeneration
{
    public class TerrainGenerator : MonoBehaviour
    {
        [Range(1, 50)]
        public int width = 10;
        [Range(1, 5)]
        public int definition = 1;

        [Range(1,10)]
        public float persistence  = 1;
        [Range(1,8)]
        public float lacunarity = 1;
        [Range(0,1)]
        public float sparcity = .5f;

        public Directions directions;
        public PlanetFaces planetFaces;

        public NoiseLayer[] noiseLayers;
        float constDisplacementX;
        float constDisplacementY;

# region Lifecycle
        void Refresh()
        {
            var faces = GenerateFaces(width, definition);
            SetNeighbors(faces);
            GetNoisy(faces);
            planetFaces.Refresh(faces);
        }

        void Start()
        {
            Random.InitState(name.GetHashCode());
            constDisplacementX = Random.value;
            constDisplacementY = Random.value;
            Refresh();
        }

        void OnValidate()
        {
            if (Application.isPlaying)
                Refresh();
        }

# endregion

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
            for (int i = 0; i < faces.Length; i++)
            {
                var verts = faces[i].vertices;
                float maxNoise = 0;
                float minNoise = float.MaxValue;
                for (int j = 0; j < verts.Length; j++)
                {
                    var noise = MakeSomeNoise(verts[j]);
                    if (noise > maxNoise)
                        maxNoise = noise;
                    if (noise < minNoise)
                        minNoise = noise;
                    verts[j].z = noise;
                }

                // float range = maxNoise-minNoise;
                // for (int j = 0; j < verts.Length; j++)
                // {
                //     float percentile = (verts[j].z - minNoise) / range;
                //     verts[j].z *= percentile - sparcity;
                //     // TODO: Fix sparcity normalization
                //     // if (percentile < sparcity)
                //     //     verts[j].z *= sparcity - percentile;
                // }
            }
        }

        float MakeSomeNoise(Vector2 value)
        {
            float noise = 0;
            float freq = 1;
            float amp = 1;
            for (int i = 0; i < noiseLayers.Length; i++)
            {
                var layer = noiseLayers[i];
                freq *= layer.frequency * (i+1) * lacunarity;
                var flatNoise = Mathf.PerlinNoise(value.x * freq, value.y * freq);
                amp *= (amp * (i+1)) / persistence;
                noise += flatNoise * amp;
            }
            return noise;
        }
    }
}
