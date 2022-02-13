using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuTrigger : MonoBehaviour
{
    public MainMenuCamera menuCamera;
    void OnTriggerEnter(Collider other)
    {
        PlayerComponent player = other.gameObject.GetComponent<PlayerComponent>();
        if (player)
        {
            menuCamera.EnterMainMenuArea();
        }
    }
}
