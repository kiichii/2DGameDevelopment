using UnityEngine;

namespace GameDevelopment2D
{
    public class Enemy : MonoBehaviour
    {
		[SerializeField] private Animator _Animator;
		[SerializeField] private GameObject _laser;
		[SerializeField] private float m_Speed = 4;
		[SerializeField] private int m_EnemyScore = 10;

		private bool _IsDead = false;
		private float _fireRate = 3f;
		private float _fireDelay;
		private Collider2D _collider2D;
		private AudioSource _audioSource;



		private void Awake()
		{
			_Animator = GetComponent<Animator>();
			_collider2D = GetComponent<Collider2D>();
			_audioSource = GetComponent<AudioSource>();
		}

		private void Update()
		{
			if (!_IsDead)
				transform.Translate(Vector3.down * m_Speed * Time.deltaTime);

			if(transform.position.y < -6.4f)
				transform.position = new Vector3(Random.Range(-8f, 8f), 6, 0);

			if(Time.time > _fireDelay && !_IsDead)
			{
				SpawnLaser();
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if(other.tag == "Player")
			{
				Player player = other.GetComponent<Player>();

				if(player != null)
				{
					player.TakeDamage(1);
					player.ShakeCamera();
					_Animator.SetBool("isEnemyDead", true);
					_IsDead = true;
					_collider2D.enabled = false;
					_audioSource.Play();
					Destroy(gameObject, 2.3f);
				}
				
			}
			else if(other.tag == "Laser")
			{
				Destroy(other.gameObject);
				_Animator.SetBool("isEnemyDead", true);
				_IsDead = true;
				_collider2D.enabled = false;
				UIManager.Instance.AddScore(m_EnemyScore);
				_audioSource.Play();
				Destroy(gameObject, 2.3f);
			}
		}

		private void SpawnLaser()
		{
			Instantiate(_laser, transform.position, Quaternion.identity);
			_fireRate = Random.Range(3f, 6.5f);
			_fireDelay = Time.time + _fireRate;
		}
	}
}