using System.Collections;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    void OnEnable()
    {
        Invoke("OnDisable", 5f);
    }

    private void OnDisable()
    {
        this.transform.gameObject.SetActive(false);
    }
}
