using CnControls;
using System;
using UnityEngine;

public class GameInput : SingletonDestroy<GameInput>
{
    public event EventHandler OnInteractButtonPressed;

    private const string ForwardAxis = "Horizontal";
    private const string StrafeAxis = "Vertical";
    private const string PlayerRotationXAxis = "Camera X";
    private const string PlayerRotationYAxis = "Camera Y";
    private const string InteractButton = "Interact";
    private Vector2 rotation = Vector2.zero;

    private void Update()
    {
        if (CnInputManager.GetButtonDown(InteractButton))
        {
            OnInteractButtonPressed?.Invoke(this, EventArgs.Empty);
        };
    }
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = new Vector2(CnInputManager.GetAxis(ForwardAxis), CnInputManager.GetAxis(StrafeAxis));
        return inputVector.normalized;
    }

    public Vector2 GetRotationVector()
    {
        Vector2 rotationVector = new Vector2(CnInputManager.GetAxis(PlayerRotationXAxis), CnInputManager.GetAxis(PlayerRotationYAxis));
        return rotationVector;
    }

}
