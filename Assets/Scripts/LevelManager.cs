using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public LevelState currentState;
    public Coroutine swapState;

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

        if (swapState == null)
        {
            swapState = StartCoroutine(SwitchPhase());
        }
    }

    private IEnumerator SwitchPhase()
    {
        yield return new WaitForSeconds(5.0f);

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

        swapState = null;
    }
}

public enum LevelState
{
    INITIAL,
    PLANNING,
    ACTIVE,
    OBSERVING,
}