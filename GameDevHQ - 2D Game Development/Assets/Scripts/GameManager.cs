using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameDevelopment2D
{
    public class GameManager : Manager<GameManager>
	{ 
		private bool _isGameOver = false;



		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.R) && _isGameOver)
			{
				SceneManager.LoadScene("MainGame");
			}

			if (Input.GetKeyDown(KeyCode.Escape))
				Application.Quit();
		}

		internal void GameOver()
		{
			_isGameOver = true;
		}
	}
}