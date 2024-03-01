using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hand : MonoBehaviour
{
    public bool isRightHand = true;
    private Vector2 dir;
    
    public void Start()
    {
        InputManager.Instance.onLook.AddListener(OnLook);
    }

    private void OnLook(Vector2 arg0)
    {
        dir = arg0;
    }

    private void Update()
    {
        Vector2 lookDirection = Vector2.zero;
        // Check if the input device is mouse or gamepad
        if (InputManager.Instance.GetCurrentControlSchema() == "Keyboard&Mouse")
        {
            // Get the mouse position in world coordinates
            if (Camera.main != null)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                lookDirection = mousePos - transform.position;
            }
        }
        else if (InputManager.Instance.GetCurrentControlSchema() == "Gamepad")
        {
            // Get the stick input value
            lookDirection = dir;
        }
        // Point the hand towards the look direction
        transform.up = lookDirection.normalized * (isRightHand ? -1 : 1);
    }
}
