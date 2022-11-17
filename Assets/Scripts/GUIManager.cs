using TMPro;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ammoCounter;
    [SerializeField] GunScript gunScript;

    private void Update()
    {
        ammoCounter.text = gunScript.currentAmmo + " / " + gunScript.maxAmmo;
    }
}
