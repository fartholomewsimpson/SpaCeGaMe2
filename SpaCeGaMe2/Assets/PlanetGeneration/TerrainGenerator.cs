using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace PlanetGeneration
{
    [RequireComponent(typeof(MeshFilter))]
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

        public NoiseLayer[] noiseLayers;
        float constDisplacementX;
        float constDisplacementY;

# region Lifecycle
        void Refresh()
        {
            var faces = GenerateFaces(width, definition);
            var mesh = GetComponent<MeshFilter>().mesh;
            mesh.Clear();

            var combines = new CombineInstance[faces.Length];
            for (int i = 0; i < combines.Length; i++)
            {
                var f = faces[i];
                float radius = f.width/2f;
                var t = new GameObject().transform;
                t.position = (f.direction * radius) + transform.position;
                t.LookAt(f.direction + t.position);
                t.RotateAround(t.position, t.right, 90);
                combines[i] = new CombineInstance
                {
                    mesh = new Mesh { vertices = f.vertices, triangles = f.triangles },
                    transform = t.localToWorldMatrix,
                };
                Destroy(t.gameObject);
            }
            mesh.CombineMeshes(combines, true, true);

            mesh.vertices = MakeSpherical(mesh.vertices);
            mesh.vertices = GetNoisy(mesh.vertices);
            mesh.RecalculateNormals();
        }

        void Start()
        {
            Random.InitState(name.GetHashCode());
            constDisplacementX = Random.value;
            constDisplacementY = Random.value;
            Refresh();
        }

        void Update()
        {
            if (Application.isPlaying)
                Refresh();
        }

# endregion

        Face[] GenerateFaces(int width, int definition)
        {
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

        Vector3[] GetNoisy(Vector3[] vertices)
        {
            float maxNoise = 0;
            float minNoise = float.MaxValue;
            for (int i = 0; i < vertices.Length; i++)
            {
                var loc = vertices[i] - transform.position;
                var noise = MakeSomeNoise(loc);
                if (noise > maxNoise)
                    maxNoise = noise;
                if (noise < minNoise)
                    minNoise = noise;
                vertices[i] = loc + (loc.normalized * noise);
            }
            return vertices;

            // float range = maxNoise-minNoise;
            // for (int j = 0; j < vertices.Length; j++)
            // {
            //     float percentile = (vertices[j].y - minNoise) / range;
            //     vertices[j].y *= percentile - sparcity;
            //     // TODO: Fix sparcity normalization
            //     // if (percentile < sparcity)
            //     //     vertices[j].y *= sparcity - percentile;
            // }
        }

        // TODO: Somethings wrong with lacunarity and persistence.
        float MakeSomeNoise(Vector3 v)
        {
            float noise = 0;
            float freq = 1;
            float amp = 1;
            for (int i = 0; i < noiseLayers.Length; i++)
            {
                var layer = noiseLayers[i];
                freq *= layer.frequency * Mathf.Pow(lacunarity, i);
                float fx = freq * v.x;
                float fy = freq * v.y;
                float fz = freq * v.z;
                var noises = new float[6]
                {
                    Mathf.PerlinNoise(fx, fy),
                    Mathf.PerlinNoise(fx, fz),
                    Mathf.PerlinNoise(fy, fz),
                    Mathf.PerlinNoise(fy, fx),
                    Mathf.PerlinNoise(fz, fx),
                    Mathf.PerlinNoise(fz, fy),
                };
                float totNoise = 0;
                for (int j = 0; j < noises.Length; j++)
                {
                    totNoise += noises[j];
                }
                float meanNoise = totNoise / noises.Length;
                amp *= layer.intensity * Mathf.Pow(persistence, i);
                noise += meanNoise * amp;
            }
            return noise;
        }

        Vector3[] MakeSpherical(Vector3[] vertices)
        {
            float radiusLength = width / 2f;
            for (int i = 0; i < vertices.Length; i++)
            {
                var rad = vertices[i] - transform.position;
                vertices[i] = rad.normalized * radiusLength;
            }
            return vertices;
        }
    }
}
