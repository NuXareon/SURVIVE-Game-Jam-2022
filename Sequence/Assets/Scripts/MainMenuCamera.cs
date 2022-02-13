using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    public GameObject player;
    public GameObject targetLevelCameraObject;

    bool isInMainArea = false;
    Camera mainCamera;
    Camera targetLevelCamera;
    Vector3 velocity;
    float zoomVelocity;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        targetLevelCamera = targetLevelCameraObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInMainArea)
        {
            transform.position = player.transform.position;
            transform.position += Vector3.forward * -10.0f;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetLevelCameraObject.transform.position, ref velocity, 0.25f);
            mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, targetLevelCamera.orthographicSize, ref zoomVelocity, 0.25f);
        }
    }

    public void EnterMainMenuArea()
    {
        isInMainArea = true;
        velocity = player.GetComponent<Rigidbody>().velocity;
    }
}
