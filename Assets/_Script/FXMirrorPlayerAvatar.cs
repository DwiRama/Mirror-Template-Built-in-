using Cinemachine;
using Mirror;
using StarterAssets;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FXMirrorPlayerAvatar : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraRoot; //Reference to transform where camera needs to follow
    [SerializeField] private ThirdPersonController tpController; //Reference to thirdperson camera controller
    [SerializeField] private StarterAssetsInputs assetsInputs; //Reference to input asset
    [SerializeField] private PlayerInput playerInput; //Reference to player input
    [SerializeField] private InteractionController interactionController; //Reference to interaction controller
    [SerializeField] private List<GameObject> avatars; //List of avatar gameobject to be switch between

    [SyncVar(hook = "OnAvatarIndexChanged")] public int avatarIndex;

    private CinemachineVirtualCamera player3POVCam; //Reference to thirdperson cinemachine camera

    #region Server

    /// <summary>
    /// Client command to change the avatarIndex on the Server and also update the Avatar on the Server
    /// </summary>
    /// <param name="newavatarIndex"></param>
    [Command]
    public void CmdChangeAvatarIndex(int newavatarIndex)
    {
        avatarIndex = newavatarIndex;
        ShowAvatar();
    }

    #endregion

    #region Client
    /// <summary>
    /// Initial setup when the avatar first spawn
    /// </summary>
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!isOwned)
        {
            Debug.Log($"{netIdentity.netId} Avatar: is not yours");
            return;
        }

        // Setup Avatar
        avatarIndex = PlayerDataHandler.Instance.avatarIndex;
        ShowAvatar(); // Show local avatar
        CmdChangeAvatarIndex(avatarIndex); // Request to update server avatar

        // Find Player Virtual Camera
        GameObject vCamObj = GameObject.FindGameObjectWithTag("VCamThirdPOV");
        player3POVCam = vCamObj.GetComponent<CinemachineVirtualCamera>();

        // Set Camera Follow
        player3POVCam.Follow = playerCameraRoot;
        Debug.Log("Setup Camera follow");

        if (tpController != null)
        {
            tpController.enabled = true;
            Debug.Log("Enabled movementScript");
        }

        if (assetsInputs != null) 
        {
            assetsInputs.enabled = true;
            Debug.Log("Enabled assetsInputs");
        } 

        if (playerInput != null)
        {
            playerInput.enabled = true;
            Debug.Log("Enabled playerInput");
        }

        // Set interaction
        if (interactionController == null)
        {
            return;
        }

        interactionController.enabled = true;
        interactionController.Setup(transform);

        InteractionControllerView interactionView = FindAnyObjectByType<InteractionControllerView>();
        interactionView.interactionController = interactionController;
        interactionView.Setup();

    }

    /// <summary>
    /// Hook to change the avatar index, Hook method must have old and new value as parameters
    /// </summary>
    public void OnAvatarIndexChanged(int oldValue, int newValue)
    {
        avatarIndex = newValue;
        ShowAvatar();
    }

    /// <summary>
    /// Show the avatar based on selected index
    /// </summary>
    private void ShowAvatar()
    {
        if (avatarIndex >= 0 && avatarIndex < avatars.Count)
        {
            DeactivateAllAvatars();
            avatars[avatarIndex].SetActive(true);
            Debug.Log("Show avatar");
        }
    }

    /// <summary>
    /// Reset all avatar gameobject to inactive
    /// </summary>
    private void DeactivateAllAvatars()
    {
        for (int i = 0; i < avatars.Count; i++)
        {
            avatars[i].SetActive(false);
        }
    }

    #endregion
}
