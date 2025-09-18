namespace JCore
{
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        protected static T instance = null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                    instance.Init();
                }

                return instance;
            }
        }

        public void Destroy()
        {
            instance = null;
        }

        public virtual void Enable() { }
        public virtual void Disable() { }
        protected virtual void Init() { }
    }

    public interface ISingleton
    {
        void Destroy();
    }
}
