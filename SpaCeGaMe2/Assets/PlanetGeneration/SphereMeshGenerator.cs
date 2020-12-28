using ScriptableObjects;
using UnityEngine;

namespace PlanetGeneration
{
    [RequireComponent(typeof(MeshFilter))]
    public class SphereMeshGenerator : MonoBehaviour
    {
        [Range(1, 50)]
        public int width = 10;

        // number of vertices per unit of width
        [Range(1, 5)]
        public int definition = 1;
        public Directions directions;

        // void Awake()
        // {
        //     Refresh();
        // }

        void Update()
        {
            Refresh();
        }

        void Start()
        {
            Refresh();
        }
        void Refresh()
        {
            // TODO: Remove old children
            var children = GetComponentsInChildren<MeshFilter>();
            var pos = transform.position;
            var radius = width/2;
            var combines = new CombineInstance[directions.directions.Length];
            for (int i = 0; i < combines.Length; i++)
            {
                var planeObj = new GameObject($"face_{i}");
                planeObj.transform.parent = transform;
                var planeMesh = planeObj.AddComponent<MeshFilter>().mesh;
                planeMesh.Clear();
                planeMesh.vertices = MeshGenerationUtils.GeneratePlaneVertices(planeObj.transform.position, width, definition);
                planeMesh.triangles = MeshGenerationUtils.GeneratePlaneTriangles(width, definition);
                planeObj.transform.LookAt(-directions.directions[i]);
                planeObj.transform.localPosition = directions.directions[i] * radius;

                combines[i].mesh = planeMesh;
                combines[i].transform = planeObj.transform.localToWorldMatrix;
            }

            var mesh = GetComponent<MeshFilter>().mesh;
            mesh.Clear();
            mesh.CombineMeshes(combines, true, true, false);
            mesh.RecalculateNormals();
        }

        void OnDrawGizmosSelected()
        {
            var mesh = GetComponent<MeshFilter>().sharedMesh;
            var vertices = mesh?.vertices;
            var triangles = mesh?.triangles;

            if (vertices != null)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    Gizmos.DrawSphere(vertices[i], .05f);
                }
            }

            if (triangles != null)
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
