using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    public enum GameState
    {
        GameStart,
        GamePlaying,
        PlayerDead,
        LevelEnd
    }

    public GameState gameState = GameState.GameStart;

    public bool IsGamePaused()
    {
        if (gameState == GameState.GameStart)
        {
            return true;
        }

        if (gameState == GameState.LevelEnd)
        {
            return true;
        }

        if (gameState == GameState.PlayerDead)
        {
            return true;
        }

        return false;
    }

    void Start()
    {
        Time.timeScale = 0;
    }

    void Update()
    {
        if (gameState == GameState.GameStart)
        {
            if (Input.GetButtonDown("Jump")
             || Input.GetAxisRaw("Horizontal") != 0.0f
             || Input.GetAxisRaw("Vertical") != 0.0f)
            {
                Time.timeScale = 1;
                gameState = GameState.GamePlaying;
            }
        }
    }

}
