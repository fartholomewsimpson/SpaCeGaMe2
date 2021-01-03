using UnityEngine;

namespace PlanetGeneration
{
    [System.Serializable]
    public class Face
    {
        public int width;
        public int definition;
        public Vector3 direction;
        public Vector3[] vertices;
        public int[] triangles;
        [System.NonSerialized] public Face[] neighbors;
    }
}
