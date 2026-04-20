using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyState
{
    void Enter(EnemyBase enemy);
    void Tick(EnemyBase enemy);       // Called every Update
    void FixedTick(EnemyBase enemy);  // Called every FixedUpdate
    void Exit(EnemyBase enemy);
}