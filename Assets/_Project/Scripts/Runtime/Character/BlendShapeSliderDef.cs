using UnityEngine;

[CreateAssetMenu(menuName = "Character Creator/BlendShape Slider", fileName = "BS_")]
public class BlendShapeSliderDef : ScriptableObject
{
    [Header("Identity")]
    public string id = "bust";                 // stable key for save/load
    public string displayName = "Bust";        // label text shown in UI
    public string blendShapeName = "Bust";     // actual mesh blendshape name

    [Header("Value")]
    [Range(0f, 100f)] public float defaultValue = 0f;
    [Range(0f, 100f)] public float min = 0f;
    [Range(0f, 100f)] public float max = 100f;

    [Header("UI")]
    public string category = "Body";           // later: tabs/filters
    public bool showAsPercent = true;
}
