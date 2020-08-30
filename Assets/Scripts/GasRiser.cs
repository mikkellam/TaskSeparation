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

        private ParticleSystem _particleSystem;

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
            var ownPos = this.transform.position;

            //direction to the end destination
            var dir = (_destination - ownPos);

            var dirNorm = dir.normalized;

            var vel = dirNorm * Speed * Time.deltaTime;

            var magVel = vel.magnitude;

            var dest = ownPos + vel;

            //get the ground position for the next step
            var ray = new Ray(ownPos + Vector3.up * 500f, Vector3.down);

            if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, GasOutputManager.Instance.TerrainLayerMask))
            {
                return;
            }

            var groundPos = hit.point;
            dest.y = groundPos.y + _gasHeight;

            if (_isRising)
            {
                //we are below the desired destination, so move vertically
                Rise(ownPos, dest);
                return;
            }

            var dirNextStep = (dest - ownPos);
            var magDirNextStep = dirNextStep.magnitude;
            var velNextStep = dirNextStep.normalized * Speed;
            var magVelNextStep = velNextStep.magnitude;

            if (magVelNextStep > magDirNextStep)
            {
                //set to position, if we are closer than the next step, to avoid overshooting
                this.transform.position = dest;
            }
            else
            {
                this.transform.position += velNextStep;
            }
        }

        private void Rise(Vector3 ownPos, Vector3 dest)
        {
            var velVertical = Vector3.up * Speed * Time.deltaTime;
            var magVelVertical = velVertical.magnitude;
            var distToDest = dest.y - ownPos.y;

            if (magVelVertical < distToDest)
            {
                //move the gas upwards towards the destination
                this.transform.position += velVertical;
            }
            else
            {
                //one step further would take us beyond the vertical destination, to snap the position to the vertical destination
                ownPos.y = dest.y;
                this.transform.position = ownPos;
            }

            if (ownPos.y >= dest.y)
            {
                _isRising = false;
                ResetScale();
            }
            else
            {
                AdjustScale();
            }
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