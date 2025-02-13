using Mirror;
using System;
using UnityEngine;

public class FXMirrorPlayerInstance : NetworkBehaviour
{
    public static NetworkIdentity localPlayer; //reference to client local player, can be access in local only

    [SyncVar(hook = "OnPlayerInstancePlayerNameChanged")] public string playerName;
    [SyncVar(hook = "OnPlayerInstanceAvatarIndexChanged")] public int avatarIndex;
    [SerializeField] private GameObject playerAvatarPrefab; // The prefab of player avatar to spawn to the world

    public event Action<string> OnPlayerChangedName; // Callback when playerName has changed
    private ObjectSpawner spawner; // Reference to the ObjectSpawner

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
        GameObject spawnedObject = Instantiate(playerAvatarPrefab, spawnPoint.position, spawnPoint.rotation);

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

    // Command sent to the server from the player object
    [Command]
    void CmdRequestSpawn(Vector3 position)
    {
        if (spawner == null)
        {
            Debug.Log("Spawner not found");
            return;
        }
        spawner.SpawnObject(position); // Call the spawner's method on the server
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
            CmdChangePlayerName(PlayerDataHandler.Instance.playerName); // Set Player Name based on offline scene input
            CmdChangeAvatarIndex(PlayerDataHandler.Instance.avatarIndex); // Set Player Index based on offline scene input
        }
        else
        {
            CmdChangePlayerName($"Player {netId}"); // Set Player Name by netId
        }

        SpawnPlayerAvatar(); // Spawn Player Avatar

        //spawner = FindFirstObjectByType<ObjectSpawner>(); // Spawn interactable objects
        //CmdRequestSpawn(new Vector3(2.926056f, 0, -18.00968f));
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
    #endregion
}
