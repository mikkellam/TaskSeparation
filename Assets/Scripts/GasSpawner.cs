namespace Robots
{
    using System.Collections;
    using UnityEngine;

    public class GasSpawner : MonoBehaviour
    {
        public float Delay = 1f;

        public float TimeBetweenSpawn = 0.25f;

        public float SpawnRadius = 1f;

        public int Count = 100;

        public Transform ExhaustPoint;

        public GameObject GasPrefab;

        private void Start()
        {
            StartCoroutine(nameof(RunSpawn));
        }

        private IEnumerator RunSpawn()
        {
            yield return new WaitForSeconds(Delay);

            while (Count-- > 0)
            {
                var dir = Random.insideUnitSphere;
                dir.y = 0f;
                dir *= SpawnRadius;
                var pos = ExhaustPoint.position + dir;
                GasOutputManager.Instance.OutputGas(pos, GasPrefab);
                yield return new WaitForSeconds(TimeBetweenSpawn);
            }
        }
    }
}