using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraButton : MonoBehaviour
{
    Camera sCamera;
    Camera tCamera;
    public static Camera currentCamera;
    TMP_Text buttonText; 
    const string frontCameraText = "Front Camera";
    const string topCameraText = "Top Camera";

    public void Start() { 
        sCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        tCamera = GameObject.Find("Top Down Camera").GetComponent<Camera>();

        buttonText = GetComponentInChildren<TMP_Text>();
        buttonText.text = frontCameraText;

        currentCamera = sCamera; 
        tCamera.enabled = false;
    }

    public void Clicked(){
        if (currentCamera == sCamera) { 
            sCamera.enabled = false; 
            currentCamera = tCamera;
            buttonText.text = frontCameraText;
        } else if (currentCamera == tCamera) { 
            tCamera.enabled = false; 
            currentCamera = sCamera;
            buttonText.text = topCameraText;
        } else { 
            Debug.Log("Camera not found");
        }

        currentCamera.enabled = true; 
    }
}
