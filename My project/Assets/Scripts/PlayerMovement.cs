using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

enum Mouse
{
    RightClick = 1,
    LeftClick = 0
}

public class PlayerMovement : NetworkBehaviour
{
    public Camera cam;
    Ray ray;
    RaycastHit hit;
    GameManager game;
    Checker checker;
    BoardAssembler board;
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        game = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        checker = GameObject.FindGameObjectWithTag("Checker").GetComponent<Checker>();
        board = GameObject.FindGameObjectWithTag("BA").GetComponent<BoardAssembler>();
        game.playerIds.Add(netId);
        board.Bind();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer && game.isStartable)
        {
            
            if (Input.GetKeyDown(KeyCode.W))
            {
                //transform.position += Vector3.forward;
                
            }

            if (Input.GetMouseButtonUp((int)Mouse.LeftClick))
            {
                //Debug.Log(netId + " allowed player = " + game.allowedPlayer);
                ray = cam.ScreenPointToRay(Input.mousePosition);
                
                if (Physics.Raycast(ray, out hit) && netId == game.allowedPlayer)//, 500, 1 << 6))
                {
                    IdAllowedToPlay();

                    GameObject target = hit.transform.gameObject;
                    Plane targetPlane = target.GetComponent<Plane>();
                    //target.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);

                    if (!targetPlane.Used)
                        SpawnObject(target);

                    //targetPlane.OwnedBy = netId;
                    //checker.Check(target.GetComponent<Plane>(), netId);
                    //target.GetComponent<NetworkIdentity>().RemoveClientAuthority();
                    //target.GetComponent<MeshRenderer>().material.color = Color.red;

                    //checker.transform.position = target.transform.position;
                    //MyNetworkManager.SpawnGameObject("Cube", target.transform.position, target.transform.rotation);
                }
            }
        }
    }

    [Command]
    public void SpawnObject(GameObject target)
    {
        Bounds bounds = target.GetComponent<Renderer>().bounds;
        checker.transform.position = bounds.center + Vector3.up;
        // game.isBluesTurn.Switch();
        //Debug.Log(netIdentity.netId);
        checker.Check(target.GetComponent<Plane>(), netId);
        target.GetComponent<Plane>().OwnedBy = netId;

        if (netId == game.playerIds[0])
        {
            GameObject gameObject = MyNetworkManager.SpawnGameObject("CubeRed", (bounds.center + Vector3.up), target.transform.rotation);
        }
        else
        {
            GameObject gameObject = MyNetworkManager.SpawnGameObject("CubeBlue", (bounds.center + Vector3.up), target.transform.rotation);
        }

        //new Vector3(target.transform.position.x - bounds., target.transform.position.y - target.transform.lossyScale.y), target.transform.rotation);
        target.GetComponent<Plane>().Used = true;
    }

    [Command]
    void IdAllowedToPlay()
    {
        uint[] players = game.playerIds.ToArray();

        if (game.isBluesTurn)
        {
            game.isBluesTurn = false;
            game.allowedPlayer = players[0];
        }
        else
        {
            game.isBluesTurn = true;
            game.allowedPlayer = players[1];
        }
    }
}
