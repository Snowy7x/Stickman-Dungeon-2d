using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Balance : MonoBehaviour
{
    public float targetRotation;
    private Rigidbody2D rb;
    public float force;
    [SerializeField] Player player;

    private void Start()
    {
        if (!player) player = Player.Instance;
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        //rb.MoveRotation(Mathf.LerpAngle(rb.rotation, targetRotation, Time.deltaTime * force));
        // rotate towards target rotation
        rb.MoveRotation(Mathf.LerpAngle(rb.rotation, targetRotation, Time.deltaTime * force));
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        player.OnTriggerEnter2D(col);
    }
}
