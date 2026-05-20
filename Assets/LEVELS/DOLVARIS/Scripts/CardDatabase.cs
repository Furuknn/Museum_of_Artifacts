using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "Cards/Database")]
public class CardDatabase : ScriptableObject
{
<<<<<<< Updated upstream
    public List<CardSO> allCards = new List<CardSO>();
=======
    public List<CardSO> actionCards = new List<CardSO>();
    public List<CardSO> fateCards = new List<CardSO>();
>>>>>>> Stashed changes
}