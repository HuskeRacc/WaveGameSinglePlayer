using TMPro;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ammoCounter;
    [SerializeField] TextMeshProUGUI healthTXT;
    [SerializeField] GunScript gunScript;
    [SerializeField] PlayerHealth playerHealth;
    
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

        healthTXT.text = playerHealth.health.ToString("F0");
    }
}
