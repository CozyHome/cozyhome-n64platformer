using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Singleton;
using com.cozyhome.Console;
using com.cozyhome.Systems;
using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    [Header("Player Input Settings")]
    [SerializeField] private bool Listen = true;

    [SerializeField] private string JoystickX = "Horizontal";
    [SerializeField] private string JoystickY = "Vertical";

    [SerializeField] private string MouseX = "Mouse X";
    [SerializeField] private string MouseY = "Mouse Y";

    [SerializeField] private string XButton = "Fire1";
    [SerializeField] private string SquareButton = "Fire2";
    [SerializeField] private string LeftTrigger = "LeftTrigger";

    [SerializeField] private Vector2 RawMove = Vector2.zero;
    [SerializeField] private Vector2 RawMouse = Vector2.zero;

    [SerializeField] private bool RawXButton = false;
    [SerializeField] private bool RawSquareButton = false;
    [SerializeField] private bool RawLeftTrigger = false;

    [SerializeField] private InputTrigger XTrigger;
    [SerializeField] private InputTrigger SquareTrigger;

    private bool ConsoleActive;

    void Start() 
    {
        XTrigger = new InputTrigger();
        SquareTrigger = new InputTrigger();

        // public delegate void Command(string[] modifiers, out string output);
        MonoConsole.InsertCommand("ms", Func_MouseMode);
        MonoConsole.InsertToggleListener(Func_ConsoleToggled);
    }

    void Update()
    {
        if (!Listen || ConsoleActive)
            return;

        float DT = Time.deltaTime;

        RawMove[0] = Input.GetAxisRaw(JoystickX);
        RawMove[1] = Input.GetAxisRaw(JoystickY);
        RawMove = Vector2.ClampMagnitude(RawMove, 1.0F);

        RawMouse[0] = Input.GetAxisRaw(MouseX);
        RawMouse[1] = Input.GetAxisRaw(MouseY);

        RawXButton = Input.GetAxisRaw(XButton) > 0;
        RawSquareButton = Input.GetAxisRaw(SquareButton) > 0;
        RawLeftTrigger = Input.GetAxisRaw(LeftTrigger) > 0;
    
        XTrigger.Tick(DT, RawXButton);
        SquareTrigger.Tick(DT, RawSquareButton);

    }

    public Vector2 GetRawMove => RawMove;
    public Vector2 GetRawMouse => RawMouse;
    public bool GetXButton => RawXButton;
    public bool GetSquareButton => RawSquareButton;
    public bool GetLeftTrigger => RawLeftTrigger;
    public bool GetXTrigger => XTrigger.Consume();
    public bool GetSquareTrigger => SquareTrigger.Consume();

    // delegate ConsoleHeader.ToggleRelay
    void Func_ConsoleToggled(bool IsActive) => ConsoleActive = IsActive;
    // delegate ConsoleHeader.Command
    void Func_MouseMode(string[] modifiers, out string output)
    {
        if (modifiers.Length >= 1 && string.IsNullOrEmpty(modifiers[0]))
        {
            output = "error: invalid input on mouse state";
            switch (modifiers[0])
            {
                case "-c":
                case "-2":
                    Cursor.lockState = CursorLockMode.Confined;
                    output = "Cursor state is now confined";
                    break;
                case "-l":
                case "-1":
                    Cursor.lockState = CursorLockMode.Locked;
                    output = "Cursor state is now locked";
                    break;
                case "-f":
                case "-0":
                    Cursor.lockState = CursorLockMode.None;
                    output = "Cursor state is now free";
                    break;
            }
        }
        else
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                output = "Cursor state is now free";
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                output = "Cursor state is now locked";
            }
        }

        return;
    }
}

// Consume: If input press is true, return true and turn it off afterwards
// Attack: The length of how long the input buffering will last.
[System.Serializable]
public class InputTrigger
{
    [SerializeField] private float AttackLength = (10F / 60F);
    private bool Consumed, Active, LastRaw = false;
    private float Attack = (10F / 60F);

    // Ran by PlayerInput every frame
    public void Tick(float DT, bool Raw)
    {
        if(Consumed || Attack <= 0F)
            Clear();

        if (ValidPress(Raw) && !Active)
            Set();

        Attack -= DT;

        LastRaw = Raw;
    }

    public bool ValidPress(bool Raw) => Raw && !LastRaw;

    // States will have access to this method through a middlman return in PlayerInput
    public bool Consume()
    {
        if (Active)
        {
            Consumed = true;
            return true;
        }
        else
            return false;
    }

    public void Set()
    {
        Active = true;
        Consumed = false;
        Attack = (10F / 60F);
    }

    public void Clear()
    {
        Active = false;
        Attack = 0F;
    }
}