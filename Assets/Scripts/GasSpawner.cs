namespace Robots
{
    using System.Collections;
    using UnityEngine;

    public class GasSpawner : MonoBehaviour
    {
        /// <summary>
        /// Amount of seconds before the spawn loop initiates
        /// </summary>
        public float Delay = 1f;

        /// <summary>
        /// seconds between each particle spawn
        /// </summary>
        public float TimeBetweenSpawn = 0.25f;

        public float SpawnRadius = 1f;

        public int Count = 100;

        public Transform ExhaustPoint;

        public GameObject GasPrefab;

        private void Start()
        {
            StartCoroutine(nameof(RunSpawn));
        }
        /// <summary>
        /// spawns "Count" particles one at a time around the ExaustPoint 
        /// </summary>
        private IEnumerator RunSpawn()
        {
            yield return new WaitForSeconds(Delay);

            while (Count-- > 0)
            {
                var dir = Random.insideUnitSphere;
                dir.y = 0f;
                dir *= SpawnRadius;
                var pos = ExhaustPoint.position + dir;
                OutputGas(pos, GasPrefab);
                yield return new WaitForSeconds(TimeBetweenSpawn);
            }
        }

        /// <summary>
        /// Initializes a gas particle at the exausetPoint
        /// </summary>
        /// <param name="exhaustPoint">The point at which the gas particle should be spawned</param>
        /// <param name="GasPrefab">The prefab of the gas particle</param>
        public void OutputGas(Vector3 exhaustPoint, GameObject GasPrefab)
        {
            var gasDestination = GetGasDestination(exhaustPoint);
            var go = GameObject.Instantiate(GasPrefab);
            var tr = go.transform;
            tr.position = exhaustPoint;

            var gasParticle = go.GetComponent<GasParticle>();
            tr.localScale = tr.localScale * gasParticle.SizeFunc.Evaluate(0f);
            GasOutputManager.Instance.RegisterGasParticle(gasParticle);

            gasParticle.Initialize(gasDestination, GasOutputManager.Instance.GasHeight);
        }

        /// <summary>
        /// Calculates the end destination of a gasparticle given no interference
        /// </summary>
        /// <returns>
        /// a Vector3 describing The end destination of a gasparticle
        /// </returns>
        /// <param name="exhaustPosition">The starting position of the gas particle</param>
        private Vector3 GetGasDestination(Vector3 exhaustPosition)
        {
            var pos = exhaustPosition;
            pos += Vector3.up * GasOutputManager.Instance.GasHeight;
            return pos;
        }
    }
}