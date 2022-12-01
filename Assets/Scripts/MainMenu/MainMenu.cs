using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnClick_Play()
    {
        // can add a level selection.
        SceneManager.LoadScene(1);
    }

    public void OnClick_Options()
    {
        MenuManager.OpenMenu(Menu.SETTINGS, gameObject);
    }

    public void OnClick_QuitGame()
    {
        Application.Quit();
    }
}
