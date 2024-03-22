using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageController : MonoBehaviour
{
    private bool passageConnected = false;

    public void setConnection(bool connection)
    {
        passageConnected = connection;
    }

    public bool getConnection()
    {
        return passageConnected;
    }

    public void connectPassage()
    {
        passageConnected = true;
    }

    public void cutPassage()
    {
        passageConnected = false;
    }
}
