using System.Collections;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] float minDamage;
    [SerializeField] float maxDamage;
    [SerializeField] float damage;
    [SerializeField] float range = 100f;

    [Header("Ammo")]
    public int maxAmmo = 17;
    public int currentAmmo;
    [SerializeField] float reloadTime = 6f;
    [SerializeField] float timeBetweenShots;

    [Header("Assignables")]
    [SerializeField] Camera fpsCam;
    [SerializeField] ParticleSystem muzzleFlash;
    public Animator armAnim;
    [SerializeField] GameObject[] bulletHolePrefabs;
    [SerializeField] LayerMask rayLayerMask;
    [SerializeField] PoolManager pool;
    [SerializeField] Canvas xHairCanvas;
    [SerializeField] PlayerMovement player;

    private void Start()
    {
        fpsCam = Camera.main;
        currentAmmo = maxAmmo;
        pool = GameObject.FindObjectOfType<PoolManager>();
        
    }

    void Update()
    {
        InputHandler();
        AmmoClamp();
    }



    void InputHandler()
    {
        if (Input.GetMouseButton(0) && currentAmmo > 0 && !armAnim.GetBool("shoot") && !armAnim.GetBool("reload") && !player.IsSprinting)
        {
            StartCoroutine(Shoot());
        }

        if (currentAmmo <= 0 && currentAmmo != maxAmmo && !armAnim.GetBool("reload") && !armAnim.GetBool("lastround") && !player.IsSprinting ||
            currentAmmo != maxAmmo && Input.GetKeyDown(KeyCode.R) && !armAnim.GetBool("reload") && !armAnim.GetBool("lastround") && !player.IsSprinting)
        {
            StartCoroutine(Reload());
        }

        if(Input.GetMouseButtonDown(1) && !armAnim.GetBool("reload") && !armAnim.GetBool("ads") && !player.IsSprinting)
        {
            ADS();
        }
        else if(Input.GetMouseButtonDown(1) && !armAnim.GetBool("reload") && armAnim.GetBool("ads") && !player.IsSprinting)
        {
            ADSOut();
        }

    }

    void AmmoClamp()
    {
        if (currentAmmo > maxAmmo)
            currentAmmo = maxAmmo;

        if (currentAmmo < 0)
            currentAmmo = 0;
    }

    void ADS()
    {
        xHairCanvas.gameObject.SetActive(false);
        armAnim.SetBool("ads", true);
    }

    void ADSOut()
    {
        xHairCanvas.gameObject.SetActive(true);
        armAnim.SetBool("ads", false);
    }

    void DamageRNG()
    {
        damage = Random.Range(minDamage, maxDamage);
    }

    IEnumerator Shoot()
    {
        DamageRNG();

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

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, rayLayerMask))
        {
            Target target = hit.transform.GetComponent<Target>();
            EnemyController enemy = hit.transform.GetComponent<EnemyController>();
            Debug.Log(hit.transform.name);
            CreateDecalBulletHole(hit);

            if(enemy != null && target == null)
            {
                Debug.Log("Hit " + hit.transform.name);
                enemy.TakeDamage(damage);
            }

            if (target != null && enemy == null)
            {
                Debug.Log("Hit " + hit.transform.name);
                target.TakeDamage(damage);
            }
        }


        yield return new WaitForSeconds(timeBetweenShots);

        armAnim.SetBool("shoot", false);
        armAnim.SetBool("lastround", false);
    }

    public void CreateDecalBulletHole(RaycastHit hit)
    {
        GameObject randomBullet = bulletHolePrefabs[Random.Range(0, bulletHolePrefabs.Length)];

        //Iterate through hole pool list
        //enable objects that are active false

        //Unecessary but fuck it
        Vector3 hitRotation = hit.normal; 
        Vector3 hitPosition = hit.point; 

        for (int i = 0; i < pool.bulletHoleList.Count; i++)
        {
            GameObject currentBulletHole = pool.bulletHoleList[i];

            if (currentBulletHole.activeInHierarchy == false)
            {
                if (hit.collider.gameObject.CompareTag("env"))
                {
                    currentBulletHole.SetActive(true);
                    currentBulletHole.transform.position = hitPosition;
                    currentBulletHole.transform.rotation = Quaternion.LookRotation(hitRotation);
                    break;
                }
            }
            else
            {
                //create new bullet if on last item on list
                if(i == pool.bulletHoleList.Count - 1)
                {
                    //last bullet
                    GameObject newBullet = Instantiate(pool.bulletHolePrefab) as GameObject;
                    newBullet.transform.parent = pool.transform;
                    newBullet.SetActive(false);
                    pool.bulletHoleList.Add(newBullet);
                }
            }
        }
    }

    IEnumerator Reload()
    {
        armAnim.SetBool("reload", true);
        
        yield return new WaitForSeconds(reloadTime);

        armAnim.SetBool("reload", false);

        currentAmmo = maxAmmo;
    }


}
