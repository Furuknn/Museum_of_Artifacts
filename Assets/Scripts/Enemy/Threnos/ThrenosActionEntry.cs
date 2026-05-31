using UnityEngine;

[System.Serializable]
public class ThrenosActionEntry
{
    public string actionName;           // must match a case in ThrenosController.ExecuteAction()

    [Range(0f, 1f)]
    public float probability = 0.25f;  // chance to trigger each tick

    [Tooltip("Min player distance for this action to be eligible.")]
    public float minRange = 0f;

    [Tooltip("Max player distance. Use 999 for no upper bound.")]
    public float maxRange = 999f;
}