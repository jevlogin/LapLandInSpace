using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    public sealed class SettingsContainer : Singleton<SettingsContainer>
    {
        #region Fields

        [SerializeField] private SpaceShipSettings _spaceShipSettings;

        #endregion


        #region Properties

        public SpaceShipSettings SpaceShipSettings => _spaceShipSettings;

        #endregion
    }
}
