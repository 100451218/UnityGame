using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionMarkerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject civilian;
    private float counter = 0f;
    

    // Update is called once per frame
    void Update()
    {
        //Whenever the civilian marker spawns it will wait 15 seconds to destroy himself and spawns the civilian
        counter = counter + Time.deltaTime;
        if (counter>15){
            //If 15 seconds past, spawn the civilian
            Instantiate(civilian, this.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
