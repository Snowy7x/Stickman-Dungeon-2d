using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    public static Player Instance;

    [SerializeField] float damageVelocity = 100f;
    [SerializeField] private Transform body;
    [SerializeField] private Transform[] bloodSplashes;
    [SerializeField] private Rigidbody2D mainRigidbody;
    //[SerializeField] float bounceForce = 1000f;

    private void Awake()
    {
        if (!Instance) Instance = this;
        if (!mainRigidbody) mainRigidbody = body.GetComponent<Rigidbody2D>();
        
        // Free the allocated cash
        
        
        //camController.SetTarget(transform);
        //camController.SetMovement(movement);
        //rb = GetComponent<Rigidbody2D>();
    }

    public Transform GetTarget()
    {
        return body;
    }
    
    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player collided with a wall
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Spawn a random blood splash if the velocity is high enough
            Debug.Log(mainRigidbody.velocity.magnitude);
            if (mainRigidbody.velocity.magnitude > damageVelocity)
            {
                int index = UnityEngine.Random.Range(0, bloodSplashes.Length);
                Instantiate(bloodSplashes[index], transform.position, Quaternion.identity);
            }
            
            // Calculate the direction from the wall to the player
            // Vector2 direction = collision.GetContact(0).normal;

            // Apply a force in the opposite direction to the collision
            // rb.AddForce(direction.normalized * bounceForce, ForceMode2D.Impulse);
        }
    }*/

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            if (mainRigidbody.velocity.magnitude > damageVelocity)
            {
                Vector3 pos = other.ClosestPoint(body.transform.position);
                int index = UnityEngine.Random.Range(0, bloodSplashes.Length);
                Instantiate(bloodSplashes[index], pos, Quaternion.identity);
            }
        }
    }
}
