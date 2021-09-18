using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Plane : NetworkBehaviour
{
    [SyncVar] public bool Used = false;
    [SyncVar] public uint OwnedBy;
    public BoardAssembler.BoardPoint point;
    [SyncVar]public string pointStr;
    [SyncVar(hook = "ChangeColor")]
    public Color color;

    void ChangeColor(Color oldColor, Color newColor)
    {
        gameObject.GetComponent<MeshRenderer>().material.color = newColor;
    }
}
