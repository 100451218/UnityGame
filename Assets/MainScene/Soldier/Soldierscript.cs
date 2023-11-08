using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldierscript : MonoBehaviour
{
    public float lookrange = 1000f;
    public float shootrange = 400f;
    public float shootsound = 2000f;
    public Material looked;
    Renderer Enemie_Renderer;
    GameObject target;
    EnemieScript enemie_script;
    float target_distance;
    private Vector3 defaultLook;
    void Start(){
        defaultLook= transform.forward;
    }
    void Update()
    {
        (target, target_distance)=ClosestEnemy();
        if (target_distance<lookrange){
            /*
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, target.transform.position, 1.0f*Time.deltaTime, 10.0f);
            newDirection.y=0f;
            transform.rotation = Quaternion.LookRotation(newDirection);
            */

            
            //transform.LookAt(target.transform);
            
            //Best functional and natural way to make soldiers look at the enemy
            var rotation = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 3);

            target.GetComponent<MeshRenderer>().material = looked;
        } else {
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, defaultLook   , 1.0f*Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
        if (target_distance<shootrange)
        {
        //Too close, shoot
            Shoot();
        }
        
    }
    (GameObject, float) ClosestEnemy(){
        GameObject[] enemies;
        enemies=GameObject.FindGameObjectsWithTag("Enemie");
        GameObject closest=null;
        float distance = 10000000000000;
        //Should be inifinity but it does not work
        Vector3 position = transform.position;
        foreach (GameObject enemie in enemies){
            Vector3 enemie_distance = enemie.transform.position - position;
            float currentDistance = enemie_distance.sqrMagnitude;
            Enemie_Renderer= enemie.GetComponent<Renderer>();
            //We get that enemie renderer
            Vector3 direction = enemie.transform.position- this.transform.position;
            float angle= Vector3.Angle(direction, this.transform.forward);
            //Now we need to check if it is the closest one the soldier sees.
            if (currentDistance < distance && (angle>-60 && angle < 60)){
                closest=enemie;
                distance = currentDistance;
            }
        }
        
        return (closest, distance);
    }
    void Shoot()
    {
        //We need to make the sound of a shooting
        
        GameObject[] all_zombies;
        all_zombies = GameObject.FindGameObjectsWithTag("Enemie");
        foreach ( GameObject zombi in all_zombies){
            if (Vector3.Distance(zombi.transform.position, gameObject.transform.position)<1000){
                //if the zombi is close enough
                enemie_script=zombi.GetComponent<EnemieScript>();
                enemie_script.Investigate(gameObject.transform.position);
            }
        }


        //transform.LookAt(target.transform);
        //Transform.RotateTowards would be
        RaycastHit hitInfo;
        
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hitInfo, lookrange))
        {
            Debug.Log(hitInfo.transform.gameObject);
            if (hitInfo.transform.gameObject.tag=="Enemie"){
                
                Destroy(hitInfo.transform.gameObject);     
            }
            // Additional hit effects can be implemented here
        }
    }
}
