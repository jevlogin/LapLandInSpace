using UnityEngine;


namespace WORLDGAMEDEVELOPMENT
{
    public class Singleton<T> : MonoBehaviour where T : Object
    {
        #region Singletoon

        private static T _staticInstance;

        public static T Instance
        {
            get
            {
                if (_staticInstance != null)
                {
                    return _staticInstance;
                }
                _staticInstance = FindObjectOfType<T>();
                if (_staticInstance is null)
                {
                    Debug.LogError("An instance of " + typeof(T) + " is needed in the scene, but there is none.");
                }
                return _staticInstance;
            }
        } 

        #endregion
    } 
}
