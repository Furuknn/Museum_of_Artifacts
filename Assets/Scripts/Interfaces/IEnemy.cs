using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy : IDamageable
{
    void ApplyStun(float duration);
    void Death();
    float GetHealthPercent();
}
