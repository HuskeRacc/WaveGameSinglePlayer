using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsIK : MonoBehaviour
{
    [SerializeField] Animator animator;

    [SerializeField] Transform primaryGrip;
    [SerializeField] Transform secondaryGrip;

    private void OnAnimatorIK(int layerIndex)
    {

    }

}
