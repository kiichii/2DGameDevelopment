using UnityEngine;

namespace OneLastTry
{
    public class LaserParent : MonoBehaviour
    {
		//TODO
		private void Update()
		{
			if (transform.childCount < 1)
				Destroy(this.gameObject);
		}
	}
}