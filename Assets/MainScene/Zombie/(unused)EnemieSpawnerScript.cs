using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemieSpawnerScript : MonoBehaviour
{
    public GameObject objectToSpawn;
    public int probability=200;
    private float x,z;
    private int selection;
    private int spawnpoints_count;
    private Transform[] spawnpoints;
    // Start is called before the first frame update
    void Start()
    {
        spawnpoints_count = transform.childCount;
        spawnpoints = new Transform[spawnpoints_count];
        for(int i = 0; i < spawnpoints_count; i++){
            spawnpoints[i]= transform.GetChild(i);
        }
        Debug.Log(spawnpoints);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Random.Range(1, probability)==3){
            //Debug.Log("Spawn");
            selection=Random.Range(0, spawnpoints_count);
            x= spawnpoints[selection].position.x +Random.Range(-10, 10);
            z= spawnpoints[selection].position.z +Random.Range(-5, 5);
           
            Instantiate(objectToSpawn, new Vector3(x,1,z),Quaternion.identity);
        }
        
    }
}
