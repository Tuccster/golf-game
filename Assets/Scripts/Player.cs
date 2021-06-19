using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : MonoBehaviour
{
    #region VARIABLES

    [Header("Sound")]
    public float _puttSoundMultiplier;
    public AudioClip _audioPutt;
    public float _impactSoundMultiplier;
    public AudioClip _impact;
    private AudioSource _audioSource;

    [Header("General")]
    public bool _configureNetworkSettingsOnStart = true;
    public float _velocityMargin;
    public float _groundDistanceCheck;
    public GameObject _userInterface;

    [Header("Jump")]
    public bool _allowJump = true;
    public float _jumpForce = 10;

    [Header("Clubs")]
    public float _hitMultiplier = 5.0f;
    public float _clubSwingMultiplier = 0.01f;
    public float _clubStartDistance = 0.75f;
    public Vector2 _clubPutterForce;
    public Vector2 _clubChipperForce;
    public Vector2 _clubDriverForce;
    public GameObject _clubHolder;

    [Header("Guide")]
    public GameObject _guide;

    [Header("Camera")]
    public float _startIncrement = 3;
    public float _zoomIncrement = 1;
    public float _panExeleration = 1;
    public Camera _camera;
    private Transform _cameraTransform;
    public Transform _cameraCenter;

    //private
    private Vector3 _lockedCameraAngle;
    private HandlerUserInterfaceGameplay _gameplayOverlay;
    private PlayerNetworked _playerNetworked;
    private bool _positionLastSet = false;
    private bool _clubShown;
    private bool _isLocal;

    //read-only private
    [HideInInspector] public Rigidbody _rigidbody;
    [HideInInspector] public bool _allowHit = false;
    [HideInInspector] public bool _allowCursorControl = true;
    [HideInInspector] public bool _allowInput = true;
    [HideInInspector] public int _playerID;
    [HideInInspector] public bool _canHit = false;
    [HideInInspector] public bool _hitting = false;
    [HideInInspector] public bool _stopped = true;
    [HideInInspector] public bool _onGreen = false;
    [HideInInspector] public bool _greenCheckComplete = false;
    [HideInInspector] public string _greenCheckTag;
    [HideInInspector] public string _greenCheckName;
    [HideInInspector] public float _hittingMouseY = -1;
    [HideInInspector] public Vector3 _positionStart;
    [HideInInspector] public Vector3 _positionLast;
    [HideInInspector] public byte _holeID = 0;
    [HideInInspector] public byte _hits = 0;

    //read-only enum
    public enum ClubType { PUTTER, CHIPPER, DRIVER, HOLE_IN_ONE };
    [HideInInspector] public ClubType _clubType = ClubType.PUTTER;

    #endregion

    #region MONOBEHAVIOUR

    private void Start()
    {
        ConfigureNonLocal();
        InitCursor();

        _playerID = PlayerPrefs.GetInt("PlayerID");
        _playerNetworked = GetComponent<PlayerNetworked>();
        _gameplayOverlay = GameObject.FindObjectOfType<HandlerUserInterfaceGameplay>();
        transform.parent = GameObject.FindGameObjectWithTag("World.Dynamic").transform;
        _audioSource = GetComponent<AudioSource>();
        _clubStartDistance /= 10;
        _hittingMouseY = -1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        _rigidbody = GetComponent<Rigidbody>();
        _cameraTransform = _camera.transform;
        _positionStart = transform.position;
        _cameraTransform.position += _cameraTransform.forward * _zoomIncrement * -_startIncrement;
        GetComponent<MeshRenderer>().material.color = new Color32((byte)PlayerPrefs.GetInt("PlayerIDcolor32r"), (byte)PlayerPrefs.GetInt("PlayerIDcolor32g"), (byte)PlayerPrefs.GetInt("PlayerIDcolor32b"), 255);
    }

    private void Update()
    {
        CameraZoom();
        CameraPan();
        HitBall();
        UpdateGuideDirection();
        UpdateClubType();
        CheckIfStopped();
        ResetPositionListener();
        Jump();
        UpdateClubModel();
        UpdateClubDistance();
        CursorControl();

        if (Input.GetKeyDown(KeyCode.P))
		{
			_playerNetworked.CmdAddPlayer(new Color32(
			(byte)PlayerPrefs.GetInt("PlayerIDcolor32r"), 
			(byte)PlayerPrefs.GetInt("PlayerIDcolor32g"), 
			(byte)PlayerPrefs.GetInt("PlayerIDcolor32b"), 
			255), "ID#" + PlayerPrefs.GetInt("PlayerID"));
		}
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlaySound(_impact, ((_rigidbody.velocity.x +_rigidbody.velocity.y + _rigidbody.velocity.z) / 3) * _impactSoundMultiplier, 3);
    }

    #endregion

    #region CAMERA

    private void CameraZoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && _allowInput) _cameraTransform.position += _cameraTransform.forward * _zoomIncrement;
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && _allowInput) _cameraTransform.position -= _cameraTransform.forward * _zoomIncrement;
    }

    private void CameraPan()
    {
        _cameraCenter.rotation = Quaternion.Euler(_lockedCameraAngle);
        if (Input.GetMouseButton(1))
        {
            if (_hitting) _hitting = false;
            if (_hittingMouseY != -1) _hittingMouseY = -1;
            _cameraTransform.LookAt(transform.position);
            _cameraCenter.rotation = Quaternion.Euler(_cameraCenter.rotation.eulerAngles.x + -Input.GetAxis("Mouse Y") * _panExeleration, _cameraCenter.rotation.eulerAngles.y + Input.GetAxis("Mouse X") * _panExeleration, 0);
            _lockedCameraAngle = _cameraCenter.rotation.eulerAngles;
        }
    }

    #endregion

    #region MOVEMENT

    private void CheckIfStopped()
    {
        if (Mathf.Abs(_rigidbody.velocity.x) <= _velocityMargin && Mathf.Abs(_rigidbody.velocity.y) <= _velocityMargin && Mathf.Abs(_rigidbody.velocity.z) <= _velocityMargin)
        {
            _stopped = true;
            _canHit = true;
            _guide.SetActive(true);
            _clubHolder.SetActive(true);
            CheckIfOnGreen();
            if (!_positionLastSet)
            {
                if (_onGreen) _positionLast = transform.position;
                _positionLastSet = true;
            }
        }
        else
        {
            _greenCheckComplete = false;
            _stopped = false;
            _canHit = false;
            _guide.SetActive(false);
            _clubHolder.SetActive(false);
            _positionLastSet = false;
        }
    }

    private void CheckIfOnGreen()
    {
        if (_stopped && !_greenCheckComplete)
        {
            _greenCheckTag = _greenCheckName = string.Empty;
            _onGreen = false;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, (transform.localScale.x / 2) + _groundDistanceCheck))
            {
                _greenCheckTag = hit.transform.tag;
                _greenCheckName = hit.transform.name.Split(' ')[0];
                if (hit.transform.tag == "Green" && hit.transform.GetComponent<ObjectProperties>()._holeID == _holeID) _onGreen = true;
            }
            if (!_onGreen && _positionLast != Vector3.zero)
            {
                transform.position = _positionLast;
            }
            _greenCheckComplete = true;
        }
    }

    private void HitBall()
    {
        if (!_allowHit) return;
        if (Input.GetMouseButtonDown(0) && !_hitting && _canHit && _allowInput)
        {
            _hitting = true;
            _hittingMouseY = Input.mousePosition.y;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else if (_hitting)
        {
            if (Input.mousePosition.y > _hittingMouseY)
            {
                Vector3 hitForce = CalculateHitForce(Input.GetAxis("Mouse Y") * _hitMultiplier);
                _rigidbody.AddForce(hitForce);
                PlaySound(_audioPutt, ((hitForce.x + hitForce.y + hitForce.z) / 3) * _puttSoundMultiplier, 1);
                _hitting = false;
                _hittingMouseY = -1;
                _hits++;
                _gameplayOverlay._hits.text = _hits.ToString();
                //Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    private Vector3 CalculateHitForce(float hitForce)
    {
        Vector3 guideForward = _guide.transform.forward;
        if (_clubType == ClubType.PUTTER) return new Vector3(guideForward.x * _clubPutterForce.x, _clubPutterForce.y, guideForward.z * _clubPutterForce.x) * hitForce;
        else if (_clubType == ClubType.CHIPPER) return new Vector3(guideForward.x * _clubChipperForce.x, _clubChipperForce.y, guideForward.z * _clubChipperForce.x) * hitForce;
        else if (_clubType == ClubType.DRIVER) return new Vector3(guideForward.x * _clubDriverForce.x, _clubDriverForce.y, guideForward.z * _clubDriverForce.x) * hitForce;
        else return new Vector3();
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _allowInput) _rigidbody.AddForce(new Vector3(0, _jumpForce, 0));
    }

    public void RemoveVelocity()
    {
        _rigidbody.velocity = _rigidbody.angularVelocity = Vector3.zero;
    }

    #endregion

    #region GUIDE

    private void UpdateGuideDirection()
    {
        _guide.transform.rotation = Quaternion.Euler(0, _cameraCenter.rotation.eulerAngles.y, 0);
        _clubHolder.transform.rotation = Quaternion.Euler(0, _cameraCenter.rotation.eulerAngles.y, 0);
    }

    #endregion

    #region CLUBS

    private void UpdateClubType()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && _clubType != ClubType.PUTTER) _clubType = ClubType.PUTTER;
        else if (Input.GetKeyDown(KeyCode.Alpha2) && _clubType != ClubType.CHIPPER) _clubType = ClubType.CHIPPER;
        else if (Input.GetKeyDown(KeyCode.Alpha3) && _clubType != ClubType.DRIVER) _clubType = ClubType.DRIVER;
        else if (Input.GetKeyDown(KeyCode.Alpha4) && _clubType != ClubType.HOLE_IN_ONE) _clubType = ClubType.HOLE_IN_ONE;
    }

    private void UpdateClubModel()
    {
        for (int i = 0; i < 4; i++)
        {
            _clubHolder.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
        }
        _clubHolder.transform.GetChild(0).GetChild((int)_clubType).gameObject.SetActive(true);
    }

    private void UpdateClubDistance()
    {
        if (_hittingMouseY == -1) _clubHolder.transform.GetChild(0).position = _guide.transform.position + _guide.transform.forward * _clubStartDistance;
        else
            _clubHolder.transform.GetChild(0).position = _guide.transform.position + (_guide.transform.forward * (_clubStartDistance - (_clubSwingMultiplier * (_hittingMouseY - Input.mousePosition.y))));
    }

    #endregion

    #region RESET_POSITION

    private void ResetPositionListener()
    {
        if (Input.GetKeyDown(KeyCode.R) && _allowInput) ResetRigidbody(_positionStart);
        if (Input.GetKeyDown(KeyCode.F) && _allowInput) ResetRigidbody(_positionLast);
    }

    private void ResetRigidbody(Vector3 position)
    {
        transform.position = position;
        _rigidbody.velocity = _rigidbody.angularVelocity = Vector3.zero;
    }

    #endregion

    #region AUDIO

    private void PlaySound(AudioClip clip, float volume, float pitch)
    {
        _audioSource.clip = clip;
        _audioSource.volume = volume;
        _audioSource.pitch = pitch;
        _audioSource.Play();
    }

    #endregion

    #region NETWORK

    private void ConfigureNonLocal()
    {
        if (!_configureNetworkSettingsOnStart) return;

        _isLocal = GetComponent<NetworkIdentity>().isLocalPlayer;
        if (!_isLocal)
        {
            transform.tag = "PlayerNonLocal";
            _guide.SetActive(false);
            _camera.gameObject.SetActive(false);
            _clubHolder.SetActive(false);
            _userInterface.SetActive(false);
            GetComponent<Player>().enabled = false;
        }
    }

    public void ScoreboardAddPlayer()
    {

    }

    public void ScoreboardAddEntry()
    {
        
    }

    #endregion

    #region CURSOR

    private void InitCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void CursorControl()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _allowCursorControl)
        {
            if (Cursor.visible)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    #endregion
}

class PlayerNetworking : NetworkBehaviour
{

}
