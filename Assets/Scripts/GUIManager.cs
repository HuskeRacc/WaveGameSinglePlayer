using TMPro;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ammoCounter;
    [SerializeField] GunScript gunScript;
    
    private void Update()
    {
        if (gunScript.armAnim.GetBool("reload"))
        {
            ammoCounter.text = "RELOADING...";
        }
        else
        {
            ammoCounter.text = gunScript.currentAmmo + " / " + gunScript.maxAmmo;
        }
    }
}
