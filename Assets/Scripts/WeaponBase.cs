using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    public abstract void MainAttack();
    public abstract void SecondaryAttack();
    public abstract void UltimateAttack();
}
