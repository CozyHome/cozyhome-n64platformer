using UnityEngine;

namespace com.cozyhome.Singleton
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;
        protected static bool _exists { get; private set; }

        /* UnityEngine */
        void Awake()
        {
            _exists = true;
            _instance = GetComponent<T>();
            OnAwake();
        }

        protected virtual void OnAwake() { }

        protected static void Create(GameObject _self)
        {
            if (_exists)
                return;
            else
            {
                _exists = true;
                _instance = _self.AddComponent<T>();
            }
        }

        protected static T Instance => _instance;
        protected static bool TryGetInstance(out T _i)
        {
            _i = _instance;
            return _exists;
        }
    }
}