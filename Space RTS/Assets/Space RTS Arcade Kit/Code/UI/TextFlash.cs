using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Allows to display information text which fades in and out.
/// </summary>
public class TextFlash : MonoBehaviour {

    public AnimationCurve OpacityCurve;
    public float AnimationTime = 5;

    private Text _flashingText;
    private float _elapsedTime = 0;
    private bool _started = false;
    
	void Awake () {
        _flashingText = GetComponent<Text>();
        _flashingText.enabled = false;
        _flashingText.color = new Color(1, 1, 1, 0);
    }

    public void SetText(string text)
    {
        _flashingText.enabled = true;
        _flashingText.text = text;
        _started = true;
    }
	
	void Update () {
        if (!_started)
            return;

        _elapsedTime += Time.deltaTime;
        _flashingText.color = new Color(1, 1, 1, OpacityCurve.Evaluate(_elapsedTime / AnimationTime));
        if (_elapsedTime > AnimationTime) {
            TextFlash.OnMessageEnd();

            GameObject.Destroy(this.gameObject);
        }
    }

    private static List<GameObject> ActiveInstances;
    private static List<string> ActiveMessages;

    public static void ShowInfoText(string message)
    {
        if (ActiveInstances == null) { 
            ActiveInstances = new List<GameObject>();
            ActiveMessages = new List<string>();
        }

        var newInstance = GameObject.Instantiate(
            UIElements.Instance.InformationText,
            CanvasController.Instance.gameObject.transform);
        // Enqueue message
        ActiveInstances.Add(newInstance);
        ActiveMessages.Add(message);

        if(ActiveInstances.Count == 1)  // Play message if none are active right now
            newInstance.GetComponent<TextFlash>().SetText(message);
    }

    private static void OnMessageEnd()
    {
        // Remove currently ended message 
        ActiveInstances.RemoveAt(0);
        ActiveMessages.RemoveAt(0);

        // If there is a message in the wait queue, show it now
        if(ActiveInstances.Count > 0)
            ActiveInstances[0].GetComponent<TextFlash>().SetText(ActiveMessages[0]);
    }
}
