using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

public class VRManualControlHandler : MonoBehaviour
{
	[SerializeField] private bool forcePlayInEditor;
	[SerializeField] private bool startXROnStart;
	[SerializeField] private bool stopXROnDestroy;

	private void Start()
	{
		if (!startXROnStart)
		{
			return;
		}

		if (!PlayerDataHandler.Instance.vrMode)
		{
			return;
		}


#if !UNITY_EDITOR
		StartCoroutine(StartXRCoroutine());
		return;
#endif
		
		if (forcePlayInEditor)
		{
			StartCoroutine(StartXRCoroutine());
		}
	}

	private void OnDestroy()
	{
		if (!stopXROnDestroy)
		{
			return;
		}

		if (!PlayerDataHandler.Instance.vrMode)
		{
			return;
		}

#if !UNITY_EDITOR
		StopXR();
		return;
#endif

		if (forcePlayInEditor)
		{
			StopXR();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public IEnumerator StartXRCoroutine()
	{
		Debug.Log("Initializing XR...");
		yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

		if (XRGeneralSettings.Instance.Manager.activeLoader == null)
		{
			Debug.LogError("Initializing XR Failed. Check Editor or Player log for details.");
		}
		else
		{
			Debug.Log("Starting XR...");
			XRGeneralSettings.Instance.Manager.StartSubsystems();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	void StopXR()
	{
		if (XRGeneralSettings.Instance.Manager.activeLoader == null)
		{
			Debug.Log("No activeLoader found.");
			return;
		}

		Debug.Log("Stopping XR...");
		XRGeneralSettings.Instance.Manager.StopSubsystems();
		XRGeneralSettings.Instance.Manager.DeinitializeLoader();
		Debug.Log("XR stopped completely.");
	}
}
