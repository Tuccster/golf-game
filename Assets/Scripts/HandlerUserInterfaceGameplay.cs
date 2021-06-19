using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HandlerUserInterfaceGameplay : MonoBehaviour 
{

	//   |--------------------------------------------|
	//   | NEEDS TO BE REWORKED -- VERY INEFFICIENT!  |
	//   |--------------------------------------------|

	#region VARIABLES

	public GameObject _scoreboard;
	public GameObject _playerRowHolder;
	public GameObject _playerRow;
	public GameObject _importantInfoPanel;
	public Text _hits;

	#endregion

	#region MONOBEHAVIOUR

	private void Start()
	{
		_importantInfoPanel.SetActive(true);
	}

	private void Update()
	{
		OpenScoreboard();
		if (Input.GetKeyDown(KeyCode.P)) 
		{
			_importantInfoPanel.SetActive(false);
			GameObject.FindGameObjectWithTag("Player").GetComponent<Player>()._allowHit = true;
		}
	}

	#endregion

	#region PRIVATE_CONTORL

	private void UpdateTotalScore(string playerID)
	{
		for (int i = 0; i < _playerRowHolder.transform.childCount; i++)
		{
			if (_playerRowHolder.transform.GetChild(i).GetChild(0).GetComponent<Text>().text == playerID)
			{
				int totalScore = 0;
				for (int j = 0; j < _playerRowHolder.transform.GetChild(i).GetChild(2).childCount; j++)
				{
					//print(_playerRowHolder.transform.GetChild(i).GetChild(2).GetChild(j).GetComponent<Text>().text);
					int readScore = 0;
					if (int.TryParse(_playerRowHolder.transform.GetChild(i).GetChild(2).GetChild(j).GetComponent<Text>().text, out readScore))
					{
						totalScore += readScore;
					}
				}
				_playerRowHolder.transform.GetChild(i).GetChild(3).GetComponent<Text>().text = totalScore.ToString();
			}
		}
	}

	#endregion

	#region PUBLIC_CONTROL

	public void AddPlayer(Color ballColor, string playerID)
	{
		GameObject curPlayerRow = GameObject.Instantiate(_playerRow);
		curPlayerRow.transform.SetParent(_playerRowHolder.transform);
		curPlayerRow.transform.GetChild(0).GetComponent<Text>().text = playerID;
		curPlayerRow.transform.GetChild(1).GetComponent<Image>().color = ballColor;
	}

	public void AddEntry(string playerID, byte holeID, byte score)
	{
		for (int i = 0; i < _playerRowHolder.transform.childCount; i++)
		{
			if (_playerRowHolder.transform.GetChild(i).GetChild(0).GetComponent<Text>().text == playerID)
			{
				_playerRowHolder.transform.GetChild(i).GetChild(2).GetChild(holeID).GetComponent<Text>().text = score.ToString();
				UpdateTotalScore(playerID);
				return;
			}
		}
	}

	#endregion

	#region KEY_LISTENERS

	private void OpenScoreboard()
	{
		if (Input.GetKey(KeyCode.Tab)) _scoreboard.SetActive(true); 
		else _scoreboard.SetActive(false);
	}

	#endregion
}

