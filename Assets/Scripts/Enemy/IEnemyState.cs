using UnityEngine;

public interface IEnemyState
{
    void Enter(EnemyController enemy);
    void Tick(EnemyController enemy);
    void FixedTick(EnemyController enemy);
    void Exit(EnemyController enemy);
}
