using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string Museumindoor;  // Name of the scene to load

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Check if the colliding object has the "Player" tag
        {
            SceneManager.LoadScene(Museumindoor);  // Load the specified scene
        }
    }
}
