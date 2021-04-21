using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private string JoystickX = "Horizontal";
    [SerializeField] private string JoystickY = "Vertical";
    
    [SerializeField] private string MouseX = "Mouse X";
    [SerializeField] private string MouseY = "Mouse Y";

    [SerializeField] private Vector2 RawMove = Vector2.zero;
    [SerializeField] private Vector2 RawMouse = Vector2.zero;

    void Update()
    {
        RawMove[0] = Input.GetAxisRaw(JoystickX);        
        RawMove[1] = Input.GetAxisRaw(JoystickY);
        RawMove = Vector2.ClampMagnitude(RawMove, 1.0F);
    
        RawMouse[0] = Input.GetAxisRaw(MouseX);
        RawMouse[1] = Input.GetAxisRaw(MouseY);
    }
}
