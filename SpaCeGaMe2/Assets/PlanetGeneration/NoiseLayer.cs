using UnityEngine;

namespace PlanetGeneration
{
    [System.Serializable]
    public class NoiseLayer
    {
        [Range(0,1)]
        public float intensity;
        [Range(0,.5f)]
        public float frequency;
    }
}
