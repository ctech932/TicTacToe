using System;
using System.Collections.Generic;
using UnityEngine;

public class ClickTrigger : MonoBehaviour
{
	TicTacToeAI _ai;

	[SerializeField]
	private int _myCoordX = 0;
	[SerializeField]
	private int _myCoordY = 0;

	[SerializeField]
	private bool canClick;

	private void Awake()
	{
		_ai = FindObjectOfType<TicTacToeAI>();
	}

	private void Start(){

		_ai.onGameStarted.AddListener(AddReference);
		_ai.onGameStarted.AddListener(() => SetInputEndabled(true));
		_ai.onPlayerWin.AddListener((win) => SetInputEndabled(false));
	}

    private void SetInputEndabled(bool val){
		canClick = val;
	}

	private void AddReference()
	{
		_ai.RegisterTransform(_myCoordX, _myCoordY, this);
		canClick = true;
	}

	private void OnMouseDown()
	{
		if (_ai.boardState[_myCoordX, _myCoordY] == TicTacToeState.none)
        {
			if (canClick && _ai._isPlayerTurn == true)
			{
				_ai.PlayerSelects(_myCoordX, _myCoordY);

				//Once clicked, can't be clicked anymore (redundant though..)
				canClick = false;
				//Debug.Log(_myCoordX);
				//Debug.Log(_myCoordY);

				//Give turn to AI player
				_ai._isPlayerTurn = false;
				_ai._hasAIPlayed = false;
			}
		}

	}
}
