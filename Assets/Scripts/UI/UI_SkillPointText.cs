using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_SkillPointText : MonoBehaviour
{

    private int skillPoint => SkillTreeManager.Instance.skillPoints;
    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    void LateUpdate()
    {
        text.text=skillPoint.ToString();
    }
}
