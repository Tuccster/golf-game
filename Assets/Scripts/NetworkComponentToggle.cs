using UnityEngine;
using UnityEngine.Networking;

public class NetworkComponentToggle : MonoBehaviour 
{
	#region VARIABLES

	public enum StartType { ENABLE_ON_START, DISABLE_ON_START };
	public StartType _startType = StartType.DISABLE_ON_START;

	#endregion

	#region MONOBEHAVIOUR

	void Start()
	{
		if (!GetComponent<NetworkIdentity>()) gameObject.AddComponent<NetworkIdentity>();
		if (GetComponent<NetworkIdentity>().isLocalPlayer == true) 
		{
			if ((int)_startType == 0) gameObject.SetActive(true);
			else gameObject.SetActive(false); 
		}
		print("disabling " + transform.name);
	}

	#endregion
}
