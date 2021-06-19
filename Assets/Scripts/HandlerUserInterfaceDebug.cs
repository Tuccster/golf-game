using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HandlerUserInterfaceDebug : MonoBehaviour
{
    [Header("Resources")]
    public GameObject _debugOverlay;
    public Text[] _dataDisplay;

    private Player _player;
    private Rigidbody _playerRigidbody;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _playerRigidbody = _player.GetComponent<Rigidbody>();
        _dataDisplay = new Text[_debugOverlay.transform.childCount];
        for (int i = 0; i < _debugOverlay.transform.childCount; i++)
        {
            _dataDisplay[i] = _debugOverlay.transform.GetChild(i).GetComponent<Text>();
        }
    }

    private void Update()
    {
        ToggleDebugOverlay();
        UpdateValues();
    }

    private void UpdateValues()
    {
        _dataDisplay[0].text = "PLAYER_POSITION : " + _player.transform.position;
        _dataDisplay[1].text = "PLAYER_POSITION_LAST : " + _player._positionLast;
        _dataDisplay[2].text = "PLAYER_POSITION_START : " + _player._positionStart;
        _dataDisplay[3].text = "PLAYER_VELOCITY : " + _playerRigidbody.velocity;
        _dataDisplay[4].text = "PLAYER_CAMERA_ROTATION : " + _player._cameraCenter.rotation.eulerAngles;
        //_dataDisplay[5].text = "PLAYER_CAMERA_DISTANCE : " + Vector3.Distance(Camera.main.transform.position, _player.transform.position);
        _dataDisplay[6].text = "PLAYER_CLUBTYPE : " + _player._clubType;
        _dataDisplay[7].text = "PLAYER_CANHIT : " + _player._canHit;
        _dataDisplay[8].text = "PLAYER_HITTING : " + _player._hitting;
        _dataDisplay[9].text = "PLAYER_HITTINGMOUSEY : " + _player._hittingMouseY;
        _dataDisplay[10].text = "PLAYER_PIXELS_FROM_HITTINGMOUSEY : " + Mathf.Abs(_player._hittingMouseY - Input.mousePosition.y);
        _dataDisplay[11].text = "PLAYER_COMPLETED_GREEN_CHECK : " + _player._greenCheckComplete;
        _dataDisplay[12].text = "PLAYER_GREEN_CHECK_TAG : " + _player._greenCheckTag;
        _dataDisplay[13].text = "PLAYER_GREEN_CHECK_NAME : " + _player._greenCheckName;
        _dataDisplay[14].text = "PLAYER_ON_GREEN : " + _player._onGreen;
        _dataDisplay[15].text = "PLAYER_HOLE_ID : " + _player._holeID;
        //_dataDisplay[16].text controlled by HoleTrigger
        //_dataDisplay[17].text controlled by HoleTrigger
        _dataDisplay[18].text = "GAME_FRAMRATE : " + (int)(1.0f / Time.smoothDeltaTime);
        _dataDisplay[19].text = "GAME_MOUSE_SPEED : " + Input.GetAxis("Mouse Y");
        _dataDisplay[20].text = "GAME_MOUSE_POSITION :" + Input.mousePosition;
        _dataDisplay[21].text = "GAME_MOUSE_LOCK_STATE :" + Cursor.lockState;
    }

    private void ToggleDebugOverlay()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (_debugOverlay.activeSelf) _debugOverlay.SetActive(false);
            else _debugOverlay.SetActive(true);
        }
    }
}
