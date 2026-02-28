using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealerScript : MonoBehaviour
{
    public void StartDealDamage()
    {
        if (GetComponentInChildren<NightStickScript>() != null)
        {
            GetComponentInChildren<NightStickScript>().StartDealDamage();
        }
        else
        {
            return;
        }
    }
    public void EndDealDamage()
    {
        if (GetComponentInChildren<NightStickScript>() != null)
        {
            GetComponentInChildren<NightStickScript>().EndDealDamage();
        }
        else
        {
            return;
        }
    }
}
