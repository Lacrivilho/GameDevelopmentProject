using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{

    private bool[] passageConnected = new bool[0];
    
    public void setConnections(bool[] connections)
    {
        passageConnected = connections;
    }

    public bool[] getConnections()
    {
        return passageConnected;
    }

    public void connectPassage(int index)
    {
        passageConnected[index] = true;
    }

    public bool checkIndex(int index)
    {
        return passageConnected[index];
    }
}
