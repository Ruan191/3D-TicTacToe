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
    GameObject spawnPoint;

    public float maxAllowedDistance = 45;


    float horizontalSpeed = 2.0f;
    float verticalSpeed = 2.0f;
    public float speed = 10.0f;
    bool cursorHidden;


    void Start()
    {
        cam = GetComponent<Camera>();
        dir = transform.GetChild(0).gameObject;
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UI>();
        spawnPoint = GameObject.Find("SpawnPoint");
        maxAllowedDistance = 45;

        transform.position = spawnPoint.transform.position;

        if (isLocalPlayer)
        {
            cam.enabled = true;
            EnableCursorHide();
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

    float side;
    float translation;
    float h;
    float v;

    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.End))
                Application.Quit();

            if (Vector3.Distance(board.transform.position, transform.position) >= maxAllowedDistance)
            {
                transform.position = spawnPoint.transform.position;
            }

            translation = Input.GetAxis("Vertical") * speed;
            side = (Input.GetAxis("Horizontal") * speed);
            float top = 0;

            if (Input.GetKey(KeyCode.Space))
            {
                top = 1 * speed;
            }else if (Input.GetKey(KeyCode.C))
            {
                top = -1 * speed;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (cursorHidden)
                {
                    DisableCursorHide();
                }
                else
                {
                    EnableCursorHide();
                }
            }

            top *= Time.deltaTime;
            translation *= Time.deltaTime;
            side *= Time.deltaTime;

            transform.Translate(side, top, translation);

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

    void EnableCursorHide()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorHidden = true;
    }

    void DisableCursorHide()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorHidden = false;
    }
}
