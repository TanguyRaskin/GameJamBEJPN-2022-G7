using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	int score = 0;
	public Text ScoreLabel;
	static public GameManager Current
	{
		get { return current; }
	}
	static private GameManager current;
	public enum EGameState
	{
		InGame,
		Paused,
		GameOver,
		End
	};
	public EGameState m_CurrentState = EGameState.InGame;

	void Awake()
	{
		current = this;
		Time.timeScale = 1f; //just in case
	}

	#region Score FxEnhancer
	void Start()
	{
		AddToScore(0);
	}
	public void AddToScore(int value)
	{
		score += value;
		ScoreLabel.text = score.ToString();
	}
	#endregion

	private void Update()
	{
		UpdateState();
	}
	void UpdateState()
	{
		switch (m_CurrentState)
		{
			case EGameState.InGame:
				InGame();
				break;
			case EGameState.Paused:
				Pause();
				break;
			case EGameState.GameOver:
				GameOver();
				break;
			case EGameState.End:
				End();
				break;
			default:
				break;
		}
	}

	#region Switch States

	void SwitchState(EGameState gameState)
	{
		if (m_CurrentState == gameState)
			return;

		OnExitState(m_CurrentState);
		m_CurrentState = gameState;
		OnEnterState(m_CurrentState);
	}
	void OnEnterState(EGameState gameState)
	{
		switch (gameState)
		{
			case EGameState.InGame:
				Time.timeScale = 1f;
				break;
			case EGameState.Paused:
				CanvasManager.s_CanvasInstance.SetPause(true);
				Time.timeScale = 0f;
				break;
			case EGameState.GameOver:
				break;
			case EGameState.End:
				break;
			default:
				break;
		}
	}
	void OnExitState(EGameState gameState)
	{
		switch (gameState)
		{
			case EGameState.InGame:
				break;
			case EGameState.Paused:
				CanvasManager.s_CanvasInstance.SetPause(false);
				break;
			case EGameState.GameOver:
				break;
			case EGameState.End:
				break;
			default:
				break;
		}
	}

	#endregion Switch States

	#region State's methods
	void InGame()
	{
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Pause"))
		{
			SwitchState(EGameState.Paused);
		}
	}
	void Pause()
	{
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Pause"))
		{
			SwitchState(EGameState.InGame);
		}
	}
	void GameOver()
	{

	}
	void End()
	{

	}
	#endregion State's methods

	public void Resume()
	{
		SwitchState(EGameState.InGame);
	}
	public void GoToScene(int i)
    {
		SceneManager.LoadScene(i);
    }
	public void Quit()
	{
		Application.Quit();
	}
}
