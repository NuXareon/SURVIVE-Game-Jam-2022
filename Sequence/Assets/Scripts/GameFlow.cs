using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlow : MonoBehaviour
{
    public int lastLevelPlayed = 0;

    public enum GameState
    {
        GameStart,
        GamePlaying,
        PlayerDead,
        LevelEnd
    }

    [HideInInspector]
    public GameState gameState = GameState.GameStart;
    public AudioSource levelCompletedAudio1;
    public AudioSource levelCompletedAudio2;

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

    public void OnLevelCompleted()
    {
        gameState = GameState.LevelEnd;
        Time.timeScale = 0;
        StartCoroutine(FinishLevel());
    }

    public void OnPlayerDeath()
    {
        gameState = GameState.PlayerDead;
        Time.timeScale = 0;
        StartCoroutine(KillPlayer());
    }

    public void OnContinueGame()
    {
        gameState = GameState.LevelEnd;
        Time.timeScale = 0;
        lastLevelPlayed = SaveGame.LoadLastLevelPlayed();
        StartCoroutine(LoadLevel(lastLevelPlayed));
    }

    public void OnExitGame()
    {
        gameState = GameState.LevelEnd;
        Time.timeScale = 0;
        StartCoroutine(ExitGame());
    }

    public void OnCompleteGame()
    {
        gameState = GameState.LevelEnd;
        Time.timeScale = 0;
        StartCoroutine(GoToMainMenu(true));
    }

    IEnumerator FinishLevel()
    {
        int audioClip = Random.Range(0, 2);
        if (audioClip == 0)
        {
            levelCompletedAudio1.Play();
        }
        else
        {
            levelCompletedAudio2.Play();
        }

        Scene scene = SceneManager.GetActiveScene();
        int nextSceneIndex = scene.buildIndex + 1;
        SaveGame.SaveLevelProgress(nextSceneIndex);

        yield return new WaitForSecondsRealtime(1.2f);

        SceneManager.LoadScene(nextSceneIndex);
    }

    IEnumerator KillPlayer()
    {
        // TODO play death audio

        yield return new WaitForSecondsRealtime(0.3f);

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        int audioClip = Random.Range(0, 2);
        if (audioClip == 0)
        {
            levelCompletedAudio1.Play();
        }
        else
        {
            levelCompletedAudio2.Play();
        }

        yield return new WaitForSecondsRealtime(1.2f);

        if (SceneManager.sceneCountInBuildSettings > levelIndex)
        {
            SceneManager.LoadScene(levelIndex);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    IEnumerator ExitGame()
    {
        int audioClip = Random.Range(0, 2);
        if (audioClip == 0)
        {
            levelCompletedAudio1.Play();
        }
        else
        {
            levelCompletedAudio2.Play();
        }

        yield return new WaitForSecondsRealtime(1.2f);

        Application.Quit();
    }

    IEnumerator GoToMainMenu(bool gameCompleted)
    {
        if (gameCompleted)
        { 
            levelCompletedAudio1.Play();
            levelCompletedAudio2.Play();

            yield return new WaitForSecondsRealtime(3.0f);
        }

        SceneManager.LoadScene(0);
    }
}
