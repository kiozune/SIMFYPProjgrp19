using UnityEngine;
using UnityEngine.UI;

// for enemies, place the enemy and the slider object under a parent empty object to organize them
// ensure this script is attached to the slider itself
public class SliderBar : MonoBehaviour
{
    private Slider slider; 
    private Camera cam;
    [Tooltip("The entitity this slider is attached to.")]
    [SerializeField] private Transform entityAnchor = null;
    [Tooltip("Offset the slider to account for different entity sizes")] // must be adjusted in Inspector
    [SerializeField] private Vector3 sliderOffset = Vector3.zero; 

    private void Awake()
    {
        slider = GetComponent<Slider>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        
    }

    /// <summary>
    /// Adjust the slider; Call this function whenever the slider value needs to change
    /// </summary>
    /// <param name="value">New quantity of the value (i.e. currentHP)</param>
    /// <param name="maxValue">Max value of the variable this slider is presenting (i.e. maxHP)</param>
    public void UpdateBar(int value, int maxValue)
    {
        slider.value = (float)value / maxValue;
    }

    // override version for float values
    /// <summary>
    /// Adjust the slider; Call this function whenever the slider value needs to change
    /// </summary>
    /// <param name="value">New quantity of the value (i.e. currentHP)</param>
    /// <param name="maxValue">Max value of the variable this slider is presenting (i.e. maxHP)</param>
    public void UpdateBar(float value, float maxValue)
    {
        slider.value = value / maxValue;
    }

    private void Update() // mainly to fix positioning
    { 
        if (entityAnchor != null) // for EXP bar, not needed, as it is fixed in a Canvas
        {
            transform.rotation = cam.transform.rotation; // keep facing the camera
            transform.position = new Vector3(entityAnchor.position.x, // keep with the position of the entity
                entityAnchor.position.y,
                entityAnchor.position.z - 1f); // bring closer to the camera/in front of the entity

            if (sliderOffset != Vector3.zero) // only need to offset if offset is not (0,0,0)
                transform.position = transform.position + sliderOffset;
        } 
    }
}
