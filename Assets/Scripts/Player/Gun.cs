using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public GameObject bullet;
    public Rigidbody2D body;
    public Transform bulletSpawn;
    public float damage = 10f;
    public float fireRate = 15f;
    public float range = 100f;
    public float impactForce = 30f;
    public float recoil = 0.1f;
    
    public bool isAutomatic = false;
    

    void Start()
    {
        InputManager.Instance.onFire.AddListener(OnFire);
    }

    void OnFire(InputAction.CallbackContext value)
    {
        if (value.phase == InputActionPhase.Started)
        {
            // Code to run when the button is first pressed
        }
        else if (value.phase == InputActionPhase.Performed)
        {
            Shoot();
        }
        else if (value.phase == InputActionPhase.Canceled)
        {
            // Code to run when the button is released
        }
    }

    void Shoot()
    {
        if (isAutomatic)
        {
            StartCoroutine(ShootContinuously());
        }
        else
        {
            ShootSingle();
        }
    }

    IEnumerator ShootContinuously()
    {
        while (Input.GetButton("Fire1"))
        {
            ShootSingle();
            yield return new WaitForSeconds(1 / fireRate);
        }
    }
    
    void ShootSingle()
    {
        GameObject bulletObj = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
        bulletObj.GetComponent<Bullet>().Init(impactForce, damage);
        body.AddForce(-transform.right * recoil, ForceMode2D.Impulse);
    }
}
