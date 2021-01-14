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
        // TODO: Store min and max vertex shade values in here
        [System.NonSerialized] public Face[] neighbors;
    }
}
