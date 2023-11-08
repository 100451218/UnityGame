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
    }
}
