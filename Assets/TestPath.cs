using System;
using System.Collections;
using System.Collections.Generic;
using General;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPath : MonoBehaviour
{
    private PathFinding _pathFinding;
    void Awake()
    {
        _pathFinding = new PathFinding(50, 50);
    }
    
}
