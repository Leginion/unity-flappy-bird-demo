using UnityEngine;

public class PipeData : MonoBehaviour
{
    public float gap = 1.00f;
    public bool Passed = false;
    public bool Final = false;
    public bool PassSpawnLine = false;
    Transform cacheTransform;

    private void Start()
    {
        cacheTransform = transform;
    }

    private void Update()
    {
        if (GameManager.GetCurrentGameState() == GameStateType.Result)
        {
            var cs = GetComponentsInChildren<BoxCollider2D>();
            foreach (var c in cs)
            {
                c.enabled = false;
            }
        }
    }

    void OnCheckPass()
    {
        float x = cacheTransform.position.x;

        if (!Passed && x <= GameManager.GetTreshold("pass"))
        {
            OnPass();
        }
    }

    void OnCheckClean()
    {
        float x = cacheTransform.position.x;

        if (!Final && x <= GameManager.GetTreshold("clean"))
        {
            OnClean();
        }
    }

    void OnCheckPassSpawnLine()
    {
        float x = cacheTransform.position.x;

        if (!PassSpawnLine && x <= GameManager.GetTreshold("spawn-line"))
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
