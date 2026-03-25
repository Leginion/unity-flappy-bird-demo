using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    private MoveController move;

    [SerializeField]
    private BirdController bird;

    [SerializeField]
    private float moveAmount = 10.0f;

    void Update()
    {
        var gs = GameManager.GetCurrentGameState();

        if (gs == GameStateType.Intro)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                GameManager.ChangeGameState(GameStateType.Play);
                bird.Jump();
            }
        }
        else if (gs == GameStateType.Result)
        {
            // skip
        }
        else
        {
            move.SetVelocityX(0f);
            move.SetVelocityY(0f);

            if (Input.GetKey(KeyCode.UpArrow))
            {
                move.SetVelocityY(moveAmount * 1);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                move.SetVelocityY(moveAmount * -1);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                move.SetVelocityX(moveAmount * -1);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                move.SetVelocityX(moveAmount * 1);
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                bird.Jump();
            }
        }
    }
}
