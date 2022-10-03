using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public LevelState currentState;
    public float secondsRemaining = 10.0f;
    public int currentLevel = 0;
    private string[] LEVELS = { "LevelOne", "LevelTwo" };

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        for (int i = 0; i < LEVELS.Length; i++)
        {
            if (SceneManager.GetActiveScene().name == LEVELS[i])
            {
                currentLevel = i;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == LevelState.INITIAL)
        {
            currentState = LevelState.OBSERVING;
        }

        secondsRemaining -= Time.deltaTime;

        if (secondsRemaining <= 0)
        {
            secondsRemaining = 10.0f;
            SwitchPhase();
        }
    }

    private void SwitchPhase()
    {
        if (currentState == LevelState.INITIAL || currentState == LevelState.OBSERVING)
        {
            currentState = LevelState.PLANNING;
        }
        else if (currentState == LevelState.PLANNING)
        {
            currentState = LevelState.ACTIVE;
        }
        else if (currentState == LevelState.ACTIVE)
        {
            currentState = LevelState.OBSERVING;
        }
    }

    public void RestartLevel()
    {
        StartCoroutine(LevelCoroutine(SceneManager.GetActiveScene().name));
    }

    public void NextLevel()
    {
        currentLevel += 1;

        if (currentLevel == LEVELS.Length)
        {
            StartCoroutine(LevelCoroutine("Credits"));
        }
        else
        {
            StartCoroutine(LevelCoroutine(LEVELS[currentLevel]));
        }
    }

    private IEnumerator LevelCoroutine(string sceneName)
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(sceneName);
    }

    public void FastforwardTime()
    {
        secondsRemaining = 0.0f;
    }
}

public enum LevelState
{
    INITIAL,
    PLANNING,
    ACTIVE,
    OBSERVING,
    LEVEL_COMPLETE,
    GAME_COMPLETE,
}