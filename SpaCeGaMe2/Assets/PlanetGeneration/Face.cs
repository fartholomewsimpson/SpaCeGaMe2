using UnityEngine;

namespace PlanetGeneration
{
    public class Face
    {
        public int width;
        public int definition;
        public Vector3 direction;
        public Vector3[] vertices;
        public int[] triangles;
        public Face[] neighbors;
    }
}
