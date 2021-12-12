using System;
using UnityEngine;
using UnityEngine.Networking;


namespace WORLDGAMEDEVELOPMENT
{
    public sealed class SolarSystemNetworkManager : NetworkManager
    {
        #region PrivateFields

        private const string PLAYER_NAME_PREF_KEY = "PlayerNameSolar";

        #endregion


        #region Fields

        [SerializeField] private string _playerName;
        public event Action<ShipController> OnPlayerSpawned = delegate {};

        #endregion


        #region NetworkManager

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            Debug.LogError("K><J");
            var spawnTransform = GetStartPosition();
            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            var playerShip =  player.GetComponent<ShipController>();
            player.name = _playerName;
            
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
            
            OnPlayerSpawned?.Invoke(playerShip);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
        }
        #endregion
    }
}
