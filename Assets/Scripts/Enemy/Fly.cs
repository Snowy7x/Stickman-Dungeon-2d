using System;
using System.Collections;
using System.Collections.Generic;
using General;
using Pathfinding;
using UnityEngine;

namespace Enemy
{
    public abstract class Fly : Enemy
    {
        [SerializeField] protected float speed = 5f;
        [SerializeField] protected float nextWaypointDistance = 3f;

        protected Path path;
        int currentWaypoint = 0;
        bool reachedEndOfPath = false;
        
        protected Seeker seeker;
        protected Rigidbody2D rb;

        protected override void Start()
        {
            base.Start();
            _target = Player.Instance?.GetTarget();
            seeker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody2D>();
        }

        void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                path = p;
                currentWaypoint = 0;
            }
        }
        
        void UpdatePath()
        {
            if (seeker.IsDone())
                seeker.StartPath(rb.position, _target.transform.position, OnPathComplete);
        }

        protected override void Update()
        {
            base.Update();
            switch (currentState)
            {
                case EnemyState.Attacking:
                    if (IsInvoking(nameof(UpdatePath))) CancelInvoke(nameof(UpdatePath));
                    Attack(Player.Instance);
                    break;
                
                case EnemyState.Following:
                    if (!IsInvoking(nameof(UpdatePath))) InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
                    FollowTarget();
                    break;
            }
            
        }

        private void FollowTarget()
        {
            if (path == null)
            {
                rb.velocity = Vector2.zero;
                return;
            }
            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                rb.velocity = Vector2.zero;
                return;
            }
            reachedEndOfPath = false;
            
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed * Time.deltaTime;

            rb.AddForce(force);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            var position = transform.position;
            Gizmos.DrawWireSphere(position, followRange);
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(position, loseRange);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, attackRange);
            
            if (_target == null) return;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position, _target.position);
        }
        
    }
    /*public abstract class Fly : Enemy
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float pathRefreshRate = 0.5f;

        private int currentWaypoint = 0;
        private List<Vector3> path;
        [SerializeField] protected Rigidbody2D rb;

        protected virtual void Start()
        {
            _target = Player.Instance?.GetTarget();
            rb = GetComponent<Rigidbody2D>();
            if (_target) StartCoroutine(UpdatePath());
        }

        protected override void Update()
        {
            base.Update();
            switch (currentState)
            {
                case EnemyState.Attacking:
                    Attack(Player.Instance);
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (currentState == EnemyState.Following)
            {
                FollowTarget();
            }
        }

        // Follow Target function using rb (Rigidbody):
        private void FollowTarget()
        {
            if (path == null) return;
            if (currentWaypoint >= path.Count) return;
            Vector3 direction = (path[currentWaypoint] - transform.position).normalized;
            Vector3 velocity = direction * speed;
            rb.MovePosition(rb.position + (Vector2)velocity * Time.fixedDeltaTime);
            float distance = Vector2.Distance(transform.position, path[currentWaypoint]);
            if (distance <= 0.3f)
            {
                currentWaypoint++;
            }
        }

        IEnumerator UpdatePath() {
            while (true)
            {
                try
                {
                    // Get the shortest path from the Enemy's current position to the player's position
                    path = PathFinding.Instance.FindPath(transform.position, _target.transform.position);
                    SetStartPosition();
                }
                catch (Exception e)
                {
                    Debug.Log("Broke error: " + e);
                    break;
                }
                yield return new WaitForSeconds(pathRefreshRate); // Wait for a short interval before recalculating the path
            }
        }

        protected abstract override void Attack(Actor actor);

        public void SetStartPosition()
        {
            currentWaypoint = 0;
            
            if (path != null && path.Count > 1)
            {
                path.RemoveAt(0);
            }
        }

        protected override void Die()
        {
            base.Die();
            StopAllCoroutines();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            var position = transform.position;
            Gizmos.DrawWireSphere(position, followRange);
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(position, loseRange);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, attackRange);
            
            if (_target == null) return;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position, _target.position);
        }
    }*/
}