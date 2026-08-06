using UnityEngine;

public class EnemyPatrolState : IEnemyState
{
    private Transform _target;

    public void Enter(EnemyController enemy)
    {
        _target = enemy.PointB.position.x > enemy.transform.position.x ? enemy.PointB : enemy.PointA;
    }

    public void Exit(EnemyController enemy)
    {
        enemy.SetHorizontalVelocity(0f);
    }

    public void FixedTick(EnemyController enemy)
    {
        float direction = Mathf.Sign(_target.position.x - enemy.transform.position.x);
        enemy.SetHorizontalVelocity(direction * enemy.MoveSpeed);
        enemy.SetFacing(direction > 0);

        if (Mathf.Abs(enemy.transform.position.x - _target.position.x) < 0.1f)
        {
            _target = _target == enemy.PointA ? enemy.PointB: enemy.PointA;
        }
    }

    public void Tick(EnemyController enemy){ }
}
