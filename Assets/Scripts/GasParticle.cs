namespace Robots
{
    using UnityEngine;

    /// <summary>
    /// Rises to the gas height and then moves to destination
    /// </summary>
    public class GasParticle : MonoBehaviour
    {
        /// <summary>
        /// Speed the gas is moving
        /// </summary>
        public float Speed = 1f;

        /// <summary>
        /// The easing function used to determine the size of the particle depending on its distance travelled
        /// </summary>
        public AnimationCurve SizeFunc;

        /// <summary>
        /// The height above the terrain the gas is floating in
        /// </summary>
        private float _gasHeight;

        /// <summary>
        /// The point towards which the gas is moving
        /// </summary>
        private Vector3 _destination;
        /// <summary>
        /// The height above the terrain the gas was initialized
        /// </summary>
        private float _startHeight;
        /// <summary>
        /// The difference between the gas initial y value and the floating height
        /// </summary>
        private float _distanceToTravel;

        public void Initialize(Vector3 destination, float gasHeight)
        {
            _startHeight = this.transform.position.y;
            _destination = destination;
            _gasHeight = gasHeight;
            _distanceToTravel = _destination.y - _startHeight;
        }
        public void SetDestination(Vector3 destination)
        {
            _destination = destination;
        }

        private void Update()
        {
            Tick();
        }
        /// <summary>
        /// Moves the particle towards its destination and adjusts its scale
        /// </summary>
        public void Tick()
        {
            //Destinction between movement when rising and when separating was dropped here as it simplified code
            //and looked neater during the rising given separation distance wasn't too high
            var ownpos = transform.position;
            var direction = _destination - ownpos;
            var distToDestination = direction.magnitude;
            var normalizedDir = direction.normalized;
            //the vector describing the movement of the particle in this tick
            var step = (normalizedDir * Speed * Time.deltaTime);

            if (distToDestination <= step.magnitude)
            {
                transform.position = _destination;
            }
            else
            {
                transform.position += step;
            }

            AdjustScale();
        }
        /// <summary>
        /// adjusts the size of the particle depending on the easing function chosen and hoe far the particle has travelled
        /// </summary>
        private void AdjustScale()
        {
            //calculate the distance we have travelled
            var distanceTravelled = this.transform.position.y - _startHeight;

            var fraction = Mathf.Clamp(distanceTravelled / _distanceToTravel, 0f, 1f);

            var size = SizeFunc.Evaluate(fraction);

            this.transform.localScale = new Vector3(size, size, size);
        }
        
    }
}