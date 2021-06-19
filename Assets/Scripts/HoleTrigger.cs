using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HoleTrigger : MonoBehaviour
{
    public float _stayInHoleTime = 1.0f;
    private byte _holeID = 0;
    private IEnumerator _stayInHoleTimer;
    private GameObject[] _ballSpawns;
    private HandlerUserInterfaceDebug _debug;
    private HandlerUserInterfaceGameplay _gameplay;
    private GameObject _curPlayer;
    
    private void Start()
    {
        if (GetComponent<NetworkIdentity>().isLocalPlayer) this.enabled = false;

        _gameplay = GameObject.FindObjectOfType<HandlerUserInterfaceGameplay>();
        _holeID = transform.parent.GetComponent<ObjectProperties>()._holeID;
        _ballSpawns = GameObject.FindGameObjectsWithTag("BallSpawn");
        _debug = GameObject.FindObjectOfType<HandlerUserInterfaceDebug>();
    }

    private void Update()
    {
        //_debug._dataDisplay[16].text = "HOLETRIGGER_INHOLE : " + _inHole;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            _curPlayer = other.gameObject;
            MoveToNextHole();
        }
    }

    private void MoveToNextHole()
    {
        Player player = _curPlayer.GetComponent<Player>();
        foreach (GameObject ballSpawn in _ballSpawns)
        {
            if (ballSpawn.GetComponent<ObjectProperties>()._holeID == player._holeID + 1)
            {
                _debug._dataDisplay[17].text = "HOLETRIGGER_INHOLE : " + ballSpawn.transform.position;
                player._positionStart = ballSpawn.transform.position;
                _curPlayer.transform.position = ballSpawn.transform.position;
                _curPlayer.transform.rotation = ballSpawn.transform.rotation;
                player.GetComponent<PlayerNetworked>().CmdAddEntry("ID#" + player._playerID, _holeID, player._hits);
                player._holeID++;
                player._hits = 0;
                //player.RemoveVelocity();
                _gameplay._hits.text = player._hits.ToString();
                _curPlayer = null;
                return;
            }
        }
        //print("Last Hole, Bucko");
    }
}
