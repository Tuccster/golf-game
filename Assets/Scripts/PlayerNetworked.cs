using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetworked : NetworkBehaviour 
{
	[Command]
	public void CmdAddPlayer(Color ballColor, string playerID)
	{
		RpcAddPlayer(ballColor, playerID);
	}

	[ClientRpc]
	public void RpcAddPlayer(Color ballColor, string playerID)
	{
		GameObject.FindGameObjectWithTag("Player").transform.GetChild(3).GetComponent<HandlerUserInterfaceGameplay>().AddPlayer(ballColor, playerID);
	}

	[Command]
	public void CmdAddEntry(string playerID, byte holeID, byte score)
	{
		RpcAddEntry(playerID, holeID, score);
	}

	[ClientRpc]
	public void RpcAddEntry(string playerID, byte holeID, byte score)
	{
		GameObject.FindGameObjectWithTag("Player").transform.GetChild(3).GetComponent<HandlerUserInterfaceGameplay>().AddEntry(playerID, holeID, score);
	}
}
