using UnityEngine;

namespace GameDevelopment2D
{
    public class Asteroid : MonoBehaviour
    {
		[SerializeField] private float m_RotationSpeed = 1f;
		[SerializeField] private GameObject m_ExplosionPrefab;
		[SerializeField] private SpawnManager m_SpawnManager;



		private void Update()
		{
			transform.Rotate(Vector3.forward * m_RotationSpeed * Time.deltaTime);
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if(other.tag == "Laser")
			{
				Instantiate(m_ExplosionPrefab, transform.position, Quaternion.identity);
				m_SpawnManager.StartSpawning();
				Destroy(other.gameObject);
				Destroy(this.gameObject);
			}
		}
	}
}