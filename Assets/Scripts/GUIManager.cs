using TMPro;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ammoCounter;
    [SerializeField] TextMeshProUGUI healthCounter;
    [SerializeField] TextMeshProUGUI waveCounter;
    [SerializeField] GunScript gunScript;
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] WaveSpawner waveSpawner;

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

        healthCounter.text = playerHealth.health.ToString("F0");

        waveCounter.text = waveSpawner.currWave.ToString("F0");
    }
}
