using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Checker : NetworkBehaviour
{
    // Start is called before the first frame update
    public BoardAssembler boardAssembler;
    UI ui;
    GameManager game;

    private void Start()
    {
        boardAssembler = GameObject.FindGameObjectWithTag("BA").GetComponent<BoardAssembler>();
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UI>();
        game = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

    }

    [Client]
    public void Check(Plane selectedPlane, uint userID)
    {
        Debug.Log("checking -> " + userID + "    " + boardAssembler.planesMap.Length);
        Plane[] foundPlanes = CheckBlock(selectedPlane.point, userID).ToArray();
        List<Plane> ownedPlanes = new List<Plane>();
        int count = 0;

        foreach (Plane plane in foundPlanes)
        {
            BoardAssembler.BoardPoint diff = Difference(selectedPlane.point, plane.point);

            //Debug.Log(selectedPlane.point.ToString() + diff.ToString());

            int planeMax = SetToMax(diff.plane);
            int rowMax = SetToMax(diff.row);
            int platMax = SetToMax(diff.platform);

            for (int i = 0; i < boardAssembler.size; i++)
            {
                // Debug.Log(">>>>>>>>><<<<<<<<<< " + userID);
                //Debug.Log(platMax + " |" + rowMax + " |" + planeMax + " > " + diff.row);
                //Debug.Log("---->>> " + (Sum(ref platMax, diff.platform, selectedPlane.point.platform)) + " " + (Sum(ref rowMax, diff.row, selectedPlane.point.row)) + " " + (Sum(ref planeMax, diff.plane, selectedPlane.point.plane)));
                int plat = Sum(ref platMax, diff.platform, selectedPlane.point.platform);
                int row = Sum(ref rowMax, diff.row, selectedPlane.point.row);
                int _plane = Sum(ref planeMax, diff.plane, selectedPlane.point.plane);

                Plane currentPlane = currentPlane = boardAssembler.planesMap[plat, row, _plane];
                    
                ui.middleText += $"{currentPlane.point.ToString()}";//$"{diff}  {plat} {row} {_plane}";

                currentPlane.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;

                if (currentPlane.OwnedBy == userID)
                {
                    //Debug.Log("<><><><><>");
                    //currentPlane.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
                    count++;
                    ownedPlanes.Add(currentPlane);
                    if (count >= boardAssembler.size - 1)
                    {
                        foreach (Plane ownedPlane in ownedPlanes)
                        {
                            ownedPlane.color = Color.green;//ownedPlane.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                            //ownedPlane.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                        }

                        //ui.middleText = "GameOver!";
                        game.isStartable = false;

                        break;
                    }
                }
            }

            count = 0;
            ownedPlanes.Clear();

            //Debug.Log(selectedPlane.point.ToString() + " " + plane.point.ToString() + " " + dir.ToString());


            //SetToMax
        }
       // if (foundPlane != null)
         //   foundPlane.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
    }

    public List<Plane> CheckBlock(BoardAssembler.BoardPoint point, uint userID)
    {
        List<Plane> planesFound = new List<Plane>();

        for (int plat = 0; plat < 3; plat++)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    int rowStartPoint = point.row - 1;
                    int planeStartPoint = point.plane - 1;
                    int platStartPoint = point.platform - 1;

                    if (platStartPoint + plat < boardAssembler.size && platStartPoint + plat >= 0)
                    {
                        if ((rowStartPoint + k < boardAssembler.size && planeStartPoint + i < boardAssembler.size) && (rowStartPoint + k >= 0 && planeStartPoint + i >= 0))
                        {
                            Plane currentPlane = boardAssembler.planesMap[platStartPoint + plat, rowStartPoint + k, planeStartPoint + i];
                            //Debug.Log(currentPlane);
                            if (currentPlane.OwnedBy == userID)
                            {
                                planesFound.Add(currentPlane);
                            }
                        }
                    }
                }
            }
        }

        return planesFound;
    }

    BoardAssembler.BoardPoint Difference(BoardAssembler.BoardPoint point1, BoardAssembler.BoardPoint point2)
    {
        int platDif = point2.platform - point1.platform;
        int rowDif = point2.row - point1.row;
        int planeDif = point2.plane - point1.plane;

        return new BoardAssembler.BoardPoint(platDif, rowDif, planeDif);
    }

    int SetToMax(int val)
    {
        if (val < 0)
            return boardAssembler.size - 1;
        else if (val > 0)
            return 0;

        return val;
    }

    int Sum(ref int num1, int num2, int num3)
    {
        int awns = num1;

        if (num2 < 0)
        {
           // Debug.Log("---------------<<<<>>>>>>> " + num2);
            awns = num1 - Mathf.Abs(num2);
            num1 = awns;
            return awns + 1;
        }
        else if (num2 > 0)
        {
            awns = num1 + num2;
            num1 = awns;
            return awns - 1;
        }

        return num3;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit!");
    }
}
