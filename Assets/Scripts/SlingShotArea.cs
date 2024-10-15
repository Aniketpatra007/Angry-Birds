using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlingShotArea : MonoBehaviour
{
    [SerializeField] private LayerMask _slingShotAreaMask;
    public bool isWithinSlingShotArea()
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);

        if(Physics2D.OverlapPoint(worldPosition, _slingShotAreaMask))
        {
            return true;
        }
        else { return false; }
    }
}
