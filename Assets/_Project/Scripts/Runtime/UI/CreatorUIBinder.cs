using UnityEngine;
using UnityEngine.UI;

public class CreatorUIBinder : MonoBehaviour
{
    public BlendShapeDriver driver;

    public Slider sliderBust;
    public Slider sliderWaist;
    public Slider sliderHips;
    public Slider sliderNoseWidth;

    public string presetFileName = "MyPreset01";

    public string bsBust = "Bust";
    public string bsWaist = "Waist";
    public string bsHips = "Hips";
    public string bsNoseWidth = "NoseWidth";

    bool _applying;

    void Start()
    {
        sliderBust.onValueChanged.AddListener(_ => OnChanged());
        sliderWaist.onValueChanged.AddListener(_ => OnChanged());
        sliderHips.onValueChanged.AddListener(_ => OnChanged());
        sliderNoseWidth.onValueChanged.AddListener(_ => OnChanged());

        ApplyToCharacter();
    }

    void OnChanged()
    {
        if (_applying) return;
        ApplyToCharacter();
    }

    void ApplyToCharacter()
    {
        if (driver == null) return;

        driver.Set(bsBust, sliderBust.value);
        driver.Set(bsWaist, sliderWaist.value);
        driver.Set(bsHips, sliderHips.value);
        driver.Set(bsNoseWidth, sliderNoseWidth.value);
    }

    public void SavePreset()
    {
        var p = new CharacterPreset
        {
            bust = sliderBust.value,
            waist = sliderWaist.value,
            hips = sliderHips.value,
            noseWidth = sliderNoseWidth.value
        };
        PresetIO.Save(presetFileName, p);
    }

    public void LoadPreset()
    {
        var p = PresetIO.Load(presetFileName);
        if (p == null) return;

        _applying = true;
        sliderBust.value = p.bust;
        sliderWaist.value = p.waist;
        sliderHips.value = p.hips;
        sliderNoseWidth.value = p.noseWidth;
        _applying = false;

        ApplyToCharacter();
    }

    public void ResetPreset()
    {
        var p = CharacterPreset.Default();

        _applying = true;
        sliderBust.value = p.bust;
        sliderWaist.value = p.waist;
        sliderHips.value = p.hips;
        sliderNoseWidth.value = p.noseWidth;
        _applying = false;

        ApplyToCharacter();
    }
}
