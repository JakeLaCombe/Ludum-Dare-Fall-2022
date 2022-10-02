using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public StateMachine stateMachine;
    public PlayerMoveState playerMoveState;
    public PlayerDeadState playerDeadState;

    public Rigidbody2D rigidBody;
    public Animator animator;
    public IInputable input;
    public SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        input = this.gameObject.GetComponent<IInputable>();

        stateMachine = new StateMachine();
        playerMoveState = new PlayerMoveState(this);
        stateMachine.ChangeState(playerMoveState);
        playerDeadState = new PlayerDeadState(this);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    public void SetTravelingPath(List<AStarNode> newPath)
    {
        playerMoveState.SetTravelingPath(newPath);
    }

    public void ResetMovement()
    {
        playerMoveState.StopPlayer();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collided");
        stateMachine.currentState.OnTriggerEnter2D(other);
    }

    public void KillPlayer()
    {
        stateMachine.ChangeState(playerDeadState);
    }

}
