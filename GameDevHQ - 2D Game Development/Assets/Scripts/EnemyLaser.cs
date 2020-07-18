using UnityEngine;

namespace GameDevelopment2D
{
    public class EnemyLaser : MonoBehaviour
    {
		[SerializeField] private float _speed = 1f;
		[SerializeField] private AudioClip[] _laserAudio;



		private void Start()
		{
			AudioSource.PlayClipAtPoint(_laserAudio[0], transform.position);
		}

		private void Update()
		{
			transform.Translate(Vector3.down * _speed * Time.deltaTime);

			if (transform.position.y < -8.4f)
				Destroy(this.gameObject);
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if(collision.tag == "Player")
			{
				Player player = collision.GetComponent<Player>();

				if(player != null)
				{
					AudioSource.PlayClipAtPoint(_laserAudio[1], transform.position);

					player.TakeDamage(1);
					player.ShakeCamera();
					Destroy(this.gameObject);
				}
			}
		}
	}
}