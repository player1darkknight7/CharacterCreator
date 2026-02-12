using UnityEngine;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    public Slider heightSlider; // drag your slider here in Inspector

    void Update()
    {
        // Change character height based on slider
        Vector3 scale = transform.localScale;
        scale.y = heightSlider.value;
        transform.localScale = scale;
    }
}
