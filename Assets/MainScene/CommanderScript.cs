using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CommanderScript : MonoBehaviour
{
    public Camera Commander;
    public Camera Camera1;
    public Camera Camera2;
    public Camera Camera3;

    // Use this for initialization
    void Start()
    {
        Camera3.enabled = false;
        Commander.enabled= true;
        Camera1.enabled = false;
        Camera2.enabled = false;
        
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            Commander.enabled = true;
            Camera1.enabled = false;
            Camera2.enabled = false;
            Camera3.enabled = false;
        }
        if (Input.GetKeyDown("2"))
        {
            Commander.enabled = false;
            Camera1.enabled = true;
            Camera2.enabled = false;
            Camera3.enabled = false;
        }
        if (Input.GetKeyDown("3"))
        {
            Commander.enabled = false;
            Camera1.enabled = false;
            Camera2.enabled = true;
            Camera3.enabled = false;
        }
        if (Input.GetKeyDown("4"))
        {
            Commander.enabled = false;
            Camera1.enabled = false;
            Camera2.enabled = false;
            Camera3.enabled = true;
        }
    }

}