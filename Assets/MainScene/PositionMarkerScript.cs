using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionMarkerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject civilian;
    private float counter = 0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        counter = counter + Time.deltaTime;
        if (counter>15){
            Instantiate(civilian, this.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
