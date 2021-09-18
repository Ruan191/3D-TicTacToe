using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
    static List<GameObject> spawnablePrefabs = new List<GameObject>();
    GameManager game;
    public BoardAssembler board;

    public override void Start()
    {
        base.Start();
        spawnablePrefabs = spawnPrefabs;
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        GameObject gameObject = Instantiate(spawnablePrefabs.Find(prefab => prefab.name == "Y"));
        NetworkServer.Spawn(gameObject, conn);

        if (numPlayers == 2)
        {
            game.isStartable = true;
            game.allowedPlayer = game.playerIds[0];
        }
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
        game = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        
        
    }

    public static GameObject SpawnGameObject(string prefabName, Vector3 pos, Quaternion quaternion, Transform parent = null)
    {
        GameObject gameObject = Instantiate(spawnablePrefabs.Find(prefab => prefab.name == prefabName), pos, quaternion, parent);
        NetworkServer.Spawn(gameObject);
        return gameObject;
    }

    
}
