using UnityEngine;

namespace OneLastTry
{
    public class Explosion : MonoBehaviour
    {
		//TODO
		private void Start()
		{
			Destroy(this.gameObject, 3f);
		}
	}
}