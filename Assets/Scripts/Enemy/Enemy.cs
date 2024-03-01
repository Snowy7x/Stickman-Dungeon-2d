using System;
using Map;
using Map.Challenges;
using UnityEngine;

namespace Enemy
{
    public enum EnemyState
    {
        Idle,
        Following,
        Attacking,
    }
    
    public abstract class Enemy : Actor
    {
        [SerializeField] protected float followRange;
        [SerializeField] protected float attackRange;
        [SerializeField] protected float loseRange;
        [SerializeField] protected EnemyState currentState;
        [SerializeField] protected float damage = 10f;
        
        [SerializeField] protected ParticleSystem deathEffect;
        [SerializeField] protected ParticleSystem hitEffect;
        protected Transform _target = null;

        protected virtual void UpdateState()
        {
            if (_target == null) return;

            float distance = Vector2.Distance(transform.position, _target.position);
            // Choose the state depending on the distance:
            if (distance <= followRange && distance > attackRange)
            {
                currentState = EnemyState.Following;

            }else if (distance <= attackRange)
            {
                // TODO: Attack Logic.
                currentState = EnemyState.Attacking;
                
            }else if (distance > loseRange)
            {
                currentState = EnemyState.Idle;
            }
        }

        protected virtual void Update()
        {
            UpdateState();
        }
        
        protected abstract void Attack(Actor actor);

        protected override void Die()
        {
            base.Die();
            //OnDie?.Invoke(this);
        }
    }
}