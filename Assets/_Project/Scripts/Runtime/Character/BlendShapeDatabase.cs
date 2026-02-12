using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Creator/BlendShape Database", fileName = "BlendShapeDatabase")]
public class BlendShapeDatabase : ScriptableObject
{
    public List<BlendShapeSliderDef> sliders = new();
}
