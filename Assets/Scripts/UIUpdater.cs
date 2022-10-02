using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIUpdater : MonoBehaviour
{

    private TextMeshProUGUI Status;

    private TextMeshProUGUI Timer;

    // Start is called before the first frame update
    void Start()
    {
        Status = transform.Find("Level Status").GetComponent<TextMeshProUGUI>();
        Timer = transform.Find("Timer").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.instance != null)
        {
            UpdateGameplay(LevelManager.instance);
        }
    }

    void UpdateGameplay(LevelManager levelManager)
    {
        if (levelManager.currentState == LevelState.PLANNING)
        {
            Status.text = "Plan out your move";
        }
        else if (levelManager.currentState == LevelState.ACTIVE)
        {
            Status.text = "Execution Phase";
        }
        else if (levelManager.currentState == LevelState.OBSERVING)
        {
            Status.text = "Observation Time";
        }

        Timer.text = Mathf.CeilToInt(levelManager.secondsRemaining).ToString();
    }
}
