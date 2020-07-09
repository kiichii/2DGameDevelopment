using UnityEngine;

namespace GameDevelopment2D
{
    public abstract class Manager<T> : MonoBehaviour where T : MonoBehaviour
    {
		private static T _instance;

		public static T Instance
		{
			get
			{
				return _instance;
			}
			set
			{
				//If there are no instance yet of the object
				//Set the instance.gameObject to DontDestroy
				if(_instance == null)
				{
					_instance = value;
					//DontDestroyOnLoad(_instance.gameObject);
				}

				//If there is already an instance of the object, destroy all incoming instances of that object
				else if (_instance != value)
				{
					Destroy(value.gameObject);
				}
			}
		}

		protected virtual void Awake()
		{
			//We wanna make sure that we are going through our accessor
			Instance = this as T;
		}
	}
}