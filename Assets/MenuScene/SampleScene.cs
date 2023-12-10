using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SampleScene : MonoBehaviour
{
    float counter=0f;
    bool enter=false;
    public GvrReticlePointer pointer;
    public void Enter()
    {
        enter=true;
        pointer.reticleGrowthSpeed=0.8f;
        
    }
    public void Leave()
    {
        counter=0f;
        enter=false;
        pointer.reticleGrowthSpeed=20.0f;
        Debug.Log("He left");
        GetComponent<Renderer>().material.color = Color.white;
    }

    void Start(){
        pointer.reticleGrowthSpeed=0.3f;
    }
    void Update(){
        if (enter==true){
            counter= counter + Time.deltaTime;
            
        } else {
            counter=0;
        }
        
        if (counter>1.5){
            GetComponent<Renderer>().material.color = Color.black;
            switch (this.gameObject.name)
            {
                case "Game1":
                    Game1Button();   
                    break;
                case "Game2":
                    Game2Button();
                    break;
                case "GameTutorial":
                    TutorialButton();
                    break;
            }
            
        }
    }

    void Game1Button(){
        Debug.Log("Play");
        SceneManager.LoadScene(sceneName: "Game1");
    }
    void Game2Button(){
        Debug.Log("Play");
        SceneManager.LoadScene(sceneName: "Game2");
    }
    void TutorialButton(){
        Debug.Log("Play");
        SceneManager.LoadScene(sceneName: "Tutorial");
    }
}
