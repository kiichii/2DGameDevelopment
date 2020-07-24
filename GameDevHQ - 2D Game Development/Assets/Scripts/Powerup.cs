using UnityEngine;

namespace GameDevelopment2D
{
	//Create new powerup
	//Create new projectile
	//Spawn rarely


	public enum Powerups
	{
		TripleShot, Shield, Speed, Ammo, Health, ScatterShot, ReduceAmmo
	}

	public class Powerup : MonoBehaviour
    {
		[SerializeField] private float _speed = 1f;

		[SerializeField] private Powerups _powerup;

		public Powerups PowerType { get => _powerup; }



		private void Update()
		{
			transform.Translate(Vector3.down * _speed * Time.deltaTime);

			if(transform.position.y < -6f)
			{
				Destroy(gameObject);
			}
		}
	}
}