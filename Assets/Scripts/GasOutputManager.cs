namespace Robots
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using UnityEngine;

    /// <summary>
    /// Manages the output and diffusion gasses into the atmosphere of the asteroid - received messages from e.g. the converter
    /// </summary>
    public class GasOutputManager : MonoBehaviour
    {
        public static GasOutputManager Instance;

        /// <summary>
        /// The height above the navmesh the gas is floating in
        /// </summary>
        public float GasHeight = 24f;

        /// <summary>
        /// The minimum separation between gas -> i.e. the gas systems a repulsing each other
        /// </summary>
        public float Separation = 4f;

        /// <summary>
        /// The frequency where new separation is calculated
        /// </summary>
        public float SeparationTickFrequency = 1f;

        public bool _logDebug;

        public LayerMask TerrainLayerMask;

        /// <summary>
        /// Next time the separation loop is run
        /// </summary>
        private float _nextSeparationTick = 0;
        
        /// <summary>
        /// List handling the registration of system
        /// </summary>
        private HashSet<GasRiser> _activeGas = new HashSet<GasRiser>();

        private Stopwatch _stopwatch;

        private void Awake()
        {
            Instance = this;
            _stopwatch = new Stopwatch();
        }

        private void Update()
        {
            SeparateGas();
        }
        

        public void OutputGas(Vector3 exhaustPoint, GameObject GasPrefab)
        {
            var gasDestination = GetGasDestination(exhaustPoint);
            var go = GameObject.Instantiate(GasPrefab);
            var tr = go.transform;
            tr.position = exhaustPoint;

            var gasRiser = go.GetComponent<GasRiser>();
            tr.localScale = tr.localScale * gasRiser.Size.Evaluate(0f);
            _activeGas.Add(gasRiser);

            gasRiser.Initialize(gasDestination, GasHeight);
        }

        private void SeparateGas()
        {
            _stopwatch.Start();

            //for (int i = 0; i < count; i++)
            foreach (var gas in _activeGas)
            {
                //var gas = _activeGas[i];
                var self = gas.transform.position;
                var separationVector = GetSeparationVector(_activeGas, gas, Separation);

                var dest = self + separationVector;

                var ray = new Ray(dest + Vector3.up * 500f, Vector3.down);

                if (!Physics.Raycast(ray,out RaycastHit hit, 1000f, TerrainLayerMask))
                {
                    return;
                }

                dest = hit.point;
                dest += Vector3.up * GasHeight;

                gas.SetDestination(dest);
            }

            _stopwatch.Stop();

            if(_logDebug)
                UnityEngine.Debug.Log("_stopwatch == " + _stopwatch.ElapsedMilliseconds + " " + _stopwatch.ElapsedTicks);
        }
        
        private Vector3 GetGasDestination(Vector3 exhaustPosition)
        {
            var pos = exhaustPosition;
            pos += Vector3.up * GasHeight;
            return pos;
        }

        public Vector3 GetSeparationVector(HashSet<GasRiser> others, GasRiser self, float separation)
        {
            var selfPos = self.transform.position;

            var count = others.Count;

            var separationVector = Vector3.zero;

            if (count == 0)
            {
                return Vector3.zero;
            }

            var countSeparators = 0;

            foreach (var other in others)
            {
                if (other.Equals(self))
                {
                    continue;
                }

                var otherPos = other.transform.position;

                var dirToOther = selfPos - otherPos;

                var mag = dirToOther.magnitude;

                if (mag > separation)
                {
                    continue;
                }

                if (mag < Mathf.Epsilon)
                {
                    //the systems are located in exactly the same position, so they need to separate
                    var randomAngle = Random.value * 360f;
                    dirToOther = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;
                }

                separationVector += dirToOther.normalized * (separation - mag);

                countSeparators++;
            }

            if (countSeparators > 0)
            {
                separationVector /= countSeparators;
            }

            return separationVector;
        }
    }
}