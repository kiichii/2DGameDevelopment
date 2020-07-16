using UnityEngine;

namespace GameDevelopment2D
{
    public class Laser : MonoBehaviour
    {
		[SerializeField] private float _speed = 8;



		private void Update()
		{
			transform.Translate(Vector3.up * _speed * Time.deltaTime);

			if (transform.position.y > 12)
			{
				if(transform.parent != null)
				{
					Destroy(transform.parent.gameObject);
				}
				Destroy(gameObject);
			}
		}
	}
}