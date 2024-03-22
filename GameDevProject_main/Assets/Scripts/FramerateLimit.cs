using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateLimit : MonoBehaviour
{
    public int framerate = 60;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = framerate;
    }
}
