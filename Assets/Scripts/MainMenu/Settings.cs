using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{

    public void OnClick_OptionsBackButton()
    {
        MenuManager.OpenMenu(Menu.MAIN_MENU, gameObject);
    }
}
