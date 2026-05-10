using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

// CustomEditor içindeki tipi Card (ScriptableObject olan sýnýfýn adý) olarak deðiþtir
[CustomEditor(typeof(CardSO))]
public class CardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 1. Standart deðiþkenleri çiz (ScriptableObject içindeki deðerler)
        base.OnInspectorGUI();

        CardSO card = (CardSO)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Component Ýþlemleri", EditorStyles.boldLabel);

        // 2. CardComponent'ten türeyen sýnýflarý bul
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(CardComponent).IsAssignableFrom(p) && !p.IsAbstract);

        // 3. Her tip için bir buton oluþtur
        foreach (var type in types)
        {
            if (GUILayout.Button($"+ {type.Name} Ekle"))
            {
                // Deðiþikliði kaydetmek için Undo sistemine kaydet (Ctrl+Z çalýþmasý için)
                Undo.RecordObject(card, "Add Card Component");

                card.components.Add((CardComponent)Activator.CreateInstance(type));

                // ScriptableObject'in deðiþtiðini ve kaydedilmesi gerektiðini Unity'ye bildir
                EditorUtility.SetDirty(card);
                AssetDatabase.SaveAssets(); // Deðiþikliði asset dosyasýna yaz
            }
        }

        if (card.components.Count > 0)
        {
            if (GUILayout.Button("Listeyi Temizle", EditorStyles.miniButtonMid))
            {
                Undo.RecordObject(card, "Clear Components");
                card.components.Clear();
                EditorUtility.SetDirty(card);
            }
        }
    }
}