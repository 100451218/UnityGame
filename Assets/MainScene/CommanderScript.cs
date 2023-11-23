using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CommanderScript : MonoBehaviour
{
    public Camera Commander;
    public Camera Camera1;
    public Camera Camera2;
    public Camera Camera3;
    public Camera Camera4;

    // Use this for initialization
    void Start()
    {
        Camera3.enabled = false;
        Commander.enabled= true;
        Camera1.enabled = false;
        Camera2.enabled = false;
        Camera4.enabled = false;
    }
    public void SwitchMainCamera(){
        Camera3.enabled = false;
        Commander.enabled= true;
        Camera1.enabled = false;
        Camera2.enabled = false;
        Camera4.enabled = false;
    }
    public void SwitchTimCamera(){
        Camera3.enabled = true;
        Commander.enabled= false;
        Camera1.enabled = false;
        Camera2.enabled = false;
        Camera4.enabled = false;
    }
    public void SwitchBobCamera(){
        Camera3.enabled = false;
        Commander.enabled= false;
        Camera1.enabled = true;
        Camera2.enabled = false;
        Camera4.enabled = false;
    }
    public void SwitchSamCamera(){
        Camera3.enabled = false;
        Commander.enabled= false;
        Camera1.enabled = false;
        Camera2.enabled = true;
        Camera4.enabled = false;
    }
    public void SwitchUAVCamera(){
        Camera3.enabled = false;
        Commander.enabled= false;
        Camera1.enabled = false;
        Camera2.enabled = false;
        Camera4.enabled = true;
    }

    public void MoveSoldier(string soldier_name){
        GameObject soldier = GameObject.Find(soldier_name);
        Debug.Log(soldier);
        soldier.GetComponent<Soldierscript>().follow=!soldier.GetComponent<Soldierscript>().follow;
    }

    public void Move()
    {
        //work in progress
        RaycastHit hitInfo;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hitInfo, 10000))
        {   
            Debug.Log(hitInfo.transform.name + hitInfo.transform.position);
        }
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