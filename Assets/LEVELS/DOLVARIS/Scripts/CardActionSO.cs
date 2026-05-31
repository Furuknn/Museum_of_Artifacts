using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;



[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card")]
public class CardSO : ScriptableObject
{
    public CardType type;
    [SerializeReference] public List<CardComponent> components = new List<CardComponent>();
    public string value;
    public Color valueColor = new Color(1,1,1,1);
    public bool passiveCard = false;
    public Sprite cardSprite;

    [Header("Countdown Card Settings")]
    public Sprite countdownSprite;
    public bool countdownCard = false;
    public bool actionEveryCount = false;
    public int count = 3;

    [Header("Other")]
    public bool inDeck = true;
    public int inDeckAmount;

}

[Serializable]
public abstract class CardComponent
{
    public abstract void Use();

    
}

[Serializable]
public class card_damage: CardComponent
{
    public int damage;
    public bool trueDamage = false;
    public bool comboDamage = false;

    public override void Use()
    {
        CardGameManager.Instance.Damage(damage, trueDamage, comboDamage);
    }
}

[Serializable]
public class card_heal : CardComponent
{
    public int heal;
    public bool healDefence = false;
    public bool maxHeal = false;
    public override void Use()
    {
        CardGameManager.Instance.Heal(heal, healDefence, maxHeal);
    }
}

[Serializable]
public class card_addmove : CardComponent
{
    public int addMove;
    public override void Use()
    {
        CardGameManager.Instance.AddMove(addMove);
    }
}

[Serializable]
public class card_drawcard : CardComponent
{
    public int drawCard;

    public override void Use()
    {
        CardGameManager.Instance.DrawCardEvent(drawCard);
    }
}

[Serializable]
public class card_destroycountdown : CardComponent
{
    public string destroyCountdown;
    public override void Use()
    {
        CardGameManager.Instance.DestroyCountdown();
    }
}

[Serializable]
public class card_ignoreattack : CardComponent
{
    public string ignoreAttack;
    public override void Use()
    {
        
    }
}

[Serializable]
public class card_drawfate : CardComponent
{
    public string drawfate;
    public override void Use()
    {
        CardGameManager.Instance.PickFate();
    }
}

[Serializable]
public class fate_handcount: CardComponent
{
    public int handCount;
    public override void Use()
    {
        CardGameManager.Instance.AdjustHandCount(handCount);
    }
}

[Serializable]
public class fate_nodefence : CardComponent
{
    public string noDefence;
    public override void Use()
    {
        CardGameManager.Instance.noDefence = true;
    }
}

[Serializable]
public class fate_noheal : CardComponent
{
    public string noHeal;
    public override void Use()
    {
        CardGameManager.Instance.noHeal = true;
    }
}

[Serializable]
public class fate_damagemulti : CardComponent
{
    public float damageMultiplier;
    public override void Use()
    {
        CardGameManager.Instance.fateDamageMulti = damageMultiplier;
    }
}

[Serializable]
public class fate_maxhealth : CardComponent
{
    public string maxHealthorDefence;
    public int maxValue;
    public bool isDefence = false;

    public override void Use()
    {
        if (isDefence)
        {
            CardGameManager.Instance.PLAYER_MAXDEFENCE = maxValue;
            if (CardGameManager.Instance.playerDefence > CardGameManager.Instance.PLAYER_MAXDEFENCE) CardGameManager.Instance.playerDefence = CardGameManager.Instance.PLAYER_MAXDEFENCE;
            CardGameManager.Instance.UpdateUIs();
            CardGameManager.Instance.PlayerDefenceHealFeedback();
        }
        else
        {
            CardGameManager.Instance.PLAYER_MAXHEALTH = maxValue;
            if (CardGameManager.Instance.playerHealth > CardGameManager.Instance.PLAYER_MAXHEALTH) CardGameManager.Instance.playerHealth = CardGameManager.Instance.PLAYER_MAXHEALTH;
            CardGameManager.Instance.UpdateUIs();
            CardGameManager.Instance.PlayerHealthHealFeedback();
        }
        
    }
}

[Serializable]
public class fate_drawandheal : CardComponent
{
    public string drawAndHeal;
    public int healValue;
    public bool isDefence = false;

    public override void Use()
    {
        CardGameManager.Instance.drawAndHeal = healValue;
    }
}

[Serializable]
public class fate_vampire : CardComponent
{
    public string vampire;
    public float lifeStealPercentage;

    public override void Use()
    {
        CardGameManager.Instance.lifeStealPercentage = lifeStealPercentage;
    }
}

[Serializable]
public class fate_thorns : CardComponent
{
    public string thorns;
    public float thornsDamagePercentage;

    public override void Use()
    {
        CardGameManager.Instance.thornsDamagePercentage = thornsDamagePercentage;
    }
}

[Serializable]
public class fate_healbonus : CardComponent
{
    public string healBonus;
    public float healBonusPercentage;

    public override void Use()
    {
        CardGameManager.Instance.healBonusPercentage = healBonusPercentage;
    }
}

[Serializable]
public class fate_defencebonus : CardComponent
{
    public string defenceBonus;
    public float defenceBonusPercentage;

    public override void Use()
    {
        CardGameManager.Instance.defenceBonusPercentage = defenceBonusPercentage;
    }
}