using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivillianScript : MonoBehaviour
{
    float time_left;
    // Start is called before the first frame update
    void Start()
    {
        time_left = Random.Range(30f, 90f);
        //Debug.Log(time_left);
    }

    // Update is called once per frame
    void Update()
    {
        time_left=time_left-Time.deltaTime;
        if (time_left<=0){
            Destroy(gameObject);
        }
        Vector3 survivingV = DodgeZombies();
        Debug.Log(gameObject.name+survivingV);
        gameObject.transform.position +=survivingV * Time.deltaTime;
    }
    Vector3 DodgeZombies(){
        LayerMask zombieMask = LayerMask.GetMask("Zombie", "Walls");
        Collider[] zombies = Physics.OverlapSphere(transform.position, 30, zombieMask);
        Vector3 dodgeVector= Vector3.zero;
        foreach(var zombi in zombies){
            Vector3 avoidVector= new Vector3(transform.position.x-zombi.transform.position.x, 0, transform.position.z-zombi.transform.position.z);
            Debug.Log(transform.position+""+avoidVector+""+zombi.transform.position);
            dodgeVector=dodgeVector+(avoidVector.normalized/avoidVector.magnitude);
        }
        return dodgeVector.normalized * 5;
    }
}
