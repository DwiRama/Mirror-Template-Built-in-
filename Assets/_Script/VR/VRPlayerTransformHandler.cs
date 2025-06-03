using BNG;
using UnityEngine;
#if !UNITY_ANDROID
//using Valve.VR;
#endif

public class VRPlayerTransformHandler : MonoBehaviour
{
	[HideInInspector] public Vector3 normalScale = Vector3.one;
	[HideInInspector] public Vector3 bigPlayerScale;

	public CharacterController characterController;
	public CharacterIKFollow characterIKFollow;
	public Transform locomotionTransform;
	
	private const float baseMultipler = 4.7f;
	private Vector3 bigLocomotionScale;
	private Vector3 bigGeometryPositionOffset;
	private Vector3 bigGeometryLookDownOffset;
	private bool isBigPlayer;

	public bool XButtonDown { get; private set; }
	public bool YButtonDown { get; private set; }

	void Start()
	{
		bigPlayerScale = transform.localScale;
		bigGeometryPositionOffset = characterIKFollow.PositionOffset;
		bigGeometryLookDownOffset = characterIKFollow.LookDownOffset;

		BigPlayer();
	}

	// Update is called once per frame
	void Update()
	{
        //XButtonDown = SteamVR_Actions.vRIF_XButton.stateDown;
#if !UNITY_ANDROID
		//YButtonDown = SteamVR_Actions.vRIF_YButton.stateDown;
#endif

        //if (XButtonDown)
        //{
        //	characterIKFollow.UpdateBasePlayerHeight(characterController.height);
        //	characterIKFollow.AdjustPlayerScale();
        //	return;
        //}

        if (YButtonDown)
		{
			ToggleTransformPlayer();
			return;
		}
	}

	public void NormalPlayer()
	{
		transform.localScale = normalScale;
		locomotionTransform.localScale = normalScale;

		var newPosOffset = bigGeometryPositionOffset;
		var newLookDownOffset = bigGeometryLookDownOffset;

		newPosOffset.y += newPosOffset.y * baseMultipler;
		newLookDownOffset.y += newLookDownOffset.y * baseMultipler;

		characterIKFollow.PositionOffset = newPosOffset;
		characterIKFollow.LookDownOffset = newLookDownOffset;
		isBigPlayer = false;
	}

	public void BigPlayer()
	{
		var newScale = bigPlayerScale.x / normalScale.x;
		bigLocomotionScale = normalScale / newScale;

		locomotionTransform.localScale = bigLocomotionScale;
		transform.localScale = bigPlayerScale;

		characterIKFollow.PositionOffset = bigGeometryPositionOffset;
		characterIKFollow.LookDownOffset = bigGeometryLookDownOffset;
		isBigPlayer = true;
	}

	public void ToggleTransformPlayer()
	{
		isBigPlayer = !isBigPlayer;
		
		if (isBigPlayer)
		{
			BigPlayer();
		}
		else
		{
			NormalPlayer();
		}
	}
}
