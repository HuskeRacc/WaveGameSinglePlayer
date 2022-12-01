using System.Collections;
using UnityEngine;

public class PlayerNet : MonoBehaviour
{
    [SerializeField] float lowestYPosAllowed = -10f;

    [SerializeField] Transform resetPoint;

    private void Update()
    {
        if (transform.position.y < lowestYPosAllowed)
        {
            StartCoroutine(ResetPosition());
        }
    }

    IEnumerator ResetPosition()
    {
        CharacterController controller = GetComponent<CharacterController>();
        PlayerMovement player = GetComponent<PlayerMovement>();
        player.enabled = false;
        controller.enabled = false;
        Debug.Log("Player fell under map!");
        transform.position = resetPoint.position;
        yield return new WaitForSeconds(.1f);
        player.enabled = true;
        controller.enabled = true;
    }
}
