using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	bool isPlayerTurn = true;
	MapGenerator mapGenerator;
	Vector2 currentPosition;
	public Text TurnInfo;
	public Text GamesWonLostStats;

	int numOfGamesWon = 0;
	int numOfGamesLost = 0;

	List<AiMove> aiMoves = new List<AiMove> {
			new AiMove { stepX = -1, stepY = -1 },
			new AiMove { stepX = 0, stepY = -1 },
			new AiMove { stepX = 1, stepY = -1 },
			new AiMove { stepX = 1, stepY = 0 },
			new AiMove { stepX = 1, stepY = 1 },
			new AiMove { stepX = 0, stepY = 1 },
			new AiMove { stepX = -1, stepY = 1 },
			new AiMove { stepX = -1, stepY = 0 }
		};

	// Use this for initialization
	void Start()
	{
		if (PlayerPrefs.HasKey("GamesWon"))
			numOfGamesWon = PlayerPrefs.GetInt("GamesWon");
		if (PlayerPrefs.HasKey("GamesLost"))
			numOfGamesLost = PlayerPrefs.GetInt("GamesLost");
		UpdateUiGamesWonLost();
	}
	public void StartGame(int xSize, int ySize) 
	{
		mapGenerator = GetComponent<MapGenerator>();
		mapGenerator.GenerateMap(xSize, ySize);
		currentPosition = mapGenerator.GetCenter();
		Random.InitState(System.DateTime.Now.Millisecond);
		int rnd = Random.Range(0, 2);
		isPlayerTurn = (rnd == 1);
		UpdateUiTurn();
		if (isPlayerTurn == false)
			AiMovePC();
	}
	
	void UpdateUiGamesWonLost()
	{
		GamesWonLostStats.text = "Games won: " + numOfGamesWon + ", games lost: " + numOfGamesLost;
	}

	public bool CheckIsPlayerTurn()
	{
		return isPlayerTurn;
	}

	public bool MoveToPosition(int x, int y)
	{
		bool retVal = false;
		int diffX = Mathf.Abs((int)currentPosition.x - x);
		int diffY = Mathf.Abs((int)currentPosition.y - y);
		bool isBounceable = mapGenerator.CheckIsBounceable(x, y);
		if (diffX <= 1 && diffY <=1 )
		{
			mapGenerator.SetCurrentPlayerType(isPlayerTurn ? MapGenerator.PlayerType.Player : MapGenerator.PlayerType.Ai);
			if (mapGenerator.MoveToPosition(x, y, currentPosition))
			{
				currentPosition = new Vector2(x, y); // after move set new current position
				retVal = true;
				if(CheckForGoalPlayer())
				{
					WinGame();
				}
				else if(CheckForGoalAi())
				{
					LoseGame();
				}
				else if(CheckForDraw())
				{
					DrawGame();
				}
				else
				{
					if (isBounceable == false) // if we can't bounce then switch current player
					{
						isPlayerTurn = !isPlayerTurn;
						UpdateUiTurn();
					}
					if (isPlayerTurn == false)
					{
						AiMovePC();
					}
				}
			}
		}

		return retVal;
	}

	private void UpdateUiTurn()
	{
		if (isPlayerTurn)
		{
			TurnInfo.text = "Player turn";
		}
		else
		{
			TurnInfo.text = "Ai turn";
		}
	}

	private bool CheckForGoalPlayer()
	{
		bool isGoal = false;
		Vector2 aiGoalPosition = mapGenerator.ReturnAiGoalPosition();
		if (currentPosition == aiGoalPosition)
			isGoal = true;

		return isGoal;
	}

	private bool CheckForGoalAi()
	{
		bool isGoal = false;
		Vector2 playerGoalPosition = mapGenerator.ReturnPlayerGoalPosition();
		if (currentPosition == playerGoalPosition)
			isGoal = true;

		return isGoal;
	}

	private bool CheckForDraw()
	{
		bool isDraw = true;
		foreach(AiMove aim in aiMoves) // i will use that list with possible moves from ai
		{
			if(mapGenerator.MoveToPosition((int)currentPosition.x + aim.stepX, (int)currentPosition.y + aim.stepY, currentPosition, false, true) )
			{
				//at least one possible move
				isDraw = false;
				break;
			}
		}
		return isDraw;
	}

	private void WinGame()
	{
		TurnInfo.text = "Great job! Player won";
		numOfGamesWon++;
		PlayerPrefs.SetInt("GamesWon", numOfGamesWon);
		UpdateUiGamesWonLost();
	}
	private void LoseGame()
	{
		TurnInfo.text = "Good luck next time. AI won";
		numOfGamesLost++;
		PlayerPrefs.SetInt("GamesLost", numOfGamesLost);
		UpdateUiGamesWonLost();
	}

	private void DrawGame()
	{
		TurnInfo.text = "It's a draw";
	}

	public void AiMovePC()
	{
		StartCoroutine(AiThink(1.0f));
	}

	IEnumerator AiThink(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		
		//need to calculate power of every move
		Vector2 playerGoalPosition = mapGenerator.ReturnPlayerGoalPosition();
		foreach(AiMove aim in aiMoves)
		{
			int currentX = (int)currentPosition.x + aim.stepX;
			int currentY = (int)currentPosition.y + aim.stepY;
			aim.powerOfStep = Mathf.Abs(currentX - (int)playerGoalPosition.x) + (Mathf.Abs(currentY - (int)playerGoalPosition.y))*2; //priority on y axis
		}
		aiMoves = aiMoves.OrderBy(o => o.powerOfStep).ToList();
		foreach(AiMove aim in aiMoves)
		{
			if(MoveToPosition((int)currentPosition.x + aim.stepX, (int)currentPosition.y + aim.stepY))
			{
				break;
			}
		}
	}

	public void ResetGame()
	{
		SceneManager.LoadScene(0);
	}

	public void AutoWin()
	{
		WinGame();
		ResetGame();
	}
	public void AutoLose()
	{
		LoseGame();
		ResetGame();
	}

	class AiMove
	{
		public int stepX;
		public int stepY;
		public int powerOfStep;
	}
}
