using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    float minFov = 15f;
    float maxFov = 90f;
    float sensitivity = 20f;

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.backgroundColor = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        float fov = CameraButton.currentCamera.fieldOfView;
        fov -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        CameraButton.currentCamera.fieldOfView = fov;
    }
}
