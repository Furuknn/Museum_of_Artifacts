using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(CardDatabase))]
public class CardDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Standart Inspector görünümünü çiz
        base.OnInspectorGUI();

        CardDatabase database = (CardDatabase)target;

        EditorGUILayout.Space(10);

        // Þýk bir buton oluþtur
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("Update Card Database", GUILayout.Height(40)))
        {
            RefreshCards(database);
        }
        GUI.backgroundColor = Color.white;
    }

    private void RefreshCards(CardDatabase db)
    {
        // 1. Projedeki tüm "Card" tipindeki assetlerin GUID'lerini bul
        // "t:Card" ifadesi, Card sýnýfýndan türeyen tüm assetleri filtreler
        string[] guids = AssetDatabase.FindAssets("t:CardSO");

<<<<<<< Updated upstream
        List<CardSO> foundCards = new List<CardSO>();
=======
        List<CardSO> foundActionCards = new List<CardSO>();
        List<CardSO> foundFateCards = new List<CardSO>();
>>>>>>> Stashed changes

        foreach (string guid in guids)
        {
            // GUID'yi dosya yoluna çevir
            string path = AssetDatabase.GUIDToAssetPath(guid);
            // Dosyayý yükle ve listeye ekle
            CardSO card = AssetDatabase.LoadAssetAtPath<CardSO>(path);

<<<<<<< Updated upstream
            if (card != null)
            {
                foundCards.Add(card);
=======
            if (card != null && card.inDeck)
            {
                if (card.type == CardType.Action) foundActionCards.Add(card);
                else if (card.type == CardType.Fate) foundFateCards.Add(card);
>>>>>>> Stashed changes
            }
        }

        // 2. Deðiþikliði kaydetmek için Undo kaydý oluþtur (Geri alýnabilir yapar)
        Undo.RecordObject(db, "Refresh Card Database");

        // 3. Listeyi güncelle
<<<<<<< Updated upstream
        db.allCards = foundCards;
=======
        db.actionCards = foundActionCards;
        db.fateCards = foundFateCards;
>>>>>>> Stashed changes

        // 4. Dosyayý kirli (Dirty) olarak iþaretle ki Unity kaydedilmesi gerektiðini anlasýn
        EditorUtility.SetDirty(db);

        // 5. Deðiþiklikleri diske yaz
        AssetDatabase.SaveAssets();

<<<<<<< Updated upstream
        Debug.Log($"Ýþlem Tamam: {foundCards.Count} adet kart bulundu ve veritabanýna eklendi.");
=======
        Debug.Log($"Ýþlem Tamam: {foundActionCards.Count} adet kart bulundu ve veritabanýna eklendi.");
>>>>>>> Stashed changes
    }
}