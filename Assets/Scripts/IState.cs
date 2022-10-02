using UnityEngine;
public interface IState
{
    public void Enter();
    public void Execute();
    public void Exit();
    public void OnTriggerEnter2D(Collider2D other);
}