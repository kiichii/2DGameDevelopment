using UnityEngine;
using System.Collections;

namespace GameDevelopment2D
{
    public class SpawnManager : MonoBehaviour
    {
		[SerializeField] private float _spawnInterval = 5f;
		[SerializeField] private GameObject _enemyPrefab;
		[SerializeField] private Transform _enemyContainer;
		[SerializeField] private Powerup[] _powerups;

		private bool playerAlive = true;



		internal void StartSpawning()
		{
			StartCoroutine(SpawnEnemy());
			StartCoroutine(SpawnPowerup());
		}

		IEnumerator SpawnEnemy()
		{
			yield return new WaitForSeconds(3f);

			while (playerAlive)
			{
				GameObject enemy = Instantiate(_enemyPrefab, new Vector3(Random.Range(-8.1f, 8.1f), 6f, 0), Quaternion.identity);
				enemy.transform.parent = _enemyContainer;
				yield return new WaitForSeconds(_spawnInterval);
			}
		}

		IEnumerator SpawnPowerup()
		{
			yield return new WaitForSeconds(3f);

			while (playerAlive)
			{
				yield return new WaitForSeconds(Random.Range(7, 11));
				Instantiate(_powerups[Random.Range(0, _powerups.Length)].gameObject, new Vector3(Random.Range(-8.35f, 8.35f), 6f, 0), Quaternion.identity);
			}
		}

		internal void OnPlayerDeath()
		{
			playerAlive = false;
		}
    }
}