using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TicTacToeState{none, cross, circle}

[System.Serializable]
public class WinnerEvent : UnityEvent<int>
{
}

public class TicTacToeAI : MonoBehaviour
{

	int _aiLevel;

	TicTacToeState[,] boardState;

	[SerializeField]
	public bool _isPlayerTurn;

	[SerializeField]
	private int _gridSize = 3;
	
	[SerializeField]
	private TicTacToeState playerState = TicTacToeState.cross;
	TicTacToeState aiState = TicTacToeState.circle;

	[SerializeField]
	private GameObject _xPrefab;

	[SerializeField]
	private GameObject _oPrefab;

	public UnityEvent onGameStarted;

	//Call This event with the player number to denote the winner
	public WinnerEvent onPlayerWin;

	ClickTrigger[,] _triggers;
	
	private void Awake()
	{
		if(onPlayerWin == null){
			onPlayerWin = new WinnerEvent();
		}
	}

    private void Start()
    {
		//Initialize boardState
		boardState = new TicTacToeState[_gridSize, _gridSize];
		//DisplayBoardState();

		//Initialize AI state (cross or circle)
		if (playerState == TicTacToeState.circle)
        {
			aiState = TicTacToeState.cross;
        }
		else if (playerState == TicTacToeState.cross)
        {
			aiState = TicTacToeState.circle;
        }
		else
        {
			aiState = TicTacToeState.none;
        }

		//The player starts
		_isPlayerTurn = true;
    }

    private void Update()
    {
		if (!_isPlayerTurn)
        {
			//Debug.Log("AI turn");
			AiStrategy();

			//Give turn back to human player
			_isPlayerTurn = true;
        }


    }

    public void StartAI(int AILevel){
		_aiLevel = AILevel;
		StartGame();
	}

	public void RegisterTransform(int myCoordX, int myCoordY, ClickTrigger clickTrigger)
	{
		_triggers[myCoordX, myCoordY] = clickTrigger;
	}

	private void StartGame()
	{
		_triggers = new ClickTrigger[3,3];
		onGameStarted.Invoke();
	}

	public void PlayerSelects(int coordX, int coordY){

		SetVisual(coordX, coordY, playerState);
		Debug.Log("Player playing");
		boardState[coordX, coordY] = playerState;
		//DisplayBoardState();
	}

	public void AiSelects(int coordX, int coordY){

		SetVisual(coordX, coordY, aiState);
		Debug.Log("AI playing");
		boardState[coordX, coordY] = aiState;
	}

	private void SetVisual(int coordX, int coordY, TicTacToeState targetState)
	{
		Instantiate(
			targetState == TicTacToeState.circle ? _oPrefab : _xPrefab,
			_triggers[coordX, coordY].transform.position,
			Quaternion.identity
		);
	}

	private void DisplayBoardState()
    {
		for (int i=0; i < _gridSize; i++)
        {
			for (int j = 0; j < _gridSize; j++)
			{
				Debug.Log("boardState[" + i + "," + j + "]=" + boardState[i, j]);
            }
        }

		/*
		foreach (TicTacToeState state in boardState)
        {
			Debug.Log(state);
        }
		*/
    }

	private void AiStrategy()
    {
		// STRATEGY 1: DUMB AI, AI just plays wherever a spot is available
		
		bool hasFoundSpot = false;

		for (int i = 0; i < _gridSize; i++)
		{
			for (int j = 0; j < _gridSize; j++)
			{
				if (boardState[i,j] == TicTacToeState.none && hasFoundSpot == false)
                {
					hasFoundSpot = true;
					AiSelects(i, j);
                }
			}
		}

		DisplayBoardState();
	}
}
