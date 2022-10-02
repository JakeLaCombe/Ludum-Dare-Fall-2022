using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
public class EnemyPauseState : IState
{
    private Animator animator;
    private Enemy enemy;

    public EnemyPauseState(Enemy enemy)
    {
        this.animator = enemy.GetComponent<Animator>();
        this.enemy = enemy;
    }

    public void Enter()
    {
        this.animator.SetBool("isRunning", false);
    }

    public void Execute()
    {

    }

    public void Exit()
    {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {

    }
}