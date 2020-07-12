using UnityEngine;
using System.Collections;

namespace GameDevelopment2D
{
    public class Player : MonoBehaviour
    {
		[SerializeField] private float _speed;
		[SerializeField] private float _speedPowerupOffset;
		
		[SerializeField] private float _fireRate;
		[SerializeField] private int _lives;
		[SerializeField] private GameObject _laserPrefab;
		[SerializeField] private GameObject _tripleShotPrefab;
		[SerializeField] private GameObject _shield;
		[SerializeField] private GameObject _LeftEngine;
		[SerializeField] private GameObject _RightEngine;
		[SerializeField] private GameObject _playerVFX;
		[SerializeField] private GameObject _explosion;

		[SerializeField] private AudioClip[] _audioClips;

		private bool _isTripleShotActive = false;
		private bool _isSpeedActive = false;
		private bool _isShieldActive = false;
		private bool _isAlive = true;
		private bool _inShiftSpeed = false;
		private float _fireDelay;
		private float _speedShiftOffset = 1f;
		private int _shieldStrength;
		private SpriteRenderer _shieldSprite;

		private AudioSource _audioSource;
		private SpawnManager _spawnManager;
		private Coroutine _tripleShotRoutine;
		private Collider2D _collider;
		private Vector3 _laserOffset = new Vector3(0, 1f, 0);



		private void Awake()
		{
			_audioSource = GetComponent<AudioSource>();
			_collider = GetComponent<BoxCollider2D>();
			_shieldSprite = _shield.GetComponent<SpriteRenderer>();
		}

		private void Start()
		{
			_lives = 3;
			_shieldStrength = 3;
			transform.position = new Vector3(0, -3f, 0);
			_spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

			if(_spawnManager == null)
				Debug.LogError("No SpawnManager in the game");

			if (_audioSource == null)
				Debug.LogError("No AudioSource in the player");
			else
			{
				_audioSource.clip = _audioClips[0];
			}
		}

		private void Update()
		{
			CalculateMovement();

			if (Input.GetKey(KeyCode.Space))
				SpawnLaser();

			ToggleShiftSpeed();
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.tag == "Powerup")
			{
				Powerup powerup = other.GetComponent<Powerup>();

				if (powerup != null)
				{
					//This is better if the powerup itself has the audioclip, just transfer the OnTrigger in the Powerup.cs
					AudioSource.PlayClipAtPoint(_audioClips[_audioClips.Length - 1], powerup.transform.position);

					switch (powerup.PowerType)
					{
						case Powerups.TripleShot:
							if (_tripleShotRoutine != null)
							{
								StopCoroutine(_tripleShotRoutine);
							}
							_tripleShotRoutine = StartCoroutine(ToggleTripleShotPowerup());
							break;

						case Powerups.Speed:
							StartCoroutine(ToggleSpeedPowerup());
							break;

						case Powerups.Shield:
							ToggleShieldPowerup(true);
							_shieldStrength = 3;
							break;

						default:
							Debug.LogError("NO POWER OF THAT TYPE");
							break;
					}
					Destroy(powerup.gameObject);
				}
			}
			else if (other.tag == "Enemy")
			{
				//Remove, produce sound

			}
		}

		private void CalculateMovement()
		{
			float horizontal = Input.GetAxis("Horizontal");
			float vertical = Input.GetAxis("Vertical");

			Vector3 position = new Vector3(horizontal, vertical, 0);


			if (_isAlive)
			{
				if (!_isSpeedActive)
					transform.Translate(position * _speed * _speedShiftOffset * Time.deltaTime);

				else
					transform.Translate(position * (_speed + _speedPowerupOffset) * _speedShiftOffset * Time.deltaTime);
			}
			
			transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -5f, 0), 0);

			if (transform.position.x > 9.7f)
				transform.position = new Vector3(-9.7f, transform.position.y, 0);
			else if (transform.position.x <= -9.7f)
				transform.position = new Vector3(9.7f, transform.position.y, 0);
		}

		private void SpawnLaser()
		{
			if(Time.time > _fireDelay)
			{
				_fireDelay = Time.time + _fireRate;

				if (!_isTripleShotActive)
					Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);

				else
					Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);

				_audioSource.Play();
			}
		}

		internal void TakeDamage(int damage)
		{
			if (_isShieldActive)
			{
				ToggleShieldStrength(damage);
				return;
			}
				
			_lives -= damage;
			UIManager.Instance.UpdateLives(_lives);

			if (_lives == 2)
				_LeftEngine.SetActive(true);

			else if (_lives == 1)
				_RightEngine.SetActive(true);

			if (_lives < 1)
			{
				StopAllCoroutines();
				StartCoroutine(PlayerDeathSequence());
				_collider.enabled = false;
			}
		}

		private IEnumerator ToggleTripleShotPowerup()
		{
			_isTripleShotActive = true;
			yield return new WaitForSeconds(5);
			_isTripleShotActive = false;
		}

		private IEnumerator ToggleSpeedPowerup()
		{
			_isSpeedActive = true;
			yield return new WaitForSeconds(5);
			_isSpeedActive = false;
		}

		private void ToggleShieldStrength(int damage)
		{
			_shieldStrength -= damage;
			if (_shieldStrength == 2)
				//255, 174, 174
				_shieldSprite.color = new Color(1, 0.68f, 0.68f);
			
			else if (_shieldStrength == 1)
				//255, 83, 83
				_shieldSprite.color = new Color(1, 0.33f, 0.33f);

			else if(_shieldStrength == 0)
				ToggleShieldPowerup(false);
		}

		private void ToggleShieldPowerup(bool active)
		{
			_isShieldActive = active;
			_shield.gameObject.SetActive(active);

			_shieldSprite.color = new Color(1, 1, 1);
			
		}

		private void ToggleShiftSpeed()
		{
			if (Input.GetKey(KeyCode.LeftShift))
				_speedShiftOffset = 1.5f;

			else if(Input.GetKeyUp(KeyCode.LeftShift))
				_speedShiftOffset = 1;
		}

		private IEnumerator PlayerDeathSequence()
		{
			_isAlive = false;
			Instantiate(_explosion, transform.position, Quaternion.identity);

			_spawnManager.OnPlayerDeath();
			_playerVFX.SetActive(false);
			
			_audioSource.Stop();
			_audioSource.clip = _audioClips[1];
			_audioSource.Play();
			yield return new WaitForSeconds(3.5f);
			Destroy(gameObject);
		}
	}
}