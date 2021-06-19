using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HandlerUserInterfaceMenu : MonoBehaviour
{
	public Text _linesOfCode;
    public Text _usernameDisplay;
    public Image _userImageDisplay;


    #region MONOBEHAVIOUR

    private void Start()
    {
		GetTotalLinesOfCode();
        GetPlayerID();
    }

    #endregion

	#region PRIVATE_CONTROL

	private void GetTotalLinesOfCode()
	{
		int totalLines = 0;
		string[] scripts = Directory.GetFiles(Application.dataPath + "/Scripts/");
		foreach(string script in scripts)
		{
			string[] lines = File.ReadAllLines(script);
			totalLines += lines.Length;
		}
		_linesOfCode.text = totalLines.ToString();
	}

	private void GetPlayerID()
	{
		if (PlayerPrefs.GetInt("PlayerID", 0) != 0) 
		{
			_usernameDisplay.text = "ID#" + PlayerPrefs.GetInt("PlayerID");
			_userImageDisplay.color = new Color32((byte)PlayerPrefs.GetInt("PlayerIDcolor32r"), (byte)PlayerPrefs.GetInt("PlayerIDcolor32g"), (byte)PlayerPrefs.GetInt("PlayerIDcolor32b"), 255);
		}
		else GenerateNewPlayerID();
	}

	#endregion

    #region PUBLIC_CONTROL

    public void GenerateNewPlayerID()
    {
		PlayerPrefs.SetInt("PlayerID", Random.Range(1000, 10000));
		PlayerPrefs.SetInt("PlayerIDcolor32r", Random.Range(0, 256));
		PlayerPrefs.SetInt("PlayerIDcolor32g", Random.Range(0, 256));
		PlayerPrefs.SetInt("PlayerIDcolor32b", Random.Range(0, 256));
		GetPlayerID();
    }

    #endregion
}
