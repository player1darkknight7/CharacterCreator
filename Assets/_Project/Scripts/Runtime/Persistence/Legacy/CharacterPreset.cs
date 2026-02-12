using System;

[Serializable]
public class CharacterPreset
{
    public float bust;
    public float waist;
    public float hips;
    public float noseWidth;

    public static CharacterPreset Default() => new CharacterPreset();
}
