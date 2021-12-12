using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;


namespace WORLDGAMEDEVELOPMENT
{
    public sealed class HudController : MonoBehaviour
    {
        #region PrivateFields

        private const string PLAYER_NAME_PREF_KEY = "PlayerNameHUD";

        #endregion

        [SerializeField] private GameObject _panelUIConnected;
        [SerializeField] private GameObject _panelUIStop;

        [SerializeField] private Button _buttonStartHost;
        [SerializeField] private Button _buttonStartClient;
        [SerializeField] private Button _buttonLanServerOnly;
        [SerializeField] private Button _buttonEnableMatchMaker;

        [SerializeField] private Button _buttonStop;
        [SerializeField] private TMP_InputField _inputPlayerName;

        [SerializeField] private SolarSystemNetworkManager _manager;
        private event Action _isConnectedPlayer;

        public string PlayerName { get; protected set; }

        private void Start()
        {
            _manager = NetworkManager.singleton as SolarSystemNetworkManager;
            _panelUIConnected.SetActive(true);
            _buttonStartHost.onClick.AddListener(StartHost);
            _buttonStartClient.onClick.AddListener(StartClient);
            _buttonLanServerOnly.onClick.AddListener(() => _manager.StartServer());
            _buttonEnableMatchMaker.onClick.AddListener(() => _manager.StartMatchMaker());

            SetDefaultPlayerName();
            _inputPlayerName.onEndEdit.AddListener(SetPlayerName);
            //_manager.OnPlayerSpawned += SetPlayerNamePrefab;
            _manager.OnPlayerSpawned += SetPlayerNamePrefab;
        }

        private void SetPlayerNamePrefab(ShipController ship)
        {
            //if (ship.hasAuthority && ship.isLocalPlayer)
            //{
            //    ship.CmdSetPlayerName(PlayerName);
            //}
            ship.PlayerName = PlayerName;
            ship.RpcPlayerNameUpdate();
            ship.UpdatePlayerName();
        }

        private void SetPlayerName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Player Name is null or Empty");
                return;
            }

            PlayerName = name;
            PlayerPrefs.SetString(PLAYER_NAME_PREF_KEY, name);
        }

        private void SetDefaultPlayerName()
        {
            string defaultName = string.Empty;
            if (_inputPlayerName != null)
            {
                if (PlayerPrefs.HasKey(PLAYER_NAME_PREF_KEY))
                {
                    defaultName = PlayerPrefs.GetString(PLAYER_NAME_PREF_KEY);
                    _inputPlayerName.text = defaultName;
                    PlayerName = defaultName;
                }
            }
        }

        private void StartClient()
        {
            _manager.StartClient();
            _panelUIConnected.SetActive(false);
            _panelUIStop.SetActive(true);
            _buttonStop.onClick.AddListener(StopClient);

            //FindObjectOfType<ShipController>().PlayerName = PlayerName;
            //Debug.Log(FindObjectOfType<ShipController>().PlayerName);
        }

        private void StartHost()
        {
            _manager.StartHost();
            _panelUIConnected.SetActive(false);
            _panelUIStop.SetActive(true);
            _buttonStop.onClick.AddListener(StopHost);

            //FindObjectOfType<ShipController>().PlayerName = PlayerName;
            //Debug.Log(FindObjectOfType<ShipController>().PlayerName);
        }

        private void StopClient()
        {
            _manager.StopClient();
            //_manager = NetworkManager.singleton as SolarSystemNetworkManager;

            _panelUIConnected.SetActive(true);
            _panelUIStop.SetActive(false);
        }

        private void StopHost()
        {
            _manager.StopHost();
            //_manager = NetworkManager.singleton as SolarSystemNetworkManager;

            _panelUIConnected.SetActive(true);
            _panelUIStop.SetActive(false);
        }

        private void OnDestroy()
        {
            _panelUIConnected.SetActive(false);
            _buttonStartHost.onClick.RemoveAllListeners();
            _buttonStartClient.onClick.RemoveAllListeners();
            _buttonLanServerOnly.onClick.RemoveAllListeners();
            _buttonEnableMatchMaker.onClick.RemoveAllListeners();
            _buttonStop.onClick.RemoveAllListeners();
            _manager.OnPlayerSpawned -= SetPlayerNamePrefab;

        }
    }
}
