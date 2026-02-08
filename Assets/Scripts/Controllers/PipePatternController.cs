using System;
using UnityEngine;

class PipePatternController : MonoBehaviour
{
    [SerializeField]
    private float speedPerFixedFrame = 0.020f;  // 每固定帧移动距离

    private int instanceCount = 0;
    private int passCount = 0;
    private int passSpawnLineCount = 0;
    private bool canClean = false;
    private bool passSpawnLine = false;
    private bool canCheckClean = false;

    private Transform cacheTransform;
    private PipeData[] pipes;
    private PipeData lastPipe;

    void Start()
    {
        cacheTransform = transform;
        pipes = gameObject.GetComponentsInChildren<PipeData>();
        instanceCount = pipes.Length;
        passCount = 0;
        lastPipe = pipes[^1];
    }

    private void FixedUpdate()
    {
        if (canClean) return;
        if (GameManager.GetCurrentGameState() == GameStateType.Result) return;

        cacheTransform.position += Vector3.left * speedPerFixedFrame;

        foreach (var pipe in pipes)
        {
            if (!pipe.Passed)
            {
                pipe.SendMessage("OnCheckPass");
            }
            if (!pipe.Final)
            {
                pipe.SendMessage("OnCheckClean");
            }
            if (!pipe.PassSpawnLine)
            {
                pipe.SendMessage("OnCheckPassSpawnLine");
            }
        }

        if (canCheckClean)
        {
            if (lastPipe.transform.position.x <= -2f)
            {
                canClean = true;
            }
        }
    }

    void ApplyPipePassReward()
    {
        GameManager.IncreaseCurrentScore(1);
    }

    private void OnPipePass()
    {
        passCount += 1;
        ApplyPipePassReward();
    }

    private void OnPipeFinal()
    {
        if (instanceCount == passCount)
        {
            canCheckClean = true;
        }
    }

    private void OnPipePassSpawnLine()
    {
        passSpawnLineCount += 1;
        if (instanceCount == passSpawnLineCount)
        {
            passSpawnLine = true;
        }
    }

    public bool CheckCanClean()
    {
        return canClean;
    }

    public bool CheckPassSpawnLine()
    {
        return passSpawnLine;
    }
}