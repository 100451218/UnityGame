using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using KKSpeech;
using UnityEngine.SceneManagement;

public class RecordingCanvas : MonoBehaviour
{
  //We set up the variables 
  private bool Tim=false;
  private bool Sam=false;
  private bool Bob=false;
  //Created because the speech to text has started to malfunction for no reason and calls 4 times
  public Text resultText;
  public Text soldier_status;
  private float waiter=0f;
  CommanderScript voice_control;
  void Start()
  {
    
    voice_control=FindObjectOfType<CommanderScript>();
    if (SpeechRecognizer.ExistsOnDevice())
    {
      SpeechRecognizerListener listener = GameObject.FindObjectOfType<SpeechRecognizerListener>();
      listener.onAuthorizationStatusFetched.AddListener(OnAuthorizationStatusFetched);
      listener.onAvailabilityChanged.AddListener(OnAvailabilityChange);
      listener.onErrorDuringRecording.AddListener(OnError);
      listener.onErrorOnStartRecording.AddListener(OnError);
      listener.onFinalResults.AddListener(OnFinalResult);
      listener.onPartialResults.AddListener(OnPartialResult);
      listener.onEndOfSpeech.AddListener(OnEndOfSpeech);
      SpeechRecognizer.RequestAccess();
    }
    else
    {
      resultText.text = "Sorry, but this device doesn't support speech recognition";
      //startRecordingButton.enabled = false;
    }


  }

  void Update(){
    //OnStartRecordingPressed();
    //Debug.Log(this.gameObject.transform.parent.transform.parent.parent);
    string temp1;
    string temp2;
    string temp3;
    if (Tim==false){
      temp1="<color=teal>Tim</color>: <color=red>patrol</color>";
    } else{
      temp1="<color=teal>Tim</color>: <color=green>following</color>";
    }
    if (Sam==false){
      temp2="<color=maroon>\nSam</color>: <color=red>patrol</color>";
    } else{
      temp2="<color=maroon>\nSam</color>: <color=green>following</color>";
    }
    if (Bob==false){
      temp3="<color=orange>\nBob</color>: <color=red>patrol</color>";
    } else{
      temp3="<color=orange>\nBob</color>: <color=green>following</color>";
    }
    soldier_status.text=temp1+temp2+temp3;
    if (Vector3.Dot(this.transform.forward, Vector3.down)>0.85)
    {
      //Vector3.Dot is near 1 when the two vectors are similar
      //Therefore, when we look down, we get the Recording      
      OnContinuousRecording();
    }
    
  }

  public void OnFinalResult(string result)
  {
    //startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
   //Debug.Log("Final result");
    resultText.text = result;
    Debug.Log(this.gameObject.transform.parent.transform.parent.parent.gameObject.name);
    if (this.gameObject.transform.parent.transform.parent.parent.gameObject.name=="Commander"){
      //Even if it is not the active camera, all soldiers have a speech to text so we only act "as the commander"
      VoiceCommand(result);
    }
    
    
    
    //startRecordingButton.enabled = true;
  }

  public void OnPartialResult(string result)
  {
    resultText.text = result;
  }

  public void OnAvailabilityChange(bool available)  
  {
    //startRecordingButton.enabled = available;
    if (!available)
    {
      resultText.text = "Speech Recognition not available";
    }
    else
    {
      resultText.text = "Say something :-)";
    }
  }

  public void OnAuthorizationStatusFetched(AuthorizationStatus status)
  {
    switch (status)
    {
      case AuthorizationStatus.Authorized:
        //startRecordingButton.enabled = true;
        break;
      default:
        //startRecordingButton.enabled = false;
        resultText.text = "Cannot use Speech Recognition, authorization status is " + status;
        break;
    }
  }

  public void OnEndOfSpeech()
  {
    //startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
   //Debug.Log("END");
  }

  public void OnError(string error)
  {
    //startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
    //startRecordingButton.enabled = true;
   //Debug.Log("Speech Recognition Error: " + error);
  }

  public void OnStartRecordingPressed()
  {
    if (SpeechRecognizer.IsRecording())
    {
#if UNITY_IOS && !UNITY_EDITOR
			SpeechRecognizer.StopIfRecording();
			//startRecordingButton.GetComponentInChildren<Text>().text = "Stopping";
			//startRecordingButton.enabled = false;
#elif UNITY_ANDROID && !UNITY_EDITOR
			SpeechRecognizer.StopIfRecording();
			//startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
#endif
    }
    else
    {
      SpeechRecognizer.StartRecording(true);
      //startRecordingButton.GetComponentInChildren<Text>().text = "Stop Recording";
      resultText.text = "Say something :-)";
    }
  }
  


  public void OnContinuousRecording()
  {
    if (!SpeechRecognizer.IsRecording())
    {
     //Debug.Log("not recording");
      SpeechRecognizer.StartRecording(true);
      resultText.text = "Taking notes!";
    }
    
  }
  void VoiceCommand(string result){
   Debug.Log("Call");
    switch (result){
      case "camera Zero":
      case "cero":
      case "camera zero":
        voice_control.SwitchMainCamera();
        break;
      case "camera one":
        //Debug.Log("Si");
        voice_control.SwitchTimCamera();
        break;
      case "camera two":
        voice_control.SwitchBobCamera();
        break;
      case "camera three":
        voice_control.SwitchSamCamera();
        break;
      case "camera four":
        voice_control.SwitchUAVCamera();
        break;
      case "Tim":
      case "Tim come here":
      case "Timmy come here":
      case "Tim follow me":
      case "Timmy follow me":
      case "Tim stop":
      case "Timmy stop":
      case "Timmy":
        voice_control.MoveSoldier("Tim");
        Tim=!Tim;
        break;
      case "Bob":
      case "Bobby":
      case "Bobby come here":
        voice_control.MoveSoldier("Bob");
        Bob=!Bob;
        break;
      case "Sam":
      case "Sammy":
      case "Sam come here":
        voice_control.MoveSoldier("Sam");
        Sam=!Sam;
        break;
      case "walk":
      case "move":
        voice_control.Move();
        break;
      case "exit":
      case "Exit":
        SceneManager.LoadScene(sceneName: "Main");
        break;
      default:
        //Debug.Log("Command not in the options");
        break;
    }
  }

}
