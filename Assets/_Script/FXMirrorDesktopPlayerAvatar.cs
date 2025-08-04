using Cinemachine;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class FXMirrorDesktopPlayerAvatar : FXMirrorPlayerAvatar
{
	[SerializeField] private Transform playerCameraRoot; //Reference to transform where camera needs to follow
	[SerializeField] private ThirdPersonController tpController; //Reference to thirdperson camera controller
	[SerializeField] private StarterAssetsInputs assetsInputs; //Reference to input asset
	[SerializeField] private PlayerInput playerInput; //Reference to player input
	private CinemachineVirtualCamera player3POVCam; //Reference to thirdperson cinemachine camera

	protected override void SetupOtherPlayer()
	{
		
	}

	protected override void SetupPlayerPerMode()
	{
		// Find Player Virtual Camera
		GameObject vCamObj = GameObject.FindGameObjectWithTag("VCamThirdPOV");
		player3POVCam = vCamObj.GetComponent<CinemachineVirtualCamera>();

		// Set Camera Follow
		player3POVCam.Follow = playerCameraRoot;
		Debug.Log("Setup Camera follow");

		// Set thirdperson controller
		if (tpController != null)
		{
			tpController.enabled = true;
			Debug.Log("Enabled movementScript");
		}

		// Set player controller input interface
		if (assetsInputs != null)
		{
			assetsInputs.enabled = true;
			Debug.Log("Enabled assetsInputs");

			UICanvasControllerInput uICanvasControllerInput = FindAnyObjectByType<UICanvasControllerInput>();
			if (uICanvasControllerInput != null)
			{
				// Show mobile control if it's native mobile build or in editor
				if ((Application.platform == RuntimePlatform.Android && !XRSettings.enabled) || Application.platform == RuntimePlatform.IPhonePlayer || Application.isEditor)
				{
					uICanvasControllerInput.gameObject.SetActive(true);
                }
				#if UNITY_WEBGL
				// Show mobile control if it's web mobile build
				else if (Application.platform == RuntimePlatform.WebGLPlayer && WebMobileDetection.Instance.IsRunningOnMobile())
                {
						uICanvasControllerInput.gameObject.SetActive(true);
                }
				#endif
                else
                {
					uICanvasControllerInput.gameObject.SetActive(false);
				}

				uICanvasControllerInput.starterAssetsInputs = assetsInputs;
				Debug.Log("Mobile UI Canvas set");
			}
		}

		if (playerInput != null)
		{
			playerInput.enabled = true;
			Debug.Log("Enabled playerInput");
		}
	}
}
