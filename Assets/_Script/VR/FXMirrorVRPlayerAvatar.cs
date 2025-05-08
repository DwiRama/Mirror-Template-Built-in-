using BNG;
using Mirror;
using UnityEngine;

public class FXMirrorVRPlayerAvatar : FXMirrorPlayerAvatar
{
	[Header("Local Player")]
	[SerializeField] InputBridge inputBridge;
	[SerializeField] VREmulator vREmulator;
	[SerializeField] GameObject playerControllerObj;
	[SerializeField] GameObject playerLocomotionObj;
	[SerializeField] CharacterIK characterIK;
	[SerializeField] CharacterIKFollow characterIKFollow;

	[Header("Other Player")]
	[SerializeField] Animator animator;

	protected override void SetupOtherPlayer()
	{
		Destroy(inputBridge);
		animator.runtimeAnimatorController = null;
	}

	#region CLIENT
	protected override void SetupPlayerPerMode()
	{
		inputBridge.enabled = true;
		playerControllerObj.SetActive(true);
		playerLocomotionObj.SetActive(true);
		characterIK.enabled = true;
		characterIKFollow.enabled = true;
		vREmulator.enabled = true;
	}
	#endregion
}