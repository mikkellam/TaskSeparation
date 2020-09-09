namespace Robots
{
    using UnityEngine;

    /// <summary>
    /// Rises to the gas height and then moves to destination
    /// </summary>
    public class GasRiser : MonoBehaviour
    {
        /// <summary>
        /// Speed the gas is moving
        /// </summary>
        public float Speed = 1f;

        /// <summary>
        /// Size of the gas particle system, used to make the size smaller when it exits the exhaust pipe
        /// </summary>
        public AnimationCurve Size;

        private float _gasHeight;

        private Vector3 _destination;

        private float _startHeight;

        private float _distanceToTravel;

        private bool _isRising = false;

        public void Initialize(Vector3 destination, float gasHeight)
        {
            _startHeight = this.transform.position.y;
            _destination = destination;
            _gasHeight = gasHeight;
            _distanceToTravel = _destination.y - _startHeight;
            _isRising = true;
        }

        private void OnEnable()
        {
            ResetScale();
        }
        public void SetDestination(Vector3 destination)
        {
            _destination = destination;
        }

        private void Update()
        {
            Tick();
        }
        public void Tick()
        {
            //since we have a defined end distination that is managed outside of the class we do not need to distinguish between rising and separating the particles here
            var ownpos = transform.position;
            var direction = _destination - ownpos;
            var distToDestination = direction.magnitude;
            var normalizedDir = direction.normalized;
            //the vector describing the movement of the partice in this tick
            var step = (normalizedDir * Speed * Time.deltaTime);

            if (distToDestination <= step.magnitude)
            {
                transform.position = _destination;
            }
            else
            {
                transform.position += step;
            }
            
            //We do need to adjust the scale, but this is calculated independant of raycasts and other logic
            AdjustScale();
        }

        private void AdjustScale()
        {
            //calculate the distance we have travelled
            var distanceTravelled = this.transform.position.y - _startHeight;

            var fraction = Mathf.Clamp(distanceTravelled / _distanceToTravel, 0f, 1f);

            var size = Size.Evaluate(fraction);

            this.transform.localScale = new Vector3(size, size, size);
        }

        private void ResetScale()
        {
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}