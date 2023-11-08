using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using KKSpeech;

public class RecordingCanvas : MonoBehaviour
{
  //public Button startRecordingButton;
  public Text resultText;
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
    resultText.text = result;
    
    VoiceCommand(result);
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
  }

  public void OnError(string error)
  {
    //startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
    //startRecordingButton.enabled = true;
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
      //Debug.Log("Not recording");
      SpeechRecognizer.StartRecording(true);
      resultText.text = "Taking notes!";
    }
    
  }
  void VoiceCommand(string result){
    switch (result){
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
      case "Timmy move":
        voice_control.MoveSoldier("Tim");
        break;
      case "":
        voice_control.SwitchUAVCamera();
        break;
      default:
        //Debug.Log("Command not in the options");
        break;
    }
  }

}
