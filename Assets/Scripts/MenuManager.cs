using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

	int mapWidth = 8;
	int mapHeight = 12;

	public GameManager GameManager;
	public GameObject MenuUI;
	public GameObject GameUI;
	public Text MapWidthText;
	public Text MapHeightText;

	public void AddWidth()
	{
		if(mapWidth + 2 <= 20)
			mapWidth += 2;
		MapWidthText.text = mapWidth.ToString();
	}
	public void SubstractWidth()
	{
		if (mapWidth - 2 >= 8)
			mapWidth -= 2;
		MapWidthText.text = mapWidth.ToString();
	}
	public void AddHeight()
	{
		if (mapHeight + 2 <= 20)
			mapHeight += 2;
		MapHeightText.text = mapHeight.ToString();
	}
	public void SubstractHeight()
	{
		if (mapHeight - 2 >= 8)
			mapHeight -= 2;
		MapHeightText.text = mapHeight.ToString();
	}
	public void StartGame()
	{
		GameManager.StartGame(mapWidth, mapHeight);
		MenuUI.SetActive(false);
		GameUI.SetActive(true);
	}
}
