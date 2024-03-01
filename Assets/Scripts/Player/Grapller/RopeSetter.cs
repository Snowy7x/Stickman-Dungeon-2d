using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class RopeSetter : MonoBehaviour
{
    // Start is called before the first frame update

    public Spider[] rope;
    private int index = 0;

    private void Start()
    {
        InputManager.Instance.onFire2.AddListener(FireRope);
        InputManager.Instance.onShiftInput.AddListener(CancelRope);
    }

    public void FireRope(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            index = index > rope.Length - 1 ? 0 : index;
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rope[index].setStart(worldPos);
            index++;
        }
    }

    public void CancelRope()
    {
        Spider[] ropes = rope.Where(x => x.update).ToArray();
        if (ropes.Length == 0) return; 
        ropes[0].Reset();
    }
}
