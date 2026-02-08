using System.Collections.Generic;
using UnityEngine;

public class PipeSystem : MonoBehaviour
{
    List<PipePatternController> patternInstances = new();

    [SerializeField]
    GameObject pipePrefab;

    [System.Serializable]
    public class PipePatternPrefabPart
    {
        public int level;
        public List<GameObject> prefabs;
    }

    [SerializeField]
    PipePatternPrefabPart[] pipePatternPrefabs;
    Dictionary<int, List<GameObject>> pipePatternPrefabsMap = new();

    [SerializeField]
    BirdController bird;
    Transform cacheTransform;

    void Start()
    {
        cacheTransform = transform;

        // BuildPipePatternPrefabsMap
        for (int i = 0; i < pipePatternPrefabs.Length; i++)
        {
            var info = pipePatternPrefabs[i];
            pipePatternPrefabsMap[info.level] = info.prefabs;
        }
    }

    GameObject PickPipePettern(int level)
    {
        var list = pipePatternPrefabsMap[level];
        int i = Random.Range(0, list.Count);
        return list[i];
    }

    void FixedUpdate()
    {
        if (GameManager.GetPaused())
        {
            return;
        }

        // 刷新逻辑
        if (patternInstances.Count == 0 || patternInstances[^1].CheckPassSpawnLine())
        {
            RequestSpawn();
        }
    }

    private void LateUpdate()
    {
        // 清理逻辑
        for (int i = patternInstances.Count - 1; i >= 0; i--)
        {
            Debug.Log($"patternInstances.Count: {patternInstances.Count} -> i: ${i}");
            PipePatternController ppc = patternInstances[i];

            if (ppc.CheckCanClean())
            {
                Destroy(ppc.gameObject);
                patternInstances.RemoveAt(i);
            }
        }
    }

    void ApplyPipeGap(Transform t, float gap)
    {
        float gapH = gap / 2f;

        Transform t1 = t.Find("pipe-top");
        Transform t2 = t.Find("pipe-bottom");

        Vector3 v1 = t1.localPosition;
        Vector3 v2 = t2.localPosition;

        v1.y += gapH;
        v2.y -= gapH;

        t1.localPosition = v1;
        t2.localPosition = v2;
    }

    void SpawnPipePattern(GameObject prefab)
    {
        GameObject pipePatternRoot = Instantiate(prefab);
        Transform rootTransform = pipePatternRoot.transform;
        PipePatternController ppc = pipePatternRoot.GetComponentInChildren<PipePatternController>();
        patternInstances.Add(ppc);
        if (ppc == null)
        {
            Debug.LogError("Null PipePatternController");
            return;
        }

        PipeData[] pipeDatas = pipePatternRoot.GetComponentsInChildren<PipeData>();
        foreach (var pipeData in pipeDatas)
        {
            ApplyPipeGap(pipeData.transform, pipeData.gap);
        }

        rootTransform.SetParent(cacheTransform, false);
        rootTransform.localPosition = new Vector3(2f, Random.Range(-0.4f, 0.4f), 0f);
    }

    void RequestSpawn()
    {
        int level = GameManager.GetCurrentLevel();
        GameObject pipePatternPrefab = PickPipePettern(level);
        SpawnPipePattern(pipePatternPrefab);
    }

    public void CleanAll()
    {
        foreach (var pipe in patternInstances)
        {
            Destroy(pipe.gameObject);
        }

        patternInstances.Clear();
    }
}
