using UnityEngine;
using System.Collections;
using System;

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
		[SerializeField] private GameObject _scatterShotPrefab;
		

		[SerializeField] private AudioClip[] _audioClips;

		private bool _isTripleShotActive = false;
		private bool _isSpeedActive = false;
		private bool _isShieldActive = false;
		private bool _isAlive = true;
		private bool _inShiftSpeed = false;
		private bool _isScatterShotActive = false;
		private float _fireDelay;
		private float _speedShiftOffset = 1f;
		private float _maxThrusterCharge = 20f;
		private float _currentThrusterCharge;
		private int _shieldStrength;
		private int _ammoCount = 15;

		private Animator _cameraAnim;
		private AudioSource _audioSource;
		private SpawnManager _spawnManager;
		private Coroutine _tripleShotRoutine;
		private Coroutine _scatterShotRoutine;
		private Collider2D _collider;
		private SpriteRenderer _shieldSprite;
		private Vector3 _laserOffset = new Vector3(0, 1f, 0);



		private void Awake()
		{
			_audioSource = GetComponent<AudioSource>();
			_collider = GetComponent<BoxCollider2D>();
			_shieldSprite = _shield.GetComponent<SpriteRenderer>();
			_cameraAnim = Camera.main.GetComponent<Animator>();
		}

		private void Start()
		{
			_lives = 3;
			_shieldStrength = 3;
			_ammoCount = 15;
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

			ToggleShiftSpeed();

			UpdateThrusterCharge();

			if (Input.GetKey(KeyCode.Space))
				SpawnLaser();
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
								StopCoroutine(_tripleShotRoutine);

							_tripleShotRoutine = StartCoroutine(ToggleTripleShotPowerup());
							break;

						case Powerups.Speed:
							StartCoroutine(ToggleSpeedPowerup());
							break;

						case Powerups.Shield:
							ToggleShieldPowerup(true);
							_shieldStrength = 3;
							break;

						case Powerups.Ammo:
							ReloadAmmo();
							break;

						case Powerups.Health:
							TakeHealth();
							break;

						case Powerups.ScatterShot:
							if (_scatterShotRoutine != null)
								StopCoroutine(ToogleScatterShotPowerup());

							_scatterShotRoutine = StartCoroutine(ToogleScatterShotPowerup());
							break;

						default:
							Debug.LogError("NO POWER OF THAT TYPE");
							break;
					}
					Destroy(powerup.gameObject);
				}
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
			if(Time.time > _fireDelay && _ammoCount != 0)
			{
				_ammoCount--;

				CheckAmmoCount();

				_fireDelay = Time.time + _fireRate;

				if(!_isTripleShotActive && !_isScatterShotActive)
					Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);

				else if (_isTripleShotActive)
					Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);

				else if (_isScatterShotActive)
					Instantiate(_scatterShotPrefab, transform.position, Quaternion.identity);

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

			UpdatePlayerEffects();
			
			if (_lives < 1)
			{
				StopAllCoroutines();
				StartCoroutine(PlayerDeathSequence());
				_collider.enabled = false;
			}
		}

		private void UpdatePlayerEffects()
		{
			if(_lives == 3)
			{
				_LeftEngine.SetActive(false);
				_RightEngine.SetActive(false);
			}

			else if (_lives == 2)
			{
				_LeftEngine.SetActive(true);
				_RightEngine.SetActive(false);
			}

			else if (_lives == 1)
			{
				_RightEngine.SetActive(true);
			}
				
		}

		#region Powerups

		private IEnumerator ToggleTripleShotPowerup()
		{
			_isTripleShotActive = true;
			_isScatterShotActive = false;
			yield return new WaitForSeconds(5);
			_isTripleShotActive = false;
		}

		private IEnumerator ToggleSpeedPowerup()
		{
			_isSpeedActive = true;
			yield return new WaitForSeconds(5);
			_isSpeedActive = false;
		}

		private IEnumerator ToogleScatterShotPowerup()
		{
			_isTripleShotActive = false;
			_isScatterShotActive = true;
			yield return new WaitForSeconds(5);
			_isScatterShotActive = false;
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
			if (Input.GetKey(KeyCode.LeftShift) && _currentThrusterCharge > 0)
			{
				_speedShiftOffset = 1.5f;
				_currentThrusterCharge -= 0.05f;

				if (_currentThrusterCharge < 0)
					_currentThrusterCharge = 0;

				UIManager.Instance.UpdateThruster(_currentThrusterCharge);
			}
				
			else if(Input.GetKeyUp(KeyCode.LeftShift))
				_speedShiftOffset = 1;
		}

		private void ReloadAmmo()
		{
			_ammoCount = 15;
			CheckAmmoCount();
		}

		#endregion

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

		private void CheckAmmoCount()
		{
			if (_ammoCount != 0)
				UIManager.Instance.ShowReloadUI(false);
			else
				UIManager.Instance.ShowReloadUI(true);
		}

		private void TakeHealth()
		{
			_lives++;

			if (_lives > 3)
				_lives = 3;

			UIManager.Instance.UpdateLives(_lives);
			UpdatePlayerEffects();
		}

		private void UpdateThrusterCharge()
		{
			_currentThrusterCharge += Time.deltaTime;
			if (_currentThrusterCharge > _maxThrusterCharge)
				_currentThrusterCharge = _maxThrusterCharge;

			UIManager.Instance.UpdateThruster(_currentThrusterCharge);
		}	

		internal void ShakeCamera()
		{
			_cameraAnim.SetTrigger("Shake");
		}
	}
}