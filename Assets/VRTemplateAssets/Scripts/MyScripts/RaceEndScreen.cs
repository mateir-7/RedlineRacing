using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RaceEndScreen : MonoBehaviour
{
    public Button restartButton;

    void Start()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartRace);
    }

    void RestartRace()
    {
        // reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}