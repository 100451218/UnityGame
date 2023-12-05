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
        /*
        int x, y, z;
        x = Random.Range(0, 1000);
        y = Random.Range(10, 7);
        z = Random.Range(0, 1000);
        */
        return new Vector3(Random.Range(-280, 200), 0, Random.Range(-234, 250));
    }
    void Start()
    {
        zombies_counter=1;
        civilians_counter=1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Random.Range(1, (zombies_counter+civilians_counter)*5)==1){
            Vector3 spawnpoint=LocateSpawnPoint();
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnpoint, out hit, Mathf.Infinity, NavMesh.AllAreas)){
                var myRandomPositionInsideNavMesh = hit.position;
                //Debug.Log(myRandomPositionInsideNavMesh);
                //The provisional not studied ratio of zombie civilian would be 1 civilian per 5 zombies
                if (civilians_counter*5>zombies_counter){
                    Instantiate(zombi, myRandomPositionInsideNavMesh,Quaternion.identity);
                    zombies_counter++;
                } else {
                    Instantiate(civilian, myRandomPositionInsideNavMesh,Quaternion.identity);
                    civilians_counter++;
                }
            }
        }
    }
}
