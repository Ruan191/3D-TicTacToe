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
    RaycastHit hit;
    GameManager game;
    Checker checker;
    BoardAssembler board;
    GameObject dir;
    UI ui;

    float horizontalSpeed = 2.0f;
    float verticalSpeed = 2.0f;
    public float speed = 10.0f;


    void Start()
    {
        cam = GetComponent<Camera>();
        dir = transform.GetChild(0).gameObject;
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UI>();

        if (isLocalPlayer)
        {
            cam.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            cam.enabled = false;
        }

        game = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        checker = GameObject.FindGameObjectWithTag("Checker").GetComponent<Checker>();
        board = GameObject.FindGameObjectWithTag("BA").GetComponent<BoardAssembler>();
        game.playerIds.Add(netId);
        board.Bind();
    }

    Vector3 Dir;
    float side;
    float translation;
    float h;
    float v;

    void Update()
    {
        if (isLocalPlayer)
        {
            translation = Input.GetAxis("Vertical") * speed;
            side = (Input.GetAxis("Horizontal") * speed);

            translation *= Time.deltaTime;
            side *= Time.deltaTime;

            transform.Translate(side, 0, translation);

            h += horizontalSpeed * Input.GetAxis("Mouse X");
            v += verticalSpeed * Input.GetAxis("Mouse Y");

            transform.Rotate(-v, h, 0);

            transform.eulerAngles = new Vector3(-v, h, 0);

            if (Input.GetMouseButtonUp((int)Mouse.LeftClick))
            {         
                if (Physics.Raycast(transform.position, dir.transform.position - transform.position, out hit) && netId == game.allowedPlayer && game.isStartable)//, 500, 1 << 6))
                {
                    IdAllowedToPlay();

                    GameObject target = hit.transform.gameObject;
                    Plane targetPlane = target.GetComponent<Plane>();

                    if (!targetPlane.Used)
                        SpawnObject(target);
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
            ui.topText = "Reds Turn!";
        }
        else
        {
            game.isBluesTurn = true;
            game.allowedPlayer = players[1];
            ui.topText = "Blues Turn!";
        }
    }
}
