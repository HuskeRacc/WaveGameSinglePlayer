using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] float respawnTime = 10f;
    [SerializeField] float healAmount = 30f;
    [SerializeField] MeshRenderer[] componentsToDisable;

    private void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        componentsToDisable = gameObject.GetComponentsInChildren<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (playerHealth.health != 100)
            StartCoroutine(DisableHealthBox());
        }
    }

    IEnumerator DisableHealthBox()
    {
        Debug.Log("health picked up");
        playerHealth.health += healAmount;
        DisableHealth();
        yield return new WaitForSeconds(respawnTime);
        EnableHealth();
    }

    void DisableHealth()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }   
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    void EnableHealth()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = true;
        }
        gameObject.GetComponent<BoxCollider>().enabled = true;
    }
}
