using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
public class PlayerDeadState : IState
{
    private SpriteRenderer spriteRenderer;

    private Player player;

    public PlayerDeadState(Player player)
    {
        spriteRenderer = player.GetComponent<SpriteRenderer>();
        this.player = player;
    }

    public void Enter()
    {
        spriteRenderer.color = new Color(1, 0, 0);
        player.StartCoroutine(KillPlayer());
    }

    public void Execute()
    {

    }

    public IEnumerator KillPlayer()
    {
        Debug.Log("Killed");
        yield return new WaitForSeconds(3.0f);
        GameObject.Destroy(player.gameObject);
    }

    public void Exit()
    {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {

    }
}