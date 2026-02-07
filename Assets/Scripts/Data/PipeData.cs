using UnityEngine;

public class PipeData : MonoBehaviour
{
    public float gap = 1.00f;
    public bool Passed = false;
    public bool Final = false;
    public bool PassSpawnLine = false;
    Transform cacheTransform;
    private float tresholdPass = -0.04f;
    private float tresholdClean = -2f;
    private float tresholdSpawnLine = -0.04f;

    private void Start()
    {
        cacheTransform = transform;
    }

    void OnCheckPass()
    {
        float x = cacheTransform.position.x;

        if (!Passed && x <= tresholdPass)
        {
            OnPass();
        }
    }

    void OnCheckClean()
    {
        float x = cacheTransform.position.x;

        if (!Final && x <= tresholdClean)
        {
            OnClean();
        }
    }

    void OnCheckPassSpawnLine()
    {
        float x = cacheTransform.position.x;

        if (!PassSpawnLine && x <= tresholdSpawnLine)
        {
            OnPassSpawnLine();
        }
    }

    void OnPass()
    {
        Passed = true;
        SendMessageUpwards("OnPipePass");
    }

    void OnClean()
    {
        Final = true;
        gameObject.SetActive(false);
        SendMessageUpwards("OnPipeFinal");
    }

    void OnPassSpawnLine()
    {
        PassSpawnLine = true;
        SendMessageUpwards("OnPipePassSpawnLine");
    }
}
