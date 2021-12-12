using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace WORLDGAMEDEVELOPMENT
{
    public sealed class ShipController : NetworkMovableObject
    {
        #region Fields

        [SyncVar] private string _playerName;

        [SerializeField] private Transform _cameraAttach;

        private CameraOrbit _cameraOrbit;
        private PlayerLabel _playerLabel;
        private Rigidbody _rigidBody;

        private float _shipSpeed;

        #endregion


        #region Properties

        public string PlayerName { get => _playerName; set => _playerName = value; }
        protected override float Speed => _shipSpeed;

        #endregion


        #region Methods

        public override void OnStartAuthority()
        {
            _rigidBody = GetComponent<Rigidbody>();
            if (_rigidBody == null)
            {
                return;
            }
            gameObject.name = PlayerName;
            _cameraOrbit = FindObjectOfType<CameraOrbit>();
            _cameraOrbit.Initiate(_cameraAttach == null ? transform : _cameraAttach);
            _playerLabel = GetComponentInChildren<PlayerLabel>();
            base.OnStartAuthority();
        }

        protected override void FromServerUpdate()
        {

        }

        protected override void HasAuthorityMovement()
        {
            var spaceShipSettings = SettingsContainer.Instance?.SpaceShipSettings;

            if (spaceShipSettings == null) return;

            var isFaster = Input.GetKey(KeyCode.LeftShift);
            var speed = spaceShipSettings.ShipSpeed;
            var faster = isFaster ? spaceShipSettings.Faster : 1.0f;
            _shipSpeed = Mathf.Lerp(_shipSpeed, speed * faster, SettingsContainer.Instance.SpaceShipSettings.Acceleration);

            var currentFov = isFaster ? SettingsContainer.Instance.SpaceShipSettings.FasterFov : SettingsContainer.Instance.SpaceShipSettings.NormalFov;
            _cameraOrbit.SetFov(currentFov, SettingsContainer.Instance.SpaceShipSettings.ChangeFovSpeed);
            var velocity = _cameraOrbit.transform.TransformDirection(Vector3.forward) * _shipSpeed;
            _rigidBody.velocity = velocity * Time.deltaTime;

            if (!Input.GetKey(KeyCode.C))
            {
                var targetRotation = Quaternion.LookRotation(Quaternion.AngleAxis(_cameraOrbit.LookAngle, -transform.right) * velocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            }
        }

        protected override void SendToServer()
        {

        }

        [ClientCallback]
        private void LateUpdate()
        {
            _cameraOrbit?.CameraMovement();
        }

        #endregion


        #region UnityMethods

        private void OnGUI()
        {
            if (_cameraOrbit == null)
            {
                return;
            }
            _cameraOrbit.ShowPlayerLabels(_playerLabel);
        }

        #endregion
    }
}