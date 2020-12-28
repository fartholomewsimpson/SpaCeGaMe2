using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName="Directions", menuName="Directions Scriptable Object", order=51)]
    public class Directions : ScriptableObject
    {
        public readonly Vector3[] directions = new Vector3[]
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back,
        };
    }
}
