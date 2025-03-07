using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTank : Tank
{

    void Update()
    {
        float moveInput = InputManager.instance.GetMoveInput();
        float turnInput = InputManager.instance.GetTurnInput();
        handleInput(moveInput, turnInput);
        
    }

    void handleInput(float moveInput, float turnInput)
    {
        handleMove(moveInput);
        handleTurn(turnInput);
    }
}
