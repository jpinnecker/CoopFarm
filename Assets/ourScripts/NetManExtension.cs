using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System;
using System.Linq;
using System.Collections.Generic;

public class NetManExtension : NetworkManager {

    [Scene][SerializeField] private string onlineScene = string.Empty; 

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public static event Action OnServerStopped;

    public List<NetworkBehaviour> GamePlayers { get; } = new List<NetworkBehaviour>();

    public override void OnStartClient() {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs) {
            ClientScene.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect(NetworkConnection conn) {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn) {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn) {
        if (numPlayers >= maxConnections) {
            conn.Disconnect();
            return;
        }

        if (SceneManager.GetActiveScene().name != onlineScene) { // Change to onlineScene ?
            conn.Disconnect();
            return;
        }
    }

    public override void OnStopServer() {
        OnServerStopped?.Invoke();
        GamePlayers.Clear();
    }

    public override void OnServerReady(NetworkConnection conn) {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }
}
