using ScriptableObjects;
using UnityEngine;

namespace PlanetGeneration
{
    [RequireComponent(typeof(MeshFilter))]
    public class PlanetRenderer : MonoBehaviour
    {
        public PlanetFaces planetFaces;

        void Start()
        {
            planetFaces.Subscribe(name, OnRefresh);
        }

        void OnRefresh(Face[] faces)
        {
            var mesh = GetComponent<MeshFilter>().mesh;
            mesh.Clear();
            var combineInstances = new CombineInstance[faces.Length];
            for (int i = 0; i < faces.Length; i++)
            {
                var f = faces[i];
                float radius = f.width/2f;
                var t = new GameObject().transform; // TODO: check for leaks
                t.position = f.direction * radius;
                t.forward = f.direction;
                t.LookAt(f.direction);
                combineInstances[i] = new CombineInstance
                {
                    mesh = new Mesh { vertices = f.vertices, triangles = f.triangles },
                    transform = t.localToWorldMatrix,
                };
                Destroy(t.gameObject);
            }
            mesh.CombineMeshes(combineInstances, true, true);
            mesh.RecalculateNormals();
        }
    }
}
