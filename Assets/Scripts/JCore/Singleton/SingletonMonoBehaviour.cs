using UnityEngine;

namespace JCore
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour, ISingleton where T : SingletonMonoBehaviour<T>
    {
        protected static T instance;

        protected static bool isEnabled = true;

        public static T Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindObjectOfType(typeof(T)) as T;
                    if (!instance)
                    {
                        Debug.Log("There needs to be one active SingletonMonoBehaviour script on a GameObject in your scene. (" + typeof(T) + ")");
                    }
                    else
                    {
                        isEnabled = instance.gameObject.activeInHierarchy;
                        instance.Init();
                    }
                }
                return instance;
            }
        }

        public void Destroy()
        {
            GameObject.Destroy(instance.gameObject);
            instance = null;
        }

        public void Enable()
        {
            isEnabled = true;
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            isEnabled = false;
            gameObject.SetActive(false);
        }

        protected virtual void Init() { }
    }
}