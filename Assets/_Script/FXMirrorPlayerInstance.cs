using Mirror;
using System;
using UnityEngine;

public class FXMirrorPlayerInstance : NetworkBehaviour
{
    public static NetworkIdentity localPlayer; //reference to client local player, can be access in local only

    [SyncVar(hook = "OnPlayerInstancePlayerNameChanged")] public string playerName;
    [SyncVar(hook = "OnPlayerInstanceAvatarIndexChanged")] public int avatarIndex;
    [SyncVar(hook = "OnPlayerInstanceVRModeChanged")] public bool vrMode;
    [SerializeField] private GameObject playerAvatarPrefab; // The prefab of player avatar to spawn to the world
    [SerializeField] private GameObject playerVRAvatarPrefab; // The prefab of player avatar to spawn to the world

    [SerializeField]

    public event Action<string> OnPlayerChangedName; // Callback when playerName has changed


    #region Server
    /// <summary>
    /// Request Server to change playerName
    /// </summary>
    /// <param name="newName">Player new name</param>
    [Command]
    public void CmdChangePlayerName(string newName)
    {
        playerName = newName;
    }

    /// <summary>
    /// Request server to Spawn Player Avatar to the scene
    /// </summary>
    [Command]
    public void CmdRequestSpawnPlayerAvatar()
    {
        if (!isServer) return;
        Debug.Log("Request server to spawn Avatar");

        // Get Spawn location
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();

        // Spawn Avatar on the server
        GameObject spawnedObject = Instantiate(!vrMode ? playerAvatarPrefab : playerVRAvatarPrefab, spawnPoint.position, spawnPoint.rotation);

        // Spawn the object on all clients
        NetworkServer.Spawn(spawnedObject, connectionToClient);
    }

    /// <summary>
    /// Request Server to change selected Avatar index 
    /// </summary>
    /// <param name="newAvatarIndex"></param>
    [Command]
    public void CmdChangeAvatarIndex(int newAvatarIndex)
    {
        avatarIndex = newAvatarIndex;
    }

    /// <summary>
    /// Request Server to change the VRMode state
    /// </summary>
    /// <param name="newVrMode"></param>
    [Command]
    public void CmdChangeVRMode(bool newVrMode)
    {
        vrMode = newVrMode;
    }
    #endregion

    #region Client
    /// <summary>
    /// Client local player OnStart
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        localPlayer = netIdentity;

        if (PlayerDataHandler.Instance != null)
        {
            CmdChangeVRMode(PlayerDataHandler.Instance.vrMode); // Set Player VRMode based on offline scene input
            CmdChangePlayerName(PlayerDataHandler.Instance.playerName); // Set Player Name based on offline scene input
            CmdChangeAvatarIndex(PlayerDataHandler.Instance.avatarIndex); // Set Player Index based on offline scene input
        }
        else
        {
            CmdChangePlayerName($"Player {netId}"); // Set Player Name by netId
        }

        SpawnPlayerAvatar(); // Spawn Player Avatar
    }

    /// <summary>
    /// Hook called on client when playerName is change on the server
    /// </summary>
    /// <param name="oldName">previous playerName value</param>
    /// <param name="newName">new playerName value</param>
    public void OnPlayerInstancePlayerNameChanged(string oldName, string newName)
    {
        playerName = newName;
        OnPlayerChangedName?.Invoke(playerName);
    }

    /// <summary>
    /// Check and spawn player avatar to the scene
    /// </summary>
    public void SpawnPlayerAvatar()
    {
        Debug.Log($"Connected: {NetworkClient.isConnected}");
        Debug.Log($"LocalPlayer: {NetworkClient.localPlayer != null}");

        CmdRequestSpawnPlayerAvatar();
    }

    /// <summary>
    /// Hook called on client when avatarIndex change on the server
    /// </summary>
    /// <param name="oldValue">previous avatar index</param>
    /// <param name="newValue">new avatar index</param>
    public void OnPlayerInstanceAvatarIndexChanged(int oldValue, int newValue)
    {
        avatarIndex = newValue;
    }

    /// <summary>
    /// Hook called on client when vrMode change on the server
    /// </summary>
    /// <param name="oldValue">previous vrMode state</param>
    /// <param name="newValue">new vrMode state</param>
    public void OnPlayerInstanceVRModeChanged(bool oldValue, bool newValue)
    {
        vrMode = newValue;
    }
	#endregion
}
