using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public PatrolTypes patrolType;
    public StateMachine stateMachine;
    public Vector3 patrolDestination;
    public EnemyMove enemyMoveState;
    public EnemyPauseState enemyPauseState;
    public Vector3 startingDirection = new Vector3(1.0f, 1.0f, 1.0f);

    public Vector3 patrolDestinationStandingDirection = new Vector3(1.0f, 1.0f, 1.0f);

    // Start is called before the first frame update
    void Start()
    {
        stateMachine = new StateMachine();
        enemyMoveState = new EnemyMove(this, patrolType, patrolDestination, startingDirection, patrolDestinationStandingDirection);
        enemyPauseState = new EnemyPauseState(this);

        stateMachine.ChangeState(enemyMoveState);
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.instance != null)
        {
            UpdateState(LevelManager.instance);
            stateMachine.currentState.Execute();
        }
    }

    void UpdateState(LevelManager levelManager)
    {
        if (levelManager.currentState == LevelState.ACTIVE || levelManager.currentState == LevelState.OBSERVING)
        {
            stateMachine.ChangeState(enemyMoveState);
        }

        if (levelManager.currentState == LevelState.PLANNING)
        {
            stateMachine.ChangeState(enemyPauseState);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        stateMachine.currentState.OnTriggerEnter2D(other);
    }

    public void SetNewDestination(Vector3 newDestination)
    {
        if (stateMachine.currentState == enemyMoveState)
        {
            enemyMoveState.NewDestination(newDestination);
        }
    }
}


public enum PatrolTypes
{
    STANDING,
    MOVING,
    MOVE_AND_WAIT
}