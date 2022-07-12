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

	public TicTacToeState[,] boardState;

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

	private int numberOfPlayerTurns = 0;
	
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

			//Update count of human players turns
			numberOfPlayerTurns += 1;

			//AI plays
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
		/*
		AiStrategyDumb();		

		//DisplayBoardState();
		*/

		// STRATEGY 2
		int counterHorizontal0 = 0, counterHorizontal1 = 0, counterHorizontal2 = 0;
		int counterVertical0 = 0, counterVertical1 = 0, counterVertical2 = 0;
		int counterDiagTB = 0, counterDiagBT = 0;
		int bestI = -1, finalCounterHorizontal = -1;
		int bestJ = -1, finalCounterVertical = -1;
		int finalCounterDiag = -1;
		for (int i = 0; i < _gridSize; i++)
		{
			for (int j = 0; j < _gridSize; j++)
			{
				if (boardState[i, j] == playerState)
				{
					if (i == 0) { counterHorizontal0 += 1; }
					else if (i == 1) { counterHorizontal1 += 1; }
					else if (i == 2) { counterHorizontal2 += 1; }

					if (j == 0) { counterVertical0 += 1; }
					else if (j == 1) { counterVertical1 += 1; }
					else if (j == 2) { counterVertical2 += 1; }

					if (i == j) { counterDiagTB += 1; }

					if ( ((i==2)&&(j==0)) || ((i == 1)&&(j == 1)) || ((i == 0)&&(j == 2)))
                    {
						counterDiagBT += 1;
                    }

				}
			}
		}

		if (counterHorizontal0 >= counterHorizontal1 && counterHorizontal0 >= counterHorizontal2)
        {
			bestI = 0;
			finalCounterHorizontal = counterHorizontal0;
        }
		else if (counterHorizontal1 >= counterHorizontal0 && counterHorizontal1 >= counterHorizontal2)
        {
			bestI = 1;
			finalCounterHorizontal = counterHorizontal1;
        }
		else if (counterHorizontal2 >= counterHorizontal0 && counterHorizontal2 >= counterHorizontal1)
        {
			bestI = 2;
			finalCounterHorizontal = counterHorizontal2;
        }

		if (counterVertical0 >= counterVertical1 && counterVertical0 >= counterVertical2)
		{
			bestJ = 0;
			finalCounterVertical = counterVertical0;
		}
		else if (counterVertical1 >= counterVertical0 && counterVertical1 >= counterVertical2)
		{
			bestJ = 1;
			finalCounterVertical = counterVertical1;
		}
		else if (counterVertical2 >= counterVertical0 && counterVertical2 >= counterVertical1)
		{
			bestJ = 2;
			finalCounterVertical = counterVertical2;
		}


		if (bestI == -1 && bestJ == -1)
        {
			AiStrategyDumb();
        }
		else if (counterDiagTB >= finalCounterHorizontal && counterDiagTB >= finalCounterVertical && counterDiagTB >= counterDiagBT )
        {
			if (boardState[0, 0] == TicTacToeState.none)
			{
				AiSelects(0, 0);
			}
			else if (boardState[1, 1] == TicTacToeState.none)
            {
				AiSelects(1, 1);
            }
			else if (boardState[2, 2] == TicTacToeState.none)
            {
				AiSelects(2, 2);
            }
			else
            {
				AiStrategyDumb();
			}						
		}
		else if (counterDiagBT >= finalCounterHorizontal && counterDiagBT >= finalCounterVertical && counterDiagBT >= counterDiagTB)
        {
			if (boardState[2, 0] == TicTacToeState.none)
			{
				AiSelects(2, 0);
			}
			else if (boardState[1, 1] == TicTacToeState.none)
			{
				AiSelects(1, 1);
			}
			else if (boardState[0, 2] == TicTacToeState.none)
			{
				AiSelects(0, 2);
			}
			else
			{
				AiStrategyDumb();
			}
		}
		else if (finalCounterHorizontal >= finalCounterVertical)
        {
			if (boardState[bestI, 0] == TicTacToeState.none)
            {
				AiSelects(bestI, 0);
            }
			else if (boardState[bestI, 1] == TicTacToeState.none)
            {
				AiSelects(bestI, 1);
            }
			else if (boardState[bestI, 2] == TicTacToeState.none)
            {
				AiSelects(bestI, 2);
            }
            else
            {
				AiStrategyDumb();
            }
        }
		else if (finalCounterVertical >= finalCounterHorizontal)
        {
			if (boardState[0, bestJ] == TicTacToeState.none)
			{
				AiSelects(0, bestJ);
			}
			else if (boardState[1, bestJ] == TicTacToeState.none)
			{
				AiSelects(1, bestJ);
			}
			else if (boardState[2, bestJ] == TicTacToeState.none)
			{
				AiSelects(2, bestJ);
			}
			else
			{
				AiStrategyDumb();
			}
		}
		else
        {
			AiStrategyDumb();
        }

	}

	private void AiStrategyDumb()
    {
		bool hasFoundSpot = false;

		for (int i = 0; i < _gridSize; i++)
		{
			for (int j = 0; j < _gridSize; j++)
			{
				if (boardState[i, j] == TicTacToeState.none && hasFoundSpot == false)
				{
					hasFoundSpot = true;
					AiSelects(i, j);
				}
			}
		}
	}

}
