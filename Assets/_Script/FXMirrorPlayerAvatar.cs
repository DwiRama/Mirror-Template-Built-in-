using Mirror;
using System.Collections.Generic;
using UnityEngine;

public abstract class FXMirrorPlayerAvatar : NetworkBehaviour
{
    [SerializeField] private InteractionController interactionController; //Reference to interaction controller
    [SerializeField] private List<GameObject> avatars; //List of avatar gameobject to be switch between
    [SyncVar(hook = "OnAvatarIndexChanged")] public int avatarIndex;

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
            SetupOtherPlayer();
            Debug.Log($"{netIdentity.netId} Avatar: is not yours");
            return;
        }

        // Setup Avatar
        avatarIndex = PlayerDataHandler.Instance.avatarIndex;
        ShowAvatar(); // Show local avatar
        CmdChangeAvatarIndex(avatarIndex); // Request to update server avatar

		SetupPlayerPerMode();

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
    /// Override this function to setup your player per mode
    /// </summary>
    protected abstract void SetupPlayerPerMode();

    /// <summary>
    /// Override this function if you want doing something
    /// </summary>
    protected abstract void SetupOtherPlayer();

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
