using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class FlyingBumb : Fly
{
    protected override void Attack(Actor target)
    {
        // Explode with rigidbody and damage the player if is in range
        // Add force to all the surrounding colliders.
        Debug.Log("Exploding");
        var colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var collider in colliders)
        {
            var rb = collider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                AddExplosionForce(rb, 1000f, transform.position, 5f, 0.5f, ForceMode2D.Impulse);
            }
        }
        
        // Damage the player
        if (target != this)
        {
            target.TakeDamage(damage);
        }
        Instantiate(deathEffect, transform.position, deathEffect.transform.rotation);
        Destroy(gameObject);

    }

    private void AddExplosionForce(Rigidbody2D _rb, float explosionForce, Vector2 explosionPosition,
        float explosionRadius, float upwardsModifier = 0.0F, ForceMode2D mode = ForceMode2D.Force)
    {
        if (_rb == rb) return;

        var explosionDir = _rb.position - explosionPosition;
        var explosionDistance = (explosionDir.magnitude / explosionRadius);

        // Normalize without computing magnitude again
        if (upwardsModifier == 0)
        {
            explosionDir /= explosionDistance;
        }
        else
        {
            // If you pass a non-zero value for the upwardsModifier parameter, the direction
            // will be modified by subtracting that value from the Y component of the centre point.
            explosionDir.y += upwardsModifier;
            explosionDir.Normalize();
        }

        if (Mathf.Approximately(Time.deltaTime, 0f)) return;
        try
        {
            _rb.AddForce(Mathf.Lerp(0, explosionForce, (1 - explosionDistance)) * explosionDir, mode);
        }
        catch
        {
            // ignored
        };
    }

    protected override void Die()
    {
        base.Die();
        Attack(this);
    }
}
