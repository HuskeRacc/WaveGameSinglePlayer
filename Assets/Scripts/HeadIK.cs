using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadIK : MonoBehaviour
{
    [SerializeField] GameObject cam;
    [SerializeField] Transform head;
    [SerializeField] Transform headLock;
    [SerializeField] Vector3 HeadOffset;

    private void LateUpdate()
    {
        Quaternion rot = Quaternion.Euler(headLock.transform.eulerAngles.x, headLock.transform.eulerAngles.y, -cam.transform.eulerAngles.x) * Quaternion.Euler(HeadOffset);
        head.transform.rotation = cam.transform.rotation * Quaternion.Euler(HeadOffset);
    }
}
