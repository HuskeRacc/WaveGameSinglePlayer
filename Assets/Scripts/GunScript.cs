using System.Collections;
using UnityEngine;

public class GunScript : MonoBehaviour
{

    [SerializeField] float damage = 10f;
    [SerializeField] float range = 100f;
    public int maxAmmo = 17;
    public int currentAmmo;
    [SerializeField] float reloadTime = 6f;
    [SerializeField] float timeBetweenShots;

    [SerializeField] Camera fpsCam;

    [SerializeField] ParticleSystem muzzleFlash;
    public Animator armAnim;

    [SerializeField] GameObject[] bulletHolePrefabs;

    [SerializeField] LayerMask rayLayerMask;

    [SerializeField] PoolManager pool;

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
        if (Input.GetMouseButton(0) && currentAmmo > 0 && !armAnim.GetBool("shoot") && !armAnim.GetBool("reload"))
        {
            StartCoroutine(Shoot());
        }

        if (currentAmmo <= 0 && currentAmmo != maxAmmo && !armAnim.GetBool("reload") && !armAnim.GetBool("lastround") ||
            currentAmmo != maxAmmo && Input.GetKeyDown(KeyCode.R) && !armAnim.GetBool("reload") && !armAnim.GetBool("lastround"))
        {
            StartCoroutine(Reload());
        }

        if(Input.GetMouseButtonDown(1) && !armAnim.GetBool("reload") && !armAnim.GetBool("ads"))
        {
            ADS();
        }
        else if(Input.GetMouseButtonDown(1) && !armAnim.GetBool("reload") && armAnim.GetBool("ads"))
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
        armAnim.SetBool("ads", true);
    }

    void ADSOut()
    {
        armAnim.SetBool("ads", false);
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

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, rayLayerMask))
        {
            Target target = hit.transform.GetComponent<Target>();
            Debug.Log(hit.transform.name);
            CreateDecalBulletHole(hit);


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

    void CreateDecalBulletHole(RaycastHit hit)
    {
        GameObject randomBullet = bulletHolePrefabs[Random.Range(0, bulletHolePrefabs.Length)];


        //Iterate through hole pool list
        //enable objects that are active false
        //Instantiate(randomBullet, hit.point, Quaternion.LookRotation(hit.normal));

        //Unecessary but fuck it
        Vector3 hitRotation = hit.normal; 
        Vector3 hitPosition = hit.point; 

        for (int i = 0; i < pool.bulletHoleList.Count; i++)
        {
            GameObject currentBulletHole = pool.bulletHoleList[i];

            if (currentBulletHole.activeInHierarchy == false)
            {
                currentBulletHole.SetActive(true);
                currentBulletHole.transform.position = hitPosition;
                currentBulletHole.transform.rotation = Quaternion.LookRotation(hitRotation);
                break;
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
