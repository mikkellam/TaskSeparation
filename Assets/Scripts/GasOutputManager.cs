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
        /// The height above the terrain the gas is floating in
        /// </summary>
        public float GasHeight = 24f;

        /// <summary>
        /// The minimum distance between each gas particle
        /// </summary>
        public float SeparationDist = 4f;

        public LayerMask TerrainLayerMask;
        
        /// <summary>
        /// List of active gas particles
        /// </summary>
        private List<GasParticle> _activeGas = new List<GasParticle>();

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            SeparateGas();
        }
        /// <summary>
        /// Updates destinations of all gas particles such that they are at least SeparationDist units apart
        /// </summary>
        private void SeparateGas()
        {
            foreach (var gas in _activeGas)
            {
                var self = gas.transform.position;
                var separationVector = GetSeparationVector(_activeGas, gas, SeparationDist);

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
        }

        /// <summary>
        /// Calculates the vector describing how "self" needs to move to get "separationDist" units away all other gas
        /// particles in "others" while preserving its height above ground
        /// </summary>
        /// <returns>
        /// a Vector3 describing the direction of movement neccessary to separate from other particles
        /// </returns>
        /// <param name="others">A List of gasparticles to separate from</param>
        /// <param name="self">The Particle that needs to separate</param>
        /// <param name="separationDist">A float describing how close particles may be to each other</param>
        public Vector3 GetSeparationVector(List<GasParticle> others, GasParticle self, float separationDist)
        {
            var selfPos = self.transform.position;

            var numParticles = others.Count;

            var separationVector = Vector3.zero;

            if (numParticles == 0)
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

                var distToOther = dirToOther.magnitude;

                if (distToOther > separationDist)
                {
                    continue;
                }

                if (distToOther < Mathf.Epsilon)
                {
                    //the systems are located in exactly the same position, so they need to separate
                    var randomAngle = Random.value * 360f;
                    dirToOther = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;
                }

                separationVector += dirToOther.normalized * (separationDist - distToOther);

                countSeparators++;
            }

            if (countSeparators > 0)
            {
                separationVector /= countSeparators;
            }

            return separationVector;
        }

        /// <summary>
        /// Adds a GasParticle to the list of active gasses that needs to be managed
        /// </summary>
        /// <param name="gas">The new GasParticle to be tracked</param>
        public void RegisterGasParticle(GasParticle gas)
        {
            _activeGas.Add(gas);
        }
    }
}