using UnityEngine;

namespace Aircraft_Physics.Example.Scripts
{
    [CreateAssetMenu(fileName = "PIDConfig", menuName = "PID Config")]
    public class PidConfig : ScriptableObject
    {
        public float gainProportional = 0.1f;
        public float gainIntegral = 0.1f;
        public float gainDerivative = 0.05f;
        public float outputMax = 1;
        public float outputMin = -1;
    }
}