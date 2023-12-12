using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnerControl : MonoBehaviour
{
    // Start is called before the first frame update
    private int zombies_counter;
    private int civilians_counter;
    public GameObject civilian;
    public GameObject zombi;

    Vector3 LocateSpawnPoint()
    {
        //We set up a random point in the map and return it
        return new Vector3(Random.Range(-280, 200), 0, Random.Range(-234, 250));
    }
    void Start()
    {
        //at the start the npcs counters will be set to 1
        zombies_counter=1;
        civilians_counter=1;
    }

    // Update is called once per frame
    void Update()
    {
        //We need to generate a random number based on the ammount of zombies and civilians and if that number is one we will spawn a NPC
        // As it can be see the probability of a spawn is reduced the more npcs have been spawned until it will be almost impossible for a creature to spawn
        if (Random.Range(1, (zombies_counter+civilians_counter)*5)==1){
            //There is a fith of the total ammount of creatures previously spawned as the probability of spawn
            Vector3 spawnpoint=LocateSpawnPoint();
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnpoint, out hit, Mathf.Infinity, NavMesh.AllAreas)){
                //This function will get us the closest point within the navmesh from the given random point
                var myRandomPositionInsideNavMesh = hit.position;
                //Debug.Log(myRandomPositionInsideNavMesh);
                //The provisional not studied ratio of zombie civilian would be 1 civilian per 5 zombies. Given some test gameplay this ratio could have been modified
                if (civilians_counter*5>zombies_counter){
                    //This also unestudied ratio forces to be five times more zombies than civilians
                    Instantiate(zombi, myRandomPositionInsideNavMesh,Quaternion.identity);
                    zombies_counter++;
                } else {
                    //Spawn the civilian if there is too few
                    Instantiate(civilian, myRandomPositionInsideNavMesh,Quaternion.identity);
                    civilians_counter++;
                }
            }
            //In summary, the spawn sequence would be Z-Z-Z-Z-Z-C-Z-Z-Z-Z-Z-C...
        }
    }
}
