using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BoardAssembler : NetworkBehaviour
{
    
    public int size = 4;
    public Plane[,,] planesMap = new Plane[4,4,4];

    public struct BoardPoint
    {
        public int platform { get;}
        public int row { get;}
        public int plane { get;}

        public BoardPoint(int platform, int row, int plane)
        {
            this.platform = platform;
            this.row = row;
            this.plane = plane;
        }

        public override string ToString()
        {
            return $"({platform}, {row}, {plane})";
        }
    }

    //[ClientRpc]
    public void Bind()
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");

        for (int platformIndex = 0; platformIndex < size ; platformIndex++)
        {
            for (int row = 0; row < size; row++)
            {
                Transform selectedRow = platforms[platformIndex].transform.GetChild(row);

                for (int planeIndex = 0; planeIndex < size; planeIndex++)
                {
                    if (planeIndex < size)
                    {
                        GameObject selectedPlane = selectedRow.GetChild(planeIndex).gameObject;
                        selectedPlane.GetComponent<Plane>().point = new BoardPoint(platformIndex, row, planeIndex);
                        selectedPlane.GetComponent<Plane>().pointStr = new BoardPoint(platformIndex, row, planeIndex).ToString();
                        planesMap[platformIndex, row, planeIndex] = selectedPlane.GetComponent<Plane>();
                    }
                    else
                        break;
                }
            }
        }
    }
}
