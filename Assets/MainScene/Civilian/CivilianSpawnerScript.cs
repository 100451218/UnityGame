using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianSpawnerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject objectToSpawn;
    public int probability=1000;
    private float x,z;
    private int selection;
    private int spawnpoints_count;
    private Transform[] spawnpoints;
    public GameObject[] soldiers_position;
    bool can_spawn;
    // Start is called before the first frame update
    void Start()
    {
        spawnpoints_count = transform.childCount;
        spawnpoints = new Transform[spawnpoints_count];
        for(int i = 0; i < spawnpoints_count; i++){
            spawnpoints[i]= transform.GetChild(i);
        }
        Debug.Log(spawnpoints);
        soldiers_position = GameObject.FindGameObjectsWithTag("Allie");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //we used fixed update because we want the script to run every fixed ammount of seconds, not every frame
        if (Random.Range(1, probability)==3){

            Debug.Log("Spawn");
            can_spawn= true;
            selection=Random.Range(0, spawnpoints_count);
            foreach (GameObject soldier in soldiers_position){
                if (Vector3.Distance(soldier.transform.position, spawnpoints[selection].transform.position)<20){
                    //If the spawner is in a zone the soldier is already guarding
                    can_spawn =false;
                } 
            }
            if (can_spawn==true){
                x= spawnpoints[selection].position.x +Random.Range(-10, 10);
                z= spawnpoints[selection].position.z +Random.Range(-5, 5);      
                Instantiate(objectToSpawn, new Vector3(x,2,z),Quaternion.identity);
            }

        }
        
    }
}
