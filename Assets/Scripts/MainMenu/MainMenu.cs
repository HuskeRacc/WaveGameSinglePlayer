using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject mainMenuGO;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Play()
    {
        // can add a level selection.
        SceneManager.LoadScene(1);
    }

    public void Options()
    {
        optionsPanel.SetActive(true);

        mainMenuGO.SetActive(false);
    }

    public void OptionsBackButton()
    {
        optionsPanel.SetActive(false);

        mainMenuGO.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
