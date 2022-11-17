using System.Collections;
using UnityEngine;

public class GunScript : MonoBehaviour
{

    [SerializeField] float damage = 10f;
    [SerializeField] float range = 100f;
    [SerializeField] float fireRate = 15f;
    [SerializeField] float timeToFire = 0f;
    public int maxAmmo = 17;
    public int currentAmmo;
    [SerializeField] float reloadTime = 6f;
    [SerializeField] float timeBetweenShots;

    [SerializeField] Camera fpsCam;

    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] Animator armAnim;

    private void Start()
    {
        fpsCam = Camera.main;
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        InputHandler();
        AmmoClamp();
    }



    void InputHandler()
    {
        if (Input.GetMouseButton(0) && currentAmmo > 0 && !armAnim.GetBool("shoot"))
        {
            StartCoroutine(Shoot());
        }

        if (currentAmmo <= 0 && !armAnim.GetBool("reload") && !armAnim.GetBool("lastround") || Input.GetKeyDown(KeyCode.R) && !armAnim.GetBool("reload"))
            StartCoroutine(Reload());
    }

    void AmmoClamp()
    {
        if (currentAmmo > maxAmmo)
            currentAmmo = maxAmmo;

        if (currentAmmo < 0)
            currentAmmo = 0;
    }

    IEnumerator Shoot()
    {
        if (currentAmmo > 1)
        {
            armAnim.SetBool("shoot", true);
            currentAmmo--;
        }
        else
        {
            armAnim.SetBool("lastround", true);
            currentAmmo--;
        }
        muzzleFlash.Play();

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Target target = hit.transform.GetComponent<Target>();

            if (target != null) 
            {
                Debug.Log("Hit " + hit.transform.name);
                target.TakeDamage(damage);
            }
        }

        yield return new WaitForSeconds(timeBetweenShots);
        armAnim.SetBool("shoot", false);
        armAnim.SetBool("lastround", false);
    }

    IEnumerator Reload()
    {
        armAnim.SetBool("reload", true);
        
        yield return new WaitForSeconds(reloadTime);

        armAnim.SetBool("reload", false);

        currentAmmo = maxAmmo;
    }


}
