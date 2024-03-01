using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Actor : MonoBehaviour
{
    [SerializeField] float health;
    [SerializeField] float shield;

    private bool _isDead;
    private bool _canTakeDamage;
    public Action<Actor> OnDie { get; set; }

    protected virtual void Start()
    {
        _canTakeDamage = true;
        health = 100;
    }

    public virtual void TakeDamage(float damage)
    {
        if (!_canTakeDamage) return;
        if (shield > 0)
        {
            shield -= damage;
            if (shield < 0)
            {
                health += shield;
                shield = 0;
            }
        }
        else
        {
            health = Mathf.Max(health - damage, 0);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        _isDead = true;
    }
}