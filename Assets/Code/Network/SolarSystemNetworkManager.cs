using UnityEngine;
using UnityEngine.Networking;


namespace WORLDGAMEDEVELOPMENT
{
    public sealed class SolarSystemNetworkManager : NetworkManager
    {
        #region PrivateFields

        private const string PLAYER_NAME_PREF_KEY = "PlayerName";

        #endregion


        #region Fields

        [SerializeField] private string _playerName;

        #endregion


        #region NetworkManager

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            var spawnTransform = GetStartPosition();
            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            player.GetComponent<ShipController>().PlayerName = _playerName;
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        #endregion
    }
}
