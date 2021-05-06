using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour {

	public GameObject NormalFieldPrefab;
	public GameObject ClickableFieldPrefab;
	public GameObject StartFieldPrefab;
	public GameObject DiagonallyRightMovePrefab;
	public GameObject DiagonallyLeftMovePrefab;
	public GameObject VerticalMovePrefab;
	public GameObject HorizontalMovePrefab;

	public GameObject VerticalBoundPrefab;
	public GameObject HorizontalBoundPrefab;

	public Transform MapHolder;
	public Transform ClickableMapHolder;
	public Transform MovesHolder;
	GameObject[,] backgroundMap;
	GameObject[,] fieldMap;

	public float xStep;
	public float yStep;

	GameObject move;

	public enum PlayerType
	{
		defaultUser,
		Player,
		Ai
	}

	PlayerType currentType = PlayerType.defaultUser;

	public void GenerateMap (int xSize, int ySize) 
	{
		backgroundMap = new GameObject[ySize, xSize];
		//set starting point, map needs to be on center
		Vector2 fieldPosition = new Vector2((backgroundMap.GetLength(1) * xStep / -2), (backgroundMap.GetLength(0) * Mathf.Abs(yStep) / 2));

		for(int i=0; i< backgroundMap.GetLength(0); i++)
		{
			for (int j = 0; j < backgroundMap.GetLength(1); j++)
			{
				//spawn map
				GameObject field = Instantiate(NormalFieldPrefab, MapHolder);
				field.GetComponent<RectTransform>().anchoredPosition = new Vector3(fieldPosition.x, fieldPosition.y, 0);
				fieldPosition.x += xStep;
				//Destroy field that are not necessary
				if ( (i == 0 || i== backgroundMap.GetLength(0)-1)  && (j != (backgroundMap.GetLength(1) / 2) - 1 && j != backgroundMap.GetLength(1) / 2))
					Destroy(field);
			}
			fieldPosition.x = (backgroundMap.GetLength(1) * xStep / -2);
			fieldPosition.y += yStep;
		}

		GenerateClickableFields(xSize, ySize);
	}

	private void GenerateClickableFields(int xSize, int ySize)
	{
		//generate clickable fields
		fieldMap = new GameObject[ySize + 1, xSize + 1];
		Vector2 fieldPosition = new Vector2((fieldMap.GetLength(1) * xStep / -2), (fieldMap.GetLength(0) * Mathf.Abs(yStep) / 2));
		for (int i = 0; i < fieldMap.GetLength(0); i++)
		{
			for (int j = 0; j < fieldMap.GetLength(1); j++)
			{
				//spawn map
				if (i == fieldMap.GetLength(0) / 2 && j == fieldMap.GetLength(1) / 2)
				{
					//spawn start point
					fieldMap[i, j] = Instantiate(StartFieldPrefab, ClickableMapHolder);
				}
				else
				{
					fieldMap[i, j] = Instantiate(ClickableFieldPrefab, ClickableMapHolder);
				}
				fieldMap[i, j].GetComponent<RectTransform>().anchoredPosition = new Vector3(fieldPosition.x, fieldPosition.y, 0);
				fieldMap[i, j].GetComponent<GameField>().SetPosition(j, i);
				fieldMap[i, j].GetComponent<GameField>().gameManager = GetComponent<GameManager>();
				fieldPosition.x += xStep;

				if ((i == 0 || i == fieldMap.GetLength(0) - 1) && (j != fieldMap.GetLength(1) / 2))
				{
					fieldMap[i, j].SetActive(false);
					fieldMap[i, j].GetComponent<GameField>().isLocked = true;
				}

			}
			fieldPosition.x = (fieldMap.GetLength(1) * xStep / -2);
			fieldPosition.y += yStep;
		}

		//generate boundaries and goals
		for (int i = 1; i < fieldMap.GetLength(0) - 2; i++)
		{
			MoveToPosition(0, i + 1, new Vector2(0, i), true);
			MoveToPosition(fieldMap.GetLength(1) - 1, i + 1, new Vector2(fieldMap.GetLength(1) - 1, i), true);
		}
		MoveToPosition((fieldMap.GetLength(1) / 2) - 1, 0, new Vector2((fieldMap.GetLength(1) / 2) - 1, 1), true);
		MoveToPosition((fieldMap.GetLength(1) / 2) + 1, 0, new Vector2((fieldMap.GetLength(1) / 2) + 1, 1), true);
		MoveToPosition((fieldMap.GetLength(1) / 2) - 1, fieldMap.GetLength(0) - 2, new Vector2((fieldMap.GetLength(1) / 2) - 1, fieldMap.GetLength(0) - 1), true);
		MoveToPosition((fieldMap.GetLength(1) / 2) + 1, fieldMap.GetLength(0) - 2, new Vector2((fieldMap.GetLength(1) / 2) + 1, fieldMap.GetLength(0) - 1), true);
		for (int i = 0; i < fieldMap.GetLength(1) - 1; i++)
		{
			if (i == (fieldMap.GetLength(1) / 2) - 1 || i == (fieldMap.GetLength(1) / 2)) // make space for goals
				continue;
			MoveToPosition(i + 1, 1, new Vector2(i, 1), true);
			MoveToPosition(i + 1, fieldMap.GetLength(0) - 2, new Vector2(i, fieldMap.GetLength(0) - 2), true);
		}
	}

	public Vector2 GetCenter()
	{
		return new Vector2(fieldMap.GetLength(1) / 2, fieldMap.GetLength(0) / 2);
	}

	public bool MoveToPosition(int x, int y, Vector2 currentPosition, bool isGeneratingBoundaries = false, bool isCheckingForDraw = false)
	{
		bool retVal = false;
		int diffX = (int)currentPosition.x - x;
		int diffY = (int)currentPosition.y - y;
		if (x >= fieldMap.GetLength(1) || x < 0 || y >= fieldMap.GetLength(0) || y < 0) //out of bound so cant move there
		{
			return false;
		}
		if (fieldMap[y,x].GetComponent<GameField>().isLocked == true)
		{
			return false;
		}
		if(diffX == 1 && diffY == 1) // moving top left
		{
			if(fieldMap[(int)currentPosition.y, (int)currentPosition.x].GetComponent<GameField>().moves[0] == false &&
				fieldMap[y, x].GetComponent<GameField>().moves[4] == false)
			{
				if(isCheckingForDraw == false)
				{
					fieldMap[(int)currentPosition.y, (int)currentPosition.x].GetComponent<GameField>().moves[0] = true;
					fieldMap[y, x].GetComponent<GameField>().moves[4] = true;
				}
				retVal = true;
			}
		}
		else if (diffX == 0 && diffY == 1) // moving top
		{
			if (fieldMap[(int)currentPosition.y, (int)currentPosition.x].GetComponent<GameField>().moves[1] == false &&
				fieldMap[y, x].GetComponent<GameField>().moves[5] == false)
			{
				if (isCheckingForDraw == false)
				{
					fieldMap[(int)currentPosition.y, (int)currentPosition.x].GetComponent<GameField>().moves[1] = true;
					fieldMap[y, x].GetComponent<GameField>().moves[5] = true;
				}
				retVal = true;
			}
		}
		else if (diffX == -1 && diffY == 1) // moving top right
		{
			if (fieldMap[(int)currentPosition.y, (int)currentPosition.x].GetComponent<GameField>().moves[2] == false &&
				fieldMap[y, x].GetComponent<GameField>().moves[6] == false)
			{
				if (isCheckingForDraw == false)
				{
					fieldMap[(int)currentPosition.y, (int)currentPosition.x].GetComponent<GameField>().moves[2] = true;
					fieldMap[y, x].GetComponent<GameField>().moves[6] = true;
				}
				retVal = true;
			}
		}
		else if (diffX == -1 && diffY == 0) // moving right
		{
			if (fieldMap[(int)currentPosition.y, (int)currentPosition.x].GetComponent<GameField>().moves[3] == false &&
				fieldMap[y, x].GetComponent<GameField>().moves[7] == false)
			{
				if (isCheckingForDraw == false)
				{
					fieldMap[(int)currentPosition.y, (int)currentPosition.x].GetComponent<GameField>().moves[3] = true;
					fieldMap[y, x].GetComponent<GameField>().moves[7] = true;
				}
				retVal = true;
			}
		}
		else if (diffX == -1 && diffY == -1) // moving bottom right
		{
			if (fieldMap[(int)currentPosition.y, (int)currentPosition.x].GetComponent<GameField>().moves[4] == false &&
				fieldMap[y, x].GetComponent<GameField>().moves[0] == false)
			{
				if (isCheckingForDraw == false)
				{
					fieldMap[(int)currentPosition.y, (int)currentPosition.x].GetComponent<GameField>().moves[4] = true;
					fieldMap[y, x].GetComponent<GameField>().moves[0] = true;
				}
				retVal = true;
			}
		}
		else if (diffX == 0 && diffY == -1) // moving bottom
		{
			if (fieldMap[(int)currentPosition.y, (int)currentPosition.x].GetComponent<GameField>().moves[5] == false &&
				fieldMap[y, x].GetComponent<GameField>().moves[1] == false)
			{
				if (isCheckingForDraw == false)
				{
					fieldMap[(int)currentPosition.y, (int)currentPosition.x].GetComponent<GameField>().moves[5] = true;
					fieldMap[y, x].GetComponent<GameField>().moves[1] = true;
				}
				retVal = true;
			}
		}
		else if (diffX == 1 && diffY == -1) // moving bottom left
		{
			if (fieldMap[(int)currentPosition.y, (int)currentPosition.x].GetComponent<GameField>().moves[6] == false &&
				fieldMap[y, x].GetComponent<GameField>().moves[2] == false)
			{
				if (isCheckingForDraw == false)
				{
					fieldMap[(int)currentPosition.y, (int)currentPosition.x].GetComponent<GameField>().moves[6] = true;
					fieldMap[y, x].GetComponent<GameField>().moves[2] = true;
				}
				retVal = true;
			}
		}
		else if (diffX == 1 && diffY == 0) // moving left
		{
			if (fieldMap[(int)currentPosition.y, (int)currentPosition.x].GetComponent<GameField>().moves[7] == false &&
				fieldMap[y, x].GetComponent<GameField>().moves[3] == false)
			{
				if (isCheckingForDraw == false)
				{
					fieldMap[(int)currentPosition.y, (int)currentPosition.x].GetComponent<GameField>().moves[7] = true;
					fieldMap[y, x].GetComponent<GameField>().moves[3] = true;
				}
				retVal = true;
			}
		}
		if(retVal && isCheckingForDraw == false)
		{
			DrawLine((int)currentPosition.x, (int)currentPosition.y, x, y, isGeneratingBoundaries);
		}
		return retVal;
	}

	public void DrawLine(int curX, int curY, int targetX, int targetY, bool isGeneratingBoundaries = false)
	{
		int diffX = curX - targetX;
		int diffY = curY - targetY;

		if((diffX == 1 && diffY == -1) || (diffX == -1 && diffY == 1) ) //diagonally right
		{
			move = Instantiate(DiagonallyRightMovePrefab, MovesHolder);
			if (diffX == 1 && diffY == -1)
				move.GetComponent<RectTransform>().anchoredPosition = new Vector3((backgroundMap.GetLength(1) * xStep / -2) + ((targetX)*xStep), (backgroundMap.GetLength(0) * Mathf.Abs(yStep) / 2) + ((targetY-1) * yStep), 0);
			if (diffX == -1 && diffY == 1)
				move.GetComponent<RectTransform>().anchoredPosition = new Vector3((backgroundMap.GetLength(1) * xStep / -2) + ((targetX-1) * xStep), (backgroundMap.GetLength(0) * Mathf.Abs(yStep) / 2) + ((targetY) * yStep), 0);
		}
		else if ((diffX == 1 && diffY == 1) || (diffX == -1 && diffY == -1)) //diagonally left
		{
			move = Instantiate(DiagonallyLeftMovePrefab, MovesHolder);
			if (diffX == 1 && diffY == 1)
				move.GetComponent<RectTransform>().anchoredPosition = new Vector3((backgroundMap.GetLength(1) * xStep / -2) + ((targetX) * xStep), (backgroundMap.GetLength(0) * Mathf.Abs(yStep) / 2) + ((targetY) * yStep), 0);
			if (diffX == -1 && diffY == -1)
				move.GetComponent<RectTransform>().anchoredPosition = new Vector3((backgroundMap.GetLength(1) * xStep / -2) + ((targetX-1) * xStep), (backgroundMap.GetLength(0) * Mathf.Abs(yStep) / 2) + ((targetY-1) * yStep), 0);
		}
		else if(diffX == 0 && Mathf.Abs(diffY) == 1) // vertical
		{
			if(isGeneratingBoundaries)
				move = Instantiate(VerticalBoundPrefab, MovesHolder);
			else
				move = Instantiate(VerticalMovePrefab, MovesHolder);
			if (diffY == 1)
				move.GetComponent<RectTransform>().anchoredPosition = new Vector3((fieldMap.GetLength(1) * xStep / -2) + ((targetX) * xStep), (fieldMap.GetLength(0) * Mathf.Abs(yStep) / 2) + ((targetY) * yStep) + (yStep / 2), 0);
			if (diffY == -1)
				move.GetComponent<RectTransform>().anchoredPosition = new Vector3((fieldMap.GetLength(1) * xStep / -2) + ((targetX) * xStep), (fieldMap.GetLength(0) * Mathf.Abs(yStep) / 2) + ((targetY) * yStep) - (yStep/2), 0);
		}
		else if (Mathf.Abs(diffX) == 1 && diffY == 0) // horizontal
		{
			if (isGeneratingBoundaries)
				move = Instantiate(HorizontalBoundPrefab, MovesHolder);
			else
				move = Instantiate(HorizontalMovePrefab, MovesHolder);
			if (diffX == 1)
				move.GetComponent<RectTransform>().anchoredPosition = new Vector3((fieldMap.GetLength(1) * xStep / -2) + ((targetX) * xStep) + (xStep / 2), (fieldMap.GetLength(0) * Mathf.Abs(yStep) / 2) + ((targetY) * yStep), 0);
			if (diffX == -1)
				move.GetComponent<RectTransform>().anchoredPosition = new Vector3((fieldMap.GetLength(1) * xStep / -2) + ((targetX) * xStep) - (xStep / 2), (fieldMap.GetLength(0) * Mathf.Abs(yStep) / 2) + ((targetY) * yStep), 0);
		}
		if(currentType == PlayerType.Player)
		{
			if(move.GetComponent<Image>() != null)
				move.GetComponent<Image>().color = Color.red;
		}
		else if(currentType == PlayerType.Ai)
		{
			if (move.GetComponent<Image>() != null)
				move.GetComponent<Image>().color = Color.black;
		}

	}

	public void SetCurrentPlayerType(PlayerType type)
	{
		currentType = type;
	}

	public bool CheckIsBounceable(int x, int y)
	{
		return fieldMap[y, x].GetComponent<GameField>().IsBouncable();
	}

	public Vector2 ReturnPlayerGoalPosition()
	{
		return new Vector2(fieldMap.GetLength(1) / 2, fieldMap.GetLength(0)-1);
	}
	public Vector2 ReturnAiGoalPosition()
	{
		return new Vector2(fieldMap.GetLength(1) / 2, 0);
	}
}
