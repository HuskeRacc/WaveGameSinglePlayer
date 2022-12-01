using TMPro;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ammoCounter;
    [SerializeField] TextMeshProUGUI healthCounter;
    [SerializeField] TextMeshProUGUI waveCounter;
    [SerializeField] TextMeshProUGUI staminaCounter;
    [SerializeField] GunScript gunScript;
    [SerializeField] WaveSpawner waveSpawner;

    private void OnEnable()
    {
        PlayerMovement.OnDamage += UpdateHealth;
        PlayerMovement.OnHeal += UpdateHealth;
        PlayerMovement.OnStaminaChange += UpdateStamina;
    }

    private void OnDisable()
    {
        PlayerMovement.OnDamage -= UpdateHealth;
        PlayerMovement.OnHeal -= UpdateHealth;
        PlayerMovement.OnStaminaChange -= UpdateStamina;
    }

    private void Start()
    {
        UpdateHealth(100);
        UpdateStamina(100);
    }

    void UpdateHealth(float currentHealth)
    {
        healthCounter.text = currentHealth.ToString("F0");
    }

    void UpdateStamina(float currentStamina)
    {
        staminaCounter.text = currentStamina.ToString("F0");
    }

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

        waveCounter.text = waveSpawner.currWave.ToString("F0");
    }
}
