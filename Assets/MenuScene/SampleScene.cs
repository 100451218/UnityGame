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
        //We set the variable to true as the pointer has entered the cube
        enter=true;
        //We adjust the growth to provide feedback
        pointer.reticleGrowthSpeed=0.8f;
        
    }
    public void Leave()
    {
        //If the user stops looking at the cube we reset the counter and set the variable to false
        counter=0f;
        enter=false;
        pointer.reticleGrowthSpeed=20.0f;
        GetComponent<Renderer>().material.color = Color.white;
        //We also make the pointer instally small and change the cube colour back to white
    }

    void Start(){
        pointer.reticleGrowthSpeed=0.3f;
    }
    void Update(){
        if (enter==true){
            //if we are looking at the cube we keep counting
            counter= counter + Time.deltaTime;
            
        } else {
            //If we leave we set the counter to 0 (redundant to prevent errors)
            counter=0f;
        }
        
        if (counter>1.5){
            //If we get past 1.5 seconds we change the colour and set the new scene
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
        Debug.Log("Game1");
        SceneManager.LoadScene(sceneName: "Game1");
    }
    void Game2Button(){
        Debug.Log("Game2");
        SceneManager.LoadScene(sceneName: "Game2");
    }
    void TutorialButton(){
        Debug.Log("Tutorial");
        SceneManager.LoadScene(sceneName: "Tutorial");
    }
}
