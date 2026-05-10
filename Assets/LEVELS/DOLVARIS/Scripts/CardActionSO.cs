using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;



[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardSO : ScriptableObject
{
    public CardType type;
    [SerializeReference] public List<CardComponent> components = new List<CardComponent>();
    public int value;
    public bool passiveCard = false;
    public Sprite cardSprite;

    [Header("Countdown Card Settings")]
    public bool countdownCard = false;
    public bool actionEveryCount = false;
    public int count = 3;

    [Header("Other")]
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

    public override void Use()
    {
        CardGameManager.Instance.Damage(damage, trueDamage);
    }
}

[Serializable]
public class card_heal : CardComponent
{
    public int value;
    public bool healDefence = false;
    public bool maxHeal = false;
    public override void Use()
    {
        CardGameManager.Instance.Heal(value, healDefence, maxHeal);
    }
}

[Serializable]
public class card_addmove : CardComponent
{
    public int value;
    public override void Use()
    {
        CardGameManager.Instance.AddMove(value);
    }
}

[Serializable]
public class card_drawcard : CardComponent
{
    public int count;

    public override void Use()
    {
        CardGameManager.Instance.DrawCard(count);
    }
}

[Serializable]
public class card_destroycountdown : CardComponent
{
    public override void Use()
    {
        CardGameManager.Instance.DestroyCountdown();
    }
}

[Serializable]
public class card_ignoreattack : CardComponent
{
    public override void Use()
    {
        
    }
}


