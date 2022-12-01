using UnityEngine;

public static class MenuManager
{
    public static bool IsInitialised { get; private set; }

    public static GameObject mainMenu, settings;
    public static void Init()
    {
        GameObject canvas = GameObject.Find("MenuCanvas");
        mainMenu = canvas.transform.Find("MainMenu").gameObject;
        settings = canvas.transform.Find("OptionsPanel").gameObject;
        IsInitialised = true;
    }

    public static void OpenMenu(Menu menu, GameObject callingMenu)
    {
        if (!IsInitialised)
            Init();

        switch(menu)
        {
            case Menu.MAIN_MENU:
                mainMenu.SetActive(true);
                break;
            case Menu.SETTINGS:
                settings.SetActive(true);
                break;
        }

        callingMenu.SetActive(false);
    }
}
