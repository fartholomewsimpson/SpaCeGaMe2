using ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace PlanetGeneration
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    public class PlanetMeshGenerator : MonoBehaviour
    {
        [Range(1, 50)]
        public int width = 10;

        // number of vertices per unit of width
        [Range(1, 5)]
        public int definition = 1;
        public Directions directions;

        void Start()
        {
            Refresh();
        }

        void Awake()
        {
            Refresh();
        }

        void Update()
        {
            Refresh();
        }

        // TODO: I want to have separate components for making terrain, but currently it lives in here.
        void Refresh()
        {
            var pos = transform.position;
            var radius = width/2f;
            var combines = new CombineInstance[directions.directions.Length];

            // Create and position plane meshes
            for (int i = 0; i < combines.Length; i++)
            {
                var planeObj = new GameObject($"face_{i}").transform;
                planeObj.transform.parent = transform;
                var planeMesh = planeObj.gameObject.AddComponent<MeshFilter>().sharedMesh;
                if (planeMesh == null)
                    planeMesh = new Mesh();

                planeMesh.vertices = MeshGenerationUtils.GeneratePlaneVertices(planeObj.transform.position, width, definition);
                planeMesh.triangles = MeshGenerationUtils.GeneratePlaneTriangles(width, definition);

                // Generate random point heights on Plane mesh
                for (int j = 0; j < planeMesh.vertices.Length; j++)
                {
                    var vertex = planeMesh.vertices[j];
                    vertex.z = Mathf.PerlinNoise(vertex.x, vertex.y) * radius;
                    planeMesh.vertices[j] = vertex;
                }

                planeObj.transform.LookAt(-directions.directions[i]);
                planeObj.transform.localPosition = planeObj.transform.localPosition + (directions.directions[i] * radius);
                combines[i].mesh = planeMesh;
                combines[i].transform = planeObj.transform.localToWorldMatrix;
            }

            // Combine meshes and remove old children.
            var mesh = GetComponent<MeshFilter>().mesh;
            mesh.Clear();
            mesh.CombineMeshes(combines, true, true, false);

            foreach (Transform child in transform)
            {
                if (!EditorApplication.isPlaying)
                    DestroyImmediate(child.gameObject);
                else
                    Destroy(child.gameObject);
            }

            // Normalize vertex radius distances to form sphere
            var newVertices = mesh.vertices;
            for (int i = 0; i < newVertices.Length; i++)
            {
                var dist = newVertices[i] - transform.position;
                newVertices[i] = dist.normalized * radius;
            }
            mesh.vertices = newVertices;
            mesh.RecalculateNormals();
        }

        void OnDrawGizmosSelected()
        {
            var mesh = GetComponent<MeshFilter>().sharedMesh;
            var vertices = mesh?.vertices;
            var triangles = mesh?.triangles;

            if (vertices != null && triangles != null)
            {
                for (int i = 0; i < triangles.Length; i+=3)
                {
                    Gizmos.DrawLine(vertices[triangles[i]], vertices[triangles[i+1]]);
                    Gizmos.DrawLine(vertices[triangles[i+1]], vertices[triangles[i+2]]);
                    Gizmos.DrawLine(vertices[triangles[i+2]], vertices[triangles[i]]);
                }
            }
        }
    }
}
