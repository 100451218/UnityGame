using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using KKSpeech;

public class RecordingCanvas : MonoBehaviour
{
  //public Button startRecordingButton;
  public Text resultText;
  private float waiter=0f;
  void Start()
  {
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
      Debug.Log("Error device has no speech recognizer");
      resultText.text = "Sorry, but this device doesn't support speech recognition";
      //startRecordingButton.enabled = false;
    }


  }

  void Update(){
    //OnStartRecordingPressed();
    OnContinuousRecording();
  }

  public void OnFinalResult(string result)
  {
    //startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
    resultText.text = result;
    Debug.Log(result);
    //startRecordingButton.enabled = true;
  }

  public void OnPartialResult(string result)
  {
    resultText.text = result;
    Debug.Log(result);
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
    Debug.LogError(error);
    //startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
    //startRecordingButton.enabled = true;
  }

  public void OnStartRecordingPressed()
  {
    if (SpeechRecognizer.IsRecording())
    {
      Debug.Log("Recording");
#if UNITY_IOS && !UNITY_EDITOR
			SpeechRecognizer.StopIfRecording();
			//startRecordingButton.GetComponentInChildren<Text>().text = "Stopping";
			//startRecordingButton.enabled = false;
#elif UNITY_ANDROID && !UNITY_EDITOR
      Debug.Log("It is an android and is stoping because we pressed");
			SpeechRecognizer.StopIfRecording();
			//startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
#endif
    }
    else
    {
      Debug.Log("Else case");
      SpeechRecognizer.StartRecording(true);
      //startRecordingButton.GetComponentInChildren<Text>().text = "Stop Recording";
      resultText.text = "Say something :-)";
    }
  }
  


  public void OnContinuousRecording()
  {
    if (!SpeechRecognizer.IsRecording())
    {
      Debug.Log("Not recording");
      SpeechRecognizer.StartRecording(true);
      resultText.text = "Taking notes!";
    }
    
  }


}
