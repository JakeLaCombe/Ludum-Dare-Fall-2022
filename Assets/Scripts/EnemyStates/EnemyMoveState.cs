using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class EnemyMove : IState
{
    private Enemy enemy;
    List<AStarNode> travelingPath;
    PathFinding levelPath;
    public Animator animator;
    public PatrolTypes initialPatrolType;
    public PatrolTypes currentPatrolType;
    GameObject level;
    private Vector3 originalPosition;
    private Vector3 patrolDestination;
    private Vector3 patrolDestinationStandingDirection;
    private Vector3 startingDirection;
    private Coroutine delayedMovement;

    private float speed = 1.0f;

    enum PathState
    {
        INITIAL,
        GENERATE_PATH,
        TRAVEL_PATH,
        FREEZE,
    }

    enum TargetDestination
    {
        PATROL_DESTINATION,
        ORIGINAL_LOCATION,
    }

    TargetDestination currentDestination = TargetDestination.PATROL_DESTINATION;

    private Vector2[] detectionPoints = {
        new Vector2(-1.0f, 0.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(0.0f, -1.0f)
    };

    public EnemyMove(Enemy enemy, PatrolTypes patrolType, Vector3 patrolDestination, Vector3 startingDirection, Vector3 patrolDestinationStandingDirection)
    {
        this.enemy = enemy;
        this.initialPatrolType = patrolType;
        this.currentPatrolType = this.initialPatrolType;
        this.patrolDestination = patrolDestination;
        this.originalPosition = enemy.transform.position;
        this.startingDirection = startingDirection;
        this.patrolDestinationStandingDirection = patrolDestinationStandingDirection;
    }
    public void Enter()
    {
        level = GameObject.Find("Level");
        levelPath = level.GetComponentInChildren<PathFinding>();
        animator = enemy.GetComponent<Animator>();
        travelingPath = new List<AStarNode>();
        currentDestination = TargetDestination.ORIGINAL_LOCATION;
        DetermineAnimation(Vector3.zero, startingDirection);
    }
    public void Execute()
    {
        // if (currentPatrolType != PatrolTypes.STANDING)
        // {
        TravelPath();
        //  }
    }

    private void GeneratePath(Vector3 destination)
    {
        Vector3 flooredDestination = new Vector3(Mathf.Floor(destination.x) + 0.5f, Mathf.Floor(destination.y) + 0.5f, destination.z);
        travelingPath = levelPath.FindPath(enemy.transform.position, flooredDestination);
    }

    private List<AStarNode> CalculatePath(Vector3 destination)
    {
        Vector3 flooredDestination = new Vector3(Mathf.Floor(destination.x) + 0.5f, Mathf.Floor(destination.y) + 0.5f, destination.z);
        return levelPath.FindPath(enemy.transform.position, flooredDestination);
    }

    public void NewDestination(Vector3 newPosition)
    {
        Vector3 flooredPosition = new Vector3(Mathf.Floor(newPosition.x) + 0.5f, Mathf.Floor(newPosition.y) + 0.5f, newPosition.z);
        List<AStarNode> possiblePath = levelPath.FindPath(enemy.transform.position, flooredPosition);

        if (possiblePath.Count < 20)
        {
            speed = 4.0f;
            travelingPath = levelPath.FindPath(enemy.transform.position, flooredPosition);
        }
    }

    private void TravelPath()
    {
        if (travelingPath.Count > 0)
        {
            Vector3 nextPosition = levelPath.GetWorldCoordinates(travelingPath[0]);
            nextPosition.z = enemy.transform.position.z;
            nextPosition.x += 0.5f;
            nextPosition.y += 0.5f;

            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, nextPosition, Time.deltaTime * 3.0f * speed);
            DetermineAnimation(enemy.transform.position, nextPosition);

            if (Mathf.Abs(enemy.transform.position.x - nextPosition.x) < float.Epsilon && Mathf.Abs(enemy.transform.position.y - nextPosition.y) < float.Epsilon)
            {
                travelingPath.RemoveAt(0);
            }

            animator.SetBool("isRunning", true);
        }
        else if (currentDestination == TargetDestination.ORIGINAL_LOCATION && initialPatrolType != PatrolTypes.STANDING)
        {
            speed = 1.0f;

            if (initialPatrolType == PatrolTypes.MOVE_AND_WAIT)
            {
                if (delayedMovement == null)
                {
                    delayedMovement = this.enemy.StartCoroutine(DelayedMove(patrolDestination, PatrolTypes.MOVING, TargetDestination.PATROL_DESTINATION));
                }
            }
            else
            {
                GeneratePath(patrolDestination);
                currentDestination = TargetDestination.PATROL_DESTINATION;
                animator.SetBool("isRunning", false);
                DetermineAnimation(Vector3.zero, startingDirection);
            }
        }
        else if (currentDestination == TargetDestination.PATROL_DESTINATION || (currentDestination == TargetDestination.ORIGINAL_LOCATION && initialPatrolType == PatrolTypes.STANDING))
        {
            speed = 1.0f;

            if (initialPatrolType == PatrolTypes.MOVE_AND_WAIT)
            {
                if (delayedMovement == null)
                {
                    delayedMovement = this.enemy.StartCoroutine(DelayedMove(originalPosition, PatrolTypes.MOVING, TargetDestination.ORIGINAL_LOCATION));
                }
            }
            else
            {
                GeneratePath(originalPosition);
                currentPatrolType = PatrolTypes.MOVING;
                currentDestination = TargetDestination.ORIGINAL_LOCATION;
                animator.SetBool("isRunning", false);
                DetermineAnimation(Vector3.zero, startingDirection);
            }
        }
        else
        {
            Freeze();
        }
    }

    private void DetermineAnimation(Vector3 current, Vector3 target)
    {
        if (current.x != target.x)
        {
            animator.SetBool("isFacingDown", false);
            animator.SetBool("isFacingUp", false);

            Vector3 currentScale = enemy.transform.localScale;

            if (current.x < target.x)
            {
                currentScale.x = Mathf.Abs(currentScale.x);
            }
            else
            {
                currentScale.x = -Mathf.Abs(currentScale.x);
            }

            enemy.transform.localScale = currentScale;
        }
        else if (current.y < target.y)
        {
            animator.SetBool("isFacingDown", false);
            animator.SetBool("isFacingUp", true);

            Vector3 currentScale = enemy.transform.localScale;
            currentScale.x = Mathf.Abs(currentScale.x);
            enemy.transform.localScale = currentScale;
        }
        else if (current.y > target.y)
        {
            animator.SetBool("isFacingDown", true);
            animator.SetBool("isFacingUp", false);

            Vector3 currentScale = enemy.transform.localScale;
            currentScale.x = Mathf.Abs(currentScale.x);
            enemy.transform.localScale = currentScale;
        }
    }

    public void processAction()
    {

    }

    public void CheckCollider()
    {

    }

    public void Exit()
    {

    }

    public void Freeze()
    {
        currentPatrolType = PatrolTypes.STANDING;
        animator.SetBool("isRunning", false);
        enemy.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        if (currentDestination == TargetDestination.PATROL_DESTINATION)
        {
            DetermineAnimation(Vector3.zero, patrolDestinationStandingDirection);
        }
        else
        {
            DetermineAnimation(Vector3.zero, startingDirection);
        }
    }

    private IEnumerator DelayedMove(Vector3 targetDestination, PatrolTypes patrolType, TargetDestination targetDestinationType)
    {
        animator.SetBool("isRunning", false);

        if (currentDestination == TargetDestination.PATROL_DESTINATION)
        {
            DetermineAnimation(Vector3.zero, patrolDestinationStandingDirection);
        }
        else
        {
            DetermineAnimation(Vector3.zero, startingDirection);
        }

        yield return new WaitForSeconds(4.0f);

        GeneratePath(targetDestination);
        currentPatrolType = patrolType;
        currentDestination = targetDestinationType;

        delayedMovement = null;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        
    }
}
