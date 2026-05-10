using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "Cards/Database")]
public class CardDatabase : ScriptableObject
{
    public List<CardSO> allCards = new List<CardSO>();
}