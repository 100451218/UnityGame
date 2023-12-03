using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// Currently the commander doesn't have into account he can't run into walls, xd

/// </summary>
public class CommanderScript : MonoBehaviour
{   
    public NavMeshAgent agent;
    private Vector3 currentVelocity;


    public Camera Commander;
    public Camera Camera1;
    public Camera Camera2;
    public Camera Camera3;
    public Camera Camera4;
    
    public GameObject move_pointer;
    

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
        Camera observing_point=Camera.main;
        Debug.Log("AAAAAAAAAA");
        if (Physics.Raycast(observing_point.transform.position, observing_point.transform.forward, out hitInfo, 10000))
        {   
            //remember that the camera raycast is not fully front so maybe try to use a game object atached to the camera (an empty)
            Debug.Log(hitInfo.transform.name + hitInfo.transform.position);
            var localHit = transform.InverseTransformPoint(hitInfo.point);
            Debug.Log("Plane point:"+localHit); 
            /*
            if (hitInfo.transform.name=="Floor"){
                //We are touching the floor and therefore we can move there
                agent.destination=hitInfo.point;
            } 
            */
            agent.destination=hitInfo.point;
        }
    }
    

    Vector3 DodgeZombies(){
        LayerMask zombieMask = LayerMask.GetMask("Zombie", "Walls");
        Collider[] zombies = Physics.OverlapSphere(transform.position, 30, zombieMask);
        Vector3 dodgeVector= Vector3.zero;
        foreach(var zombi in zombies){
            Vector3 avoidVector= transform.position-zombi.transform.position;
            dodgeVector=dodgeVector+(avoidVector.normalized/avoidVector.magnitude);
        }
        return dodgeVector.normalized * 5;
    }
    // Update is called once per frame
    void Update()
    {
        
        Vector3 desiredV = Vector3.zero;
        if (agent.hasPath){
            desiredV = agent.desiredVelocity;
        } 

        Vector3 survivingV = DodgeZombies();
        desiredV= desiredV+survivingV;
        currentVelocity = Vector3.Lerp(currentVelocity, desiredV, Time.deltaTime);
        agent.Move(currentVelocity * Time.deltaTime);
        
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