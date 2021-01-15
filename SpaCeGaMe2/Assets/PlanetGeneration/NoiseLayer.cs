using UnityEngine;

namespace PlanetGeneration
{
    [System.Serializable]
    public class NoiseLayer
    {
        [Range(0,10)]
        public float intensity;
        [Range(0,1)]
        public float frequency;
    }
}
