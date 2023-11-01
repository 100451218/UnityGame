using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldierscript : MonoBehaviour
{
    public float lookrange = 1000f;
    public float shootrange = 400f;
    public Camera NPCam;
    public Material looked;
    Renderer Enemie_Renderer;
    GameObject target;
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
            transform.LookAt(target.transform);

            //var rotation = Quaternion.LookRotation(target.transform.position - transform.position);
            //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);

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
            Vector3 viewportPosition = NPCam.WorldToViewportPoint(Enemie_Renderer.transform.position);
            //Now we need to check if it is the closest one the soldier sees.
            if (currentDistance < distance && viewportPosition.x >= 0 && viewportPosition.x <= 1 &&
               viewportPosition.y >= 0 && viewportPosition.y <= 1 &&
               viewportPosition.z > 0){
                closest=enemie;
                distance = currentDistance;
            }
        }
        
        return (closest, distance);
    }
    void Shoot()
    {
        //transform.LookAt(target.transform);
        //Transform.RotateTowards would be
        RaycastHit hitInfo;
        
        if (Physics.Raycast(NPCam.transform.position, NPCam.transform.forward, out hitInfo, lookrange))
        {
            if (hitInfo.transform.gameObject.tag=="Enemie"){
                
                Destroy(hitInfo.transform.gameObject);     
            }
            // Additional hit effects can be implemented here
        }
    }
}
