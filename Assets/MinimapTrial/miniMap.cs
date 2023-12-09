using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class miniMap : MonoBehaviour {
	public Vector3 fixedPosition = new Vector3(-1200, 200f,-48.7f);
    public Vector3 fixedRotation = new Vector3(0f, 0f, 0f); // Adjust as needed

    void LateUpdate()
    {
		//Debug.Log(fixedPosition);
        // Keep the camera's position fixed
        gameObject.transform.position = fixedPosition;
		//Debug.Log(gameObject.transform.position+ ""+ gameObject.name);
        // Set the camera's rotation to a fixed value
        gameObject.transform.rotation = Quaternion.Euler(fixedRotation);
    }
	 
}
