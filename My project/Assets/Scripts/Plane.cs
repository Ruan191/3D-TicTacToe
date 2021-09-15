using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Plane : NetworkBehaviour
{
    [SyncVar] public bool Used = false;
    [SyncVar] public uint OwnedBy;
    /*[SyncVar] public List<Plane> bottom = new List<Plane>();
    [SyncVar] public List<Plane> middle = new List<Plane>();
    [SyncVar] public List<Plane> top = new List<Plane>();
    [SyncVar] public List<List<Plane>> platforms;*/
    public BoardAssembler.BoardPoint point;
    //GameObject gameObject;
    [SyncVar(hook = "ChangeColor")]
    public Color color;

    private void Awake()
    {
        //gameObject = transform.gameObject;
        //platforms = new List<List<Plane>>() { top, middle, bottom };
    }

    void ChangeColor(Color oldColor, Color newColor)
    {
        gameObject.GetComponent<MeshRenderer>().material.color = newColor;
    }
}
