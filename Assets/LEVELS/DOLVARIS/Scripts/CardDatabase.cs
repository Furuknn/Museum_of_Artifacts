using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "Cards/Database")]
public class CardDatabase : ScriptableObject
{
    public List<CardSO> actionCards = new List<CardSO>();
    public List<CardSO> fateCards = new List<CardSO>();
}