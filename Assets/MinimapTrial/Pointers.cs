using UnityEngine;
using UnityEngine.UI;

public class Pointers : MonoBehaviour
{
    public Camera worldCamera;       // Reference to the main camera
    public RenderTexture minimapTexture;
    public GameObject enemyMarkerPrefab;
    public RectTransform canvasRectTransform;
    public Image zombieImage;
    public Image turretImage;
    public Image CivillianMarkerImage;
    public Image civillianImage;
    public Image CommanderImage;
    private Transform origin, final;
    private float x_ratio, y_ratio;
    
    void Start()
    {
        origin = GameObject.Find("origin").transform;
        final = GameObject.Find("final").transform;
        Vector3 distance_relative = origin.InverseTransformPoint(final.position);

        Vector3 [] v = new Vector3[4];
        //puede que sea con 300 en vez de lo de v[0]], v[3]
        canvasRectTransform.GetWorldCorners(v);
         //Debug.Log(canvasRectTransform.gameObject.name+" "+v[0]+" "+v[1]+" "+v[2]+" "+v[3]);

        x_ratio= Mathf.Abs((v[0].x-v[2].x)/(distance_relative.x));
        y_ratio= Mathf.Abs((v[0].z-v[2].z)/(distance_relative.z));

        x_ratio= Mathf.Abs((300)/(distance_relative.x));
        y_ratio= Mathf.Abs((300)/(distance_relative.z));
         // //Debug.Log((distance_relative, x_ratio, y_ratio));
    }
        
    
    void FixedUpdate()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemie");
        GameObject[] soldiers = GameObject.FindGameObjectsWithTag("Allie");
        GameObject[] civillians = GameObject.FindGameObjectsWithTag("Civilian");
        GameObject[] civillian_markers = GameObject.FindGameObjectsWithTag("Civillian spawn");
        Vector3 position;
         //Debug.Log(enemies);
        //First we delete all the icons of the enemies
        foreach(Transform delete_image in transform){
            GameObject.Destroy(delete_image.gameObject);
        }
        foreach (GameObject enemie in enemies)
        {
            //This rectangle has the canvas data
            //Part 0, get canvas data.
            Vector3 [] v = new Vector3[4];
            canvasRectTransform.GetWorldCorners(v);
            // //Debug.Log(canvasRectTransform.gameObject.name+" "+v[0]+" "+v[1]+" "+v[2]+" "+v[3]);
            canvasRectTransform.GetWorldCorners(v);
            // //Debug.Log(canvasRectTransform.gameObject.name+" "+v[0]+" "+v[1]+" "+v[2]+" "+v[3]);
            //Vector3 new_position = new Vector3(v[0].x, v[0].z, v[0].y);
            //Vector3 new_position = new Vector3(0, 0, v[0].y);

            //Part1 get local position of the zombie from his world position.
            //Position (0,0) is the local position of the canvas for (0,0) which in the real world is (-81.6, 136) The other corner is (300,-300)
            // We need to obtain the local position of the zombies given the top left is 0,0.
            
            Vector3 enemy_world_position = enemie.transform.position;
            Vector2 enemy_marker_position = canvasRectTransform.InverseTransformPoint(enemy_world_position);
            //This only gives the distance from the zombie to the canvas rectange origin. What I want is to get 
            // //Debug.Log(enemie.name+"World: "+ enemy_world_position+ "Local1: "+ enemy_marker_position+ "Local2: "+ enemy_marker_position);
            //Note: maybe it is needed to invert the y


            //Part2 from what the camera sees get what would be the position corresponding the gameobject 
            
            Vector3 distance_relative = origin.InverseTransformPoint(enemy_world_position);

             //Debug.Log("origin coords"+ origin+ " distance from"+enemie.name+":"+distance_relative);
             //Debug.Log("Which is "+ distance_relative.x*x_ratio+" "+distance_relative.z*y_ratio);
            /*
            Vector3 new_position3=worldCamera.WorldToScreenPoint(enemie.transform.position);
            Vector2 new_position = new Vector2 (new_position3.x, new_position3.z-300 );
             //Debug.Log(enemie.name+"3d"+new_position3+"2d: "+ new_position);

            */
            //Part3 Get the image of a zombie on the local position. As it is the son the position will be relative.
            Vector2 new_position = new Vector2((distance_relative.x*x_ratio),(distance_relative.z*y_ratio));
            Image image = Instantiate(zombieImage, new_position, Quaternion.identity);
            image.transform.SetParent(canvasRectTransform, false);

            //To verify positions
            //Image image2 = Instantiate(imagePrefab, new Vector2(300,-300), Quaternion.identity);
            //image2.transform.SetParent(canvasRectTransform, false);
            //Vector2 canvas_position = 
            // //Debug.Log(enemie.name+""+enemie.transform.position);
        }
        foreach (GameObject soldier in soldiers)
        {            
            Vector3 distance_relative = origin.InverseTransformPoint(soldier.transform.position);
            Vector2 new_position = new Vector2((distance_relative.x*x_ratio),(distance_relative.z*y_ratio));
            Image image = Instantiate(turretImage, new_position, Quaternion.identity);
            image.transform.SetParent(canvasRectTransform, false);
        }
        foreach (GameObject civillian in civillians)
        {            
            Vector3 distance_relative = origin.InverseTransformPoint(civillian.transform.position);

            Vector2 new_position = new Vector2((distance_relative.x*x_ratio),(distance_relative.z*y_ratio));
            Image image = Instantiate(civillianImage, new_position, Quaternion.identity);
            image.transform.SetParent(canvasRectTransform, false);
        }
        foreach (GameObject marker in civillian_markers)
        {            
            Vector3 distance_relative = origin.InverseTransformPoint(marker.transform.position);

            Vector2 new_position = new Vector2((distance_relative.x*x_ratio),(distance_relative.z*y_ratio));
            Image image = Instantiate(CivillianMarkerImage, new_position, Quaternion.identity);
            image.transform.SetParent(canvasRectTransform, false);
        }
        Transform commander = GameObject.Find("Commander").transform;
        Vector3 c_distance_relative = origin.InverseTransformPoint(commander.position);

        Vector2 c_new_position = new Vector2((c_distance_relative.x*x_ratio),(c_distance_relative.z*y_ratio));
        Image c_image = Instantiate(CommanderImage, c_new_position, Quaternion.identity);
        c_image.transform.SetParent(canvasRectTransform, false);
    }

}
