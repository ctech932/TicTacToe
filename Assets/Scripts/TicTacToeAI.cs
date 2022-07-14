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

	private bool isGameOver = false;
	
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
			// Tests if player has won
			testPlayerWon(); 

			// Tests if Tie situation
			if (!isGameOver)
				testTieSituation();

			//AI plays
			if (!isGameOver)
				AiStrategy();

			//Tests if AI has won
			testAIWon();

			// Tests if Tie situation
			if (!isGameOver)
				testTieSituation();

			//Gives turn back to human player
			if (!isGameOver)
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
		if (_aiLevel == 0)
        {
			AiStrategyDumb();
			//DisplayBoardState();
		}


		// STRATEGY 2 : AI first looks for an immediate win. If there isn't, it tries to block the human player.
		if (_aiLevel == 1)
        {
			bool testCanAiWin;
			testCanAiWin = CanAiWin();
			if (testCanAiWin == false)
			{
				AiSemiBlockingStrategy();
			}
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

	private void AiSemiBlockingStrategy()
    {
		int counterHorizontal0 = 0, counterHorizontal1 = 0, counterHorizontal2 = 0;
		int counterVertical0 = 0, counterVertical1 = 0, counterVertical2 = 0;
		int counterDiagTB = 0, counterDiagBT = 0;
		int bestI = -1, finalCounterHorizontal = -1;
		int bestJ = -1, finalCounterVertical = -1;

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

					if (((i == 2) && (j == 0)) || ((i == 1) && (j == 1)) || ((i == 0) && (j == 2)))
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
		else if (counterDiagTB >= finalCounterHorizontal && counterDiagTB >= finalCounterVertical && counterDiagTB >= counterDiagBT)
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

	private bool CanAiWin()
    {
		// Test horizontal lines		
		for (int i = 0; i < _gridSize; i++)
		{
			if (boardState[i,0] == aiState && boardState[i, 1] == aiState)
            {
				if (boardState[i,2] == TicTacToeState.none)
                {
					AiSelects(i, 2);
					return true;
                }
            }
			else if (boardState[i, 0] == aiState && boardState[i, 2] == aiState)
            {
				if (boardState[i, 1] == TicTacToeState.none)
				{
					AiSelects(i, 1);
					return true;
				}
			}
			else if (boardState[i, 1] == aiState && boardState[i, 2] == aiState)
            {
				if (boardState[i, 0] == TicTacToeState.none)
				{
					AiSelects(i, 0);
					return true;
				}
			}
		}

		// Test vertical lines		
		for (int j = 0; j < _gridSize; j++)
		{
			if (boardState[0, j] == aiState && boardState[1, j] == aiState)
			{
				if (boardState[2, j] == TicTacToeState.none)
				{
					AiSelects(2, j);
					return true;
				}
			}
			else if (boardState[0, j] == aiState && boardState[2, j] == aiState)
			{
				if (boardState[1, j] == TicTacToeState.none)
				{
					AiSelects(1, j);
					return true;
				}
			}
			else if (boardState[1, j] == aiState && boardState[2, j] == aiState)
			{
				if (boardState[0, j] == TicTacToeState.none)
				{
					AiSelects(0, j);
					return true;
				}
			}
		}

		//Test horizontal line top-to-bottom
		if (boardState[0, 0] == aiState && boardState[1, 1] == aiState)
        {
			if (boardState[2, 2] == TicTacToeState.none)
            {
				AiSelects(2, 2);
				return true;
            }
        }
		else if (boardState[0, 0] == aiState && boardState[2, 2] == aiState)
        {
			if (boardState[1, 1] == TicTacToeState.none)
			{
				AiSelects(1, 1);
				return true;
			}
		}			
		else if (boardState[1, 1] == aiState && boardState[2, 2] == aiState)
        {
			if (boardState[0, 0] == TicTacToeState.none)
			{
				AiSelects(0, 0);
				return true;
			}
		}

		//Test horizontal line bottom-to-top
		if (boardState[2, 0] == aiState && boardState[1, 1] == aiState)
		{
			if (boardState[0, 2] == TicTacToeState.none)
			{
				AiSelects(0, 2);
				return true;
			}
		}
		else if (boardState[2, 0] == aiState && boardState[0, 2] == aiState)
		{
			if (boardState[1, 1] == TicTacToeState.none)
			{
				AiSelects(1, 1);
				return true;
			}
		}
		else if (boardState[1, 1] == aiState && boardState[0, 2] == aiState)
		{
			if (boardState[2, 0] == TicTacToeState.none)
			{
				AiSelects(2, 0);
				return true;
			}
		}

		return false;
	}

	private bool isAWinner(TicTacToeState testedState)
	{
		if (boardState[0, 0] == testedState && boardState[0, 1] == testedState && boardState[0, 2] == testedState)
        {
			return true;
        }	
		else if (boardState[1, 0] == testedState && boardState[1, 1] == testedState && boardState[1, 2] == testedState)
        {
			return true;
		}
		else if (boardState[2, 0] == testedState && boardState[2, 1] == testedState && boardState[2, 2] == testedState)
        {
			return true;
		}
		else if (boardState[0, 0] == testedState && boardState[1, 0] == testedState && boardState[2, 0] == testedState)
        {
			return true;
		}
		else if (boardState[0, 1] == testedState && boardState[1, 1] == testedState && boardState[2, 1] == testedState)
        {
			return true;
		}
		else if (boardState[0, 2] == testedState && boardState[1, 2] == testedState && boardState[2, 2] == testedState)
        {
			return true;
		}
		else if (boardState[0, 0] == testedState && boardState[1, 1] == testedState && boardState[2, 2] == testedState)
        {
			return true;
		}
		else if (boardState[2, 0] == testedState && boardState[1, 1] == testedState && boardState[0, 2] == testedState)
        {
			return true;
		}
		else
        {
			return false;
        }

	}

	private bool isBoardFull()
    {
		for (int i = 0; i < _gridSize; i++)
		{
			for (int j = 0; j < _gridSize; j++)
			{
				if (boardState[i, j] == TicTacToeState.none)
                {
					return false;
                }
			}
		}

		return true;
	}


	private void testTieSituation()
    {
		if (isBoardFull())
		{
			onPlayerWin.Invoke(-1);
			Debug.Log("Tie situation");
			isGameOver = true;
		}
	}

	private void testPlayerWon()
    {
		if (isAWinner(playerState))
		{
			onPlayerWin.Invoke(0);
			Debug.Log("Player wins");
			isGameOver = true;
		}
	}

	private void testAIWon()
    {
		if (isAWinner(aiState))
		{
			onPlayerWin.Invoke(1);
			Debug.Log("AI wins");
			isGameOver = true;
		}
	}

}
