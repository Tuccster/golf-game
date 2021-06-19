using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HandlerUserInterfaceCommand : MonoBehaviour {

	#region VARIABLES

	[Header("Resources")]
	public GameObject _commandExecutionWindow;
	public GameObject _logPanel;
	public Text _loggedText;
	public Text _commandInput;

	private Player _player;

	#endregion

	#region MONOBEHAVIOUR

	private void Start()
	{
		_player = GameObject.FindObjectOfType<Player>();
		_commandExecutionWindow.SetActive(false);
		NewLog(" < CONSOLE HAS STARTED WITHOUT ANY ERRORS >", Color.magenta);
	}

	private void Update()
	{
		ControlInput();
		InputCommand();
	}

	#endregion

	#region COMMANDS

	private void Execute(string command)
	{
		NewLog(">" + command, Color.black);
	}

	#endregion

	#region PUBLIC_INTERACT

	public void NewLog(string text, Color color)
	{
		Text curLog = GameObject.Instantiate(_loggedText);
		curLog.color = color;
		curLog.text = text;
		curLog.transform.parent = _logPanel.transform;
	}

	#endregion

	#region INPUT

	private void ControlInput()
	{
		if (Input.GetKeyDown(KeyCode.F2))
		{
			if (_commandExecutionWindow.activeSelf) 
			{
				_player._allowInput = true;
				_player._allowCursorControl = true;
				_commandExecutionWindow.SetActive(false);
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
			else 
			{
				_player._allowInput = false;
				_player._allowCursorControl = false;
				_commandExecutionWindow.SetActive(true);
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}
	}

	private void InputCommand()
	{
		if (Input.GetKeyDown(KeyCode.Return) && _commandExecutionWindow.activeSelf)
		{
			Execute(_commandInput.text);
			_commandInput.text = "";
		}
	}

	#endregion
}
