using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleManager : MonoBehaviour
{
    private Color32 normal = new Color32(135, 135, 135, 255);
    private Color32 selected = new Color32(123, 123, 123, 123);

    private void Start()
    {
        gameObject.GetComponent<Image>().color = normal;
    }

    public void ToggleChange()
    {
        if (gameObject.GetComponent<Toggle>().isOn)
            gameObject.GetComponent<Image>().color = selected;
        else
            gameObject.GetComponent<Image>().color = normal;
    }
}
