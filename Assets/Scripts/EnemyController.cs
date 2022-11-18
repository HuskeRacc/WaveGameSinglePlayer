using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public List<Collider> ragdollParts = new();
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;

    private void Awake()
    {
        SetRagdollParts();
    }

    void SetRagdollParts()
    {
        Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();

        foreach(Collider c in colliders)
        {
            if( c.gameObject != this.gameObject)
            {
                c.isTrigger = true;
                ragdollParts.Add(c);
            }
        }
    }

    void TurnOnRagdoll()
    {
        rb.useGravity = false;
        this.GetComponent<CapsuleCollider>().enabled = false;
        anim.enabled = false;

        foreach(Collider c in ragdollParts)
        {
            c.isTrigger = false;
            c.attachedRigidbody.velocity = Vector3.zero;
        }
    }
}
