using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PlayerMoveState : IState
{
    private Player player;
    List<AStarNode> travelingPath;
    PathFinding levelPath;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    GameObject level;
    private Vector3 originalPosition;
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

    public PlayerMoveState(Player player)
    {
        this.player = player;
        this.originalPosition = player.transform.position;

        level = GameObject.Find("Level");
        travelingPath = new List<AStarNode>();
        levelPath = level.GetComponentInChildren<PathFinding>();
        animator = player.GetComponent<Animator>();
        spriteRenderer = player.GetComponent<SpriteRenderer>();
    }
    public void Enter()
    {
        level = GameObject.Find("Level");
        levelPath = level.GetComponentInChildren<PathFinding>();
        animator = player.GetComponent<Animator>();
        travelingPath = new List<AStarNode>();
        currentDestination = TargetDestination.ORIGINAL_LOCATION;
    }
    public void Execute()
    {
        // if (currentPatrolType != PatrolTypes.STANDING)
        // {
        AddPlayerDecorations();
        TravelPath();
        //  }
    }

    private void AddPlayerDecorations()
    {
        bool hasForceField = player.pickups.Contains(PickupTypes.FORCE_FIELD);

        if (hasForceField)
        {
            spriteRenderer.color = new Color(0.0f, 1.0f, 0.0f);
        }
        else
        {
            spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f);
        }
    }

    private void GeneratePath(Vector3 destination)
    {
        Vector3 flooredDestination = new Vector3(Mathf.Floor(destination.x) + 0.5f, Mathf.Floor(destination.y) + 0.5f, destination.z);
        travelingPath = levelPath.FindPath(player.transform.position, flooredDestination);
    }

    private List<AStarNode> CalculatePath(Vector3 destination)
    {
        Vector3 flooredDestination = new Vector3(Mathf.Floor(destination.x) + 0.5f, Mathf.Floor(destination.y) + 0.5f, destination.z);
        return levelPath.FindPath(player.transform.position, flooredDestination);
    }

    public void NewDestination(Vector3 newPosition)
    {
        Vector3 flooredPosition = new Vector3(Mathf.Floor(newPosition.x) + 0.5f, Mathf.Floor(newPosition.y) + 0.5f, newPosition.z);
        List<AStarNode> possiblePath = levelPath.FindPath(player.transform.position, flooredPosition);

        if (possiblePath.Count < 20)
        {
            speed = 4.0f;
            travelingPath = levelPath.FindPath(player.transform.position, flooredPosition);
        }
    }

    private void TravelPath()
    {
        if (travelingPath.Count > 0)
        {
            Vector3 nextPosition = levelPath.GetWorldCoordinates(travelingPath[0]);
            nextPosition.z = player.transform.position.z;
            nextPosition.x += 0.5f;
            nextPosition.y += 0.5f;

            player.transform.position = Vector3.MoveTowards(player.transform.position, nextPosition, Time.deltaTime * 3.0f * speed);

            if (Mathf.Abs(player.transform.position.x - nextPosition.x) < float.Epsilon && Mathf.Abs(player.transform.position.y - nextPosition.y) < float.Epsilon)
            {
                travelingPath.RemoveAt(0);
            }

            DetermineAnimation(player.transform.position, nextPosition);
            animator.SetBool("isRunning", true);
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

            Vector3 currentScale = player.transform.localScale;

            if (current.x < target.x)
            {
                currentScale.x = Mathf.Abs(currentScale.x);
            }
            else
            {
                currentScale.x = -Mathf.Abs(currentScale.x);
            }

            player.transform.localScale = currentScale;
        }
        else if (current.y < target.y)
        {
            animator.SetBool("isFacingDown", false);
            animator.SetBool("isFacingUp", true);

            Vector3 currentScale = player.transform.localScale;
            currentScale.x = Mathf.Abs(currentScale.x);
            player.transform.localScale = currentScale;
        }
        else if (current.y > target.y)
        {
            animator.SetBool("isFacingDown", true);
            animator.SetBool("isFacingUp", false);

            Vector3 currentScale = player.transform.localScale;
            currentScale.x = Mathf.Abs(currentScale.x);
            player.transform.localScale = currentScale;
        }
    }

    public void SetTravelingPath(List<AStarNode> newPath)
    {
        if (newPath.Count > 0)
        {
            Vector3 nextPosition = levelPath.GetWorldCoordinates(newPath[0]);
            nextPosition.z = player.transform.position.z;
            player.transform.position = nextPosition;
        }

        travelingPath = newPath;
    }

    public void StopPlayer()
    {
        travelingPath.Clear();
        Freeze();
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
        animator.SetBool("isRunning", false);
        player.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    private IEnumerator DelayedMove(Vector3 targetDestination, TargetDestination targetDestinationType)
    {
        animator.SetBool("isRunning", false);
        yield return new WaitForSeconds(4.0f);

        GeneratePath(targetDestination);
        currentDestination = targetDestinationType;

        delayedMovement = null;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Enemy")
        {
            processEnemyHit(collider.gameObject);
        }
    }

    private void processEnemyHit(GameObject enemy)
    {
        if (player.pickups.Contains(PickupTypes.FORCE_FIELD))
        {
            player.pickups.Remove(PickupTypes.FORCE_FIELD);
            GameObject.Destroy(enemy);
        }
        else
        {
            player.KillPlayer();
        }
    }
}
