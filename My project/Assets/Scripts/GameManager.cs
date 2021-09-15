using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    [SyncVar] public bool isStartable = false;
    [SyncVar] public bool isBluesTurn = true;
    [SyncVar] public List<uint> playerIds = new List<uint>();
    [SyncVar] public uint allowedPlayer;
}
