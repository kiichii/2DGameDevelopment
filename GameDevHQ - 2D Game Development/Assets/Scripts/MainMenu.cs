using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace GameDevelopment2D
{
    public class MainMenu : MonoBehaviour
    {
		public void StartGame()
		{
			SceneManager.LoadScene("MainGame");
		}
    }
}