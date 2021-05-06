using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameField : MonoBehaviour, IPointerClickHandler {

	//0-top-left, 1-top, 2-top-right, 3-right, 4-bottom-right, 5-bottom, 6-bottom-left, 7-left
	public bool[] moves = new bool[8];

	int xPos;
	int yPos;

	public bool isLocked;

	public GameManager gameManager;

	public void OnPointerClick(PointerEventData eventData)
	{
		if(gameManager.CheckIsPlayerTurn())
		{
			gameManager.MoveToPosition(xPos, yPos);
		}
	}

	public bool IsBouncable()
	{
		bool retVal = false;
		foreach (bool m in moves)
		{
			if (m == true)
			{
				retVal = true;
				break;
			}
		}
		return retVal;
	}

	public void SetPosition(int x, int y)
	{
		xPos = x;
		yPos = y;
	}
}
