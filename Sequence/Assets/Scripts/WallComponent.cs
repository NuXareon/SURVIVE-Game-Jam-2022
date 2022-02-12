using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WallComponent : MonoBehaviour
{
    public Utils.GameColor color = Utils.GameColor.White;
    void Start()
    {
        GameObject gameLogic = GameObject.FindGameObjectWithTag("GameController");
        if (gameLogic)
        {
            Utils utils = gameLogic.GetComponent<Utils>();

            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            Renderer renderer = GetComponent<Renderer>();
            renderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", utils.GetColor(color));
            renderer.SetPropertyBlock(propBlock);
        }

        if (color != Utils.GameColor.White)
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }
    }

    private void OnValidate()
    {
        GameObject gameLogic = GameObject.FindGameObjectWithTag("GameController");
        if (gameLogic)
        {
            Utils utils = gameLogic.GetComponent<Utils>();
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            Renderer renderer = GetComponent<Renderer>();
            renderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", utils.GetColor(color));
            renderer.SetPropertyBlock(propBlock);
        }
    }

    void OnTriggerStay(Collider other)
    {
        ProcessPlayerColliding(other);
    }

    void OnTriggerEnter(Collider other)
    {
        ProcessPlayerColliding(other);
    }

    void ProcessPlayerColliding(Collider other)
    {
        PlayerComponent player = other.gameObject.GetComponent<PlayerComponent>();
        if (player)
        {
            if (player.color != color)
            {
                // TODO manage death properly
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
            }
        }
    }
}
