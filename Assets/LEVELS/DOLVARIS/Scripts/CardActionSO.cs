using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;



<<<<<<< Updated upstream
[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
=======
[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card")]
>>>>>>> Stashed changes
public class CardSO : ScriptableObject
{
    public CardType type;
    [SerializeReference] public List<CardComponent> components = new List<CardComponent>();
<<<<<<< Updated upstream
    public int value;
=======
    public string value;
    public Color valueColor = new Color(1,1,1,1);
>>>>>>> Stashed changes
    public bool passiveCard = false;
    public Sprite cardSprite;

    [Header("Countdown Card Settings")]
<<<<<<< Updated upstream
=======
    public Sprite countdownSprite;
>>>>>>> Stashed changes
    public bool countdownCard = false;
    public bool actionEveryCount = false;
    public int count = 3;

    [Header("Other")]
<<<<<<< Updated upstream
=======
    public bool inDeck = true;
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream

    public override void Use()
    {
        CardGameManager.Instance.Damage(damage, trueDamage);
=======
    public bool comboDamage = false;

    public override void Use()
    {
        CardGameManager.Instance.Damage(damage, trueDamage, comboDamage);
>>>>>>> Stashed changes
    }
}

[Serializable]
public class card_heal : CardComponent
{
<<<<<<< Updated upstream
    public int value;
=======
    public int heal;
>>>>>>> Stashed changes
    public bool healDefence = false;
    public bool maxHeal = false;
    public override void Use()
    {
<<<<<<< Updated upstream
        CardGameManager.Instance.Heal(value, healDefence, maxHeal);
=======
        CardGameManager.Instance.Heal(heal, healDefence, maxHeal);
>>>>>>> Stashed changes
    }
}

[Serializable]
public class card_addmove : CardComponent
{
<<<<<<< Updated upstream
    public int value;
    public override void Use()
    {
        CardGameManager.Instance.AddMove(value);
=======
    public int addMove;
    public override void Use()
    {
        CardGameManager.Instance.AddMove(addMove);
>>>>>>> Stashed changes
    }
}

[Serializable]
public class card_drawcard : CardComponent
{
<<<<<<< Updated upstream
    public int count;

    public override void Use()
    {
        CardGameManager.Instance.DrawCard(count);
=======
    public int drawCard;

    public override void Use()
    {
        CardGameManager.Instance.DrawCardEvent(drawCard);
>>>>>>> Stashed changes
    }
}

[Serializable]
public class card_destroycountdown : CardComponent
{
<<<<<<< Updated upstream
=======
    public string destroyCountdown;
>>>>>>> Stashed changes
    public override void Use()
    {
        CardGameManager.Instance.DestroyCountdown();
    }
}

[Serializable]
public class card_ignoreattack : CardComponent
{
<<<<<<< Updated upstream
=======
    public string ignoreAttack;
>>>>>>> Stashed changes
    public override void Use()
    {
        
    }
}

<<<<<<< Updated upstream

=======
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
>>>>>>> Stashed changes
