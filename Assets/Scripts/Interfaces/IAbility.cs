using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IAbility
{
    void Activate();
    float Cooldown { get; }
}
