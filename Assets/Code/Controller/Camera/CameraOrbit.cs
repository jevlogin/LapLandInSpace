using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    public sealed class CameraOrbit : MonoBehaviour
    {
        #region Fields

        public Vector3 LookPosition { get; private set; }

        [SerializeField] private Transform _focus = default;
        [SerializeField, Range(0.01f, 1.0f)] private float _distance = 5.0f;
        [SerializeField, Range(0, 90f)] private int _lookAngle;
        [SerializeField, Min(0.0f)] private float _focusRadius = 1.0f;
        [SerializeField, Range(0.0f, 1.0f)] private float _focusCentering = 0.5f;
        [SerializeField, Range(0.1f, 5.0f)] private float _sensivity = 0.5f;
        [SerializeField, Range(1.0f, 360.0f)] private float _rotationSpeed = 90.0f;
        [SerializeField, Range(-89.0f, 89.0f)] private float _minVerticalAngle = -30.0f;
        [SerializeField, Range(-89.0f, 89.0f)] private float _maxVerticalAngle = 60.0f;
        [SerializeField] private LayerMask _obstackleMask;

        private Camera _regularCamera;
        private PlayerLabel _playerLabel;
        private Vector3 _focusPoint;
        private Vector2 _orbitAngles = new Vector2(45.0f, 0.0f);
        private float _currentDistance;
        private float _desiredDistance;

        #endregion


        #region Properties

        private Vector3 _cameraHalfExtends
        {
            get
            {
                Vector3 halfExtends;
                halfExtends.y = _regularCamera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * _regularCamera.fieldOfView);
                halfExtends.x = halfExtends.y * _regularCamera.aspect;
                halfExtends.z = 0.0f;
                return halfExtends;
            }
        }
        public int LookAngle => _lookAngle; 

        #endregion


        #region Methods

        public void Initiate(Transform cameraAttach)
        {
            _focus = cameraAttach;
            transform.parent = null;
            _desiredDistance = _distance;
            _currentDistance = _distance;
            _regularCamera = GetComponent<Camera>();
            _focusPoint = _focus.position;
            transform.localRotation = ConstrainAngles(ref _orbitAngles);
        }

        public void CameraMovement()
        {
            UpdateFocusPoint();
            Quaternion lookRotation = ManualRotation(ref _orbitAngles) ? ConstrainAngles(ref _orbitAngles) : transform.localRotation;
            Vector3 lookDirecrion = lookRotation * Vector3.forward;
            LookPosition = _focusPoint + lookDirecrion;
            if (Physics.BoxCast(_focusPoint, _cameraHalfExtends, -lookDirecrion, out RaycastHit hit, lookRotation, _distance - _regularCamera.nearClipPlane, _obstackleMask))
            {
                _desiredDistance = hit.distance * _regularCamera.nearClipPlane;
            }
            else
            {
                _desiredDistance = _distance;
            }
            _currentDistance = Mathf.Lerp(_currentDistance, _desiredDistance, Time.deltaTime * 20.0f);
            Vector3 lookPosition = _focusPoint - lookDirecrion * _currentDistance;
            transform.SetPositionAndRotation(lookPosition, lookRotation);
        }

        public void SetFov(float fov, float changeSpeed)
        {
            _regularCamera.fieldOfView = Mathf.Lerp(_regularCamera.fieldOfView, fov, changeSpeed * Time.deltaTime);
        }

        public void ShowPlayerLabels(PlayerLabel label)
        {
            label.DrawLabel(_regularCamera);
        }

        private void OnValidate()
        {
            UpdateMinMaxVerticalAngles();
        }

        private void UpdateMinMaxVerticalAngles()
        {
            if (_maxVerticalAngle < _minVerticalAngle)
            {
                _minVerticalAngle = _maxVerticalAngle;
            }
        }

        private void UpdateFocusPoint()
        {
            var targetPoint = _focus.position;
            if (_focusRadius > 0.0f)
            {
                float distance = Vector3.Distance(targetPoint, _focusPoint);
                float t = 1.0f;
                if (distance > 0.01f && _focusCentering > 0.0f)
                {
                    t = Mathf.Pow(1.0f - _focusCentering, Time.deltaTime);
                }

                if (distance > _focusRadius)
                {
                    t = Mathf.Min(t, _focusRadius / distance);
                }
                _focusPoint = Vector3.Lerp(targetPoint, _focusPoint, t);
            }
            else
            {
                _focusPoint = targetPoint;
            }
        }

        private static float GetAngle(Vector2 direction)
        {
            float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
            return direction.x < 0.0f ? 360.0f - angle : angle;
        }

        private bool ManualRotation(ref Vector2 orbitAngles)
        {
            Vector2 input = new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
            float e = Mathf.Epsilon;
            if (input.x < -e || input.x > e || input.y < -e || input.y > e)
            {
                orbitAngles += _rotationSpeed * Time.unscaledDeltaTime * input * _sensivity;
                return true;
            }
            return false;
        }

        private Quaternion ConstrainAngles(ref Vector2 orbitAngles)
        {
            orbitAngles.x = Mathf.Clamp(orbitAngles.x, _minVerticalAngle, _maxVerticalAngle);

            if (orbitAngles.y < 0.0f)
            {
                orbitAngles.y += 360f;
            }
            else if (orbitAngles.y >= 360.0f)
            {
                orbitAngles.y -= 360.0f;
            }
            return Quaternion.Euler(orbitAngles);
        } 

        #endregion
    }
}