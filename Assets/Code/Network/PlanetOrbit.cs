using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    public sealed class PlanetOrbit : NetworkMovableObject
    {
        #region PrivateFields

        private const float CIRCLE_RADIANS = Mathf.PI * 2.0f;

        #endregion


        #region Fields

        [SerializeField] private Transform _aroundPoint;
        [SerializeField] private float _smoothTime = 0.3f;
        [SerializeField] private float _circleInSecond = 1.0f;
        [SerializeField] private float _offsetSin = 1.0f;
        [SerializeField] private float _offsetCos = 1.0f;
        [SerializeField] private float _rotationSpeed;

        private Vector3 _currentPositionSmoothVelocity;
        private float _distance;
        private float _currentAngle;
        private float _currentRotationAngle;

        #endregion


        #region Properties

        protected override float Speed => _smoothTime;

        #endregion


        #region UnityMethods

        private void Start()
        {
            if (isServer)
            {
                _distance = (transform.position - _aroundPoint.position).magnitude;
            }
            Initiate(UpdatePhase.FixedUpdate);
        }

        #endregion


        #region NetworkMovableObject

        protected override void FromServerUpdate()
        {
            if (!isClient)
            {
                return;
            }
            transform.position = Vector3.SmoothDamp(transform.position, _serverPosition, ref _currentPositionSmoothVelocity, Speed);
            transform.rotation = Quaternion.Euler(_serverEuler);
        }

        protected override void HasAuthorityMovement()
        {
            if (!isServer)
            {
                return;
            }
            var position = _aroundPoint.position;
            position.x += Mathf.Sin(_currentAngle) * _distance * _offsetSin;
            position.z += Mathf.Cos(_currentAngle) * _distance * _offsetCos;
            transform.position = position;
            _currentRotationAngle += Time.deltaTime * _rotationSpeed;
            _currentRotationAngle = Mathf.Clamp(_currentRotationAngle, 0.0f, 361.0f);
            if (_currentRotationAngle >= 360.0f)
            {
                _currentRotationAngle = 0.0f;
            }
            transform.rotation = Quaternion.AngleAxis(_currentRotationAngle, transform.up);
            _currentAngle += CIRCLE_RADIANS * _circleInSecond * Time.deltaTime;

            SendToServer();
        }

        protected override void SendToServer()
        {
            _serverPosition = transform.position;
            _serverEuler = transform.eulerAngles;
        }

        #endregion
    }
}
