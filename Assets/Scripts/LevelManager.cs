using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public LevelState currentState;
    public float secondsRemaining = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
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
}

public enum LevelState
{
    INITIAL,
    PLANNING,
    ACTIVE,
    OBSERVING,
}