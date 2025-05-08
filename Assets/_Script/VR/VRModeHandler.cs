using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Management;

public class VRModeHandler : MonoBehaviour
{
	public UnityEvent OnDesktopMode;
    public UnityEvent OnVrMode;

    void Start()
    {
        if (PlayerDataHandler.Instance.vrMode)
        {
            OnVrMode?.Invoke();
	        return;
        }

		OnDesktopMode?.Invoke();
    }
}