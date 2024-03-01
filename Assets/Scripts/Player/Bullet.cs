using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 1f;
    public float impactForce = 10f;
    public float lifeTime = 2f;
    public GameObject impactEffect;
    public Rigidbody2D rb;

    public void Init(float impactForce, float damage = 100f)
    {
        this.damage = damage;
        this.impactForce = impactForce;
        if (!rb) rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }

    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.attachedRigidbody)
        {
            col.collider.attachedRigidbody.AddForce(transform.right * impactForce, ForceMode2D.Impulse);
        }

        if (col.transform.CompareTag("Enemy"))
        {
            col.transform.GetComponent<Actor>()?.TakeDamage(damage);
        }

        Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
