using UnityEngine;
using System.Collections;

namespace GameDevelopment2D
{
    public class SpawnManager : MonoBehaviour
    {
		[SerializeField] private float _spawnEnemyInterval = 3.5f;
		[SerializeField] private float _spawnPowerupMinInterval = 7f;
		[SerializeField] private float _spawnPowerupMaxInterval = 11f;
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

				if (enemy.transform.position.x < 0)
					enemy.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 45f));
				else if (enemy.transform.position.x > 0)
					enemy.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, -45f));

				yield return new WaitForSeconds(_spawnEnemyInterval);
			}
		}

		IEnumerator SpawnPowerup()
		{
			yield return new WaitForSeconds(3f);

			while (playerAlive)
			{
				yield return new WaitForSeconds(Random.Range(_spawnPowerupMinInterval, _spawnPowerupMaxInterval));
				Instantiate(_powerups[Random.Range(0, _powerups.Length)].gameObject, new Vector3(Random.Range(-8.35f, 8.35f), 6f, 0), Quaternion.identity);
			}
		}

		internal void OnPlayerDeath()
		{
			playerAlive = false;
		}
    }
}