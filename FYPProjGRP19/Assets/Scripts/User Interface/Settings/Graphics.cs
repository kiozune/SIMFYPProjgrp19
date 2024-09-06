using TMPro;
using UnityEngine; 

public class Graphics : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resDropDown;

    private void Awake()
    {
        if (resDropDown == null) // in case it has not been assigned
            resDropDown = GetComponentInChildren<TMP_Dropdown>();
    }

    public void ChangeResolution()
    {
        int idx = resDropDown.value;
        switch (idx)
        {
            case 0: // fullscreen window
                Debug.Log("Fullscreen window");
                Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow);
                break;
            case 1: // exclusive window
                Debug.Log("Exclusive window");
                Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.ExclusiveFullScreen);
                break;
            case 2: // maximized window
                Debug.Log("Max window");
                Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.MaximizedWindow);
                break;
            case 3: // windowed
                Debug.Log("Windowed");
                Screen.SetResolution(1600, 900, FullScreenMode.Windowed);
                break;
            default: // error - unexpected input
                Debug.LogError("ERROR: unexpected value - " +  idx);
                break;
        }
    }
}
