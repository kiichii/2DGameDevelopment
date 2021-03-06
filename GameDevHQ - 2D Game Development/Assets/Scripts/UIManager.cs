﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameDevelopment2D
{
    public class UIManager : Manager<UIManager>
    {
		[SerializeField] private GameObject _mainMenu;
		[SerializeField] private GameObject _mainGame;
		[SerializeField] private Image _livesDisplay;
		[SerializeField] private Image _thrusterCharge;
		[SerializeField] private Text _scoreText;
		[SerializeField] private Text _gameOverText;
		[SerializeField] private Text _restartText;
		[SerializeField] private Sprite[] _livesSprites;
		[SerializeField] private Text _reloadUI;
		[SerializeField] private Text _ammoCount;

		private int _scoreAmount = 0;



		private void Start()
		{
			_scoreText.text = "Score: " + _scoreAmount;
		}

		internal void AddScore(int score)
		{
			_scoreAmount += score;
			_scoreText.text = "Score:" + _scoreAmount.ToString();
		}

		internal void UpdateLives(int currentLives)
		{
			_livesDisplay.sprite = _livesSprites[currentLives];

			if (currentLives == 0)
			{
				GameOverSequence();
			}
		}

		internal void ShowReloadUI(bool active)
		{
			_reloadUI.gameObject.SetActive(active);
		}

		internal void UpdateThruster(float amount)
		{
			_thrusterCharge.fillAmount = amount / 20;
		}

		internal void UpdateAmmoCount(int amount)
		{
			_ammoCount.text = amount.ToString() + "/15";
		}

		//internal void IncreaseThruster(float amount)
		//{
		//	_thrusterCharge.fillAmount
		//}

		private void GameOverSequence()
		{
			GameManager.Instance.GameOver();
			ShowRestartButton();
			StartCoroutine(GameOverTextFlicker());
		}

		private void ShowRestartButton()
		{
			_restartText.gameObject.SetActive(true);
		}

		private IEnumerator GameOverTextFlicker()
		{
			while (true)
			{
				_gameOverText.gameObject.SetActive(true);
				yield return new WaitForSeconds(0.75f);
				_gameOverText.gameObject.SetActive(false);
				yield return new WaitForSeconds(0.75f);
			}
		}

		
	}
}