using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    [Header("Digit Sprites (index = digit)")]
    [Tooltip("长度至少 10：sprites[0]..sprites[9]")]
    [SerializeField] private List<Sprite> digitSprites = new List<Sprite>(10);

    [Header("Digit Images (exactly 5)")]
    [Tooltip("5个Image，从左到右。也可留空，勾选 Auto Build 自动生成。")]
    [SerializeField] private List<Image> digitImages = new List<Image>(5);

    [Header("Build (optional)")]
    [SerializeField] private bool autoBuildIfEmpty = true;
    [SerializeField] private Vector2 digitSize = new Vector2(32, 48);
    [SerializeField] private float spacing = 0f;

    [Header("Layout")]
    [SerializeField] private bool centerAlign = true;

    [Header("Format")]
    [SerializeField] private bool padWithZeros = true;

    private void Reset()
    {
        TryAutoBuild();
        ApplyLayout();
    }

    private void Awake()
    {
        TryAutoBuild();
        ApplyLayout();
        ValidateSetupOrLog();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
            TryAutoBuild();

        ApplyLayout();
        ValidateSetupOrLog();
    }

    private void ApplyLayout()
    {
        var layout = GetComponent<HorizontalLayoutGroup>();
        if (!layout) return;

        layout.childAlignment = centerAlign ? TextAnchor.MiddleCenter : TextAnchor.MiddleLeft;
        layout.spacing = spacing;

        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
    }

    private void TryAutoBuild()
    {
        if (!autoBuildIfEmpty) return;
        if (digitImages != null && digitImages.Count == 5 && digitImages[0] != null) return;

        var layout = GetComponent<HorizontalLayoutGroup>();
        if (!layout) layout = gameObject.AddComponent<HorizontalLayoutGroup>();

        var fitter = GetComponent<ContentSizeFitter>();
        if (!fitter) fitter = gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        digitImages = new List<Image>(5);
        for (int i = 0; i < 5; i++)
        {
            var go = new GameObject($"Digit_{i}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(transform, false);

            var rt = (RectTransform)go.transform;
            rt.sizeDelta = digitSize;

            var img = go.GetComponent<Image>();
            img.raycastTarget = false;
            img.preserveAspect = true;

            digitImages.Add(img);
        }
    }

    private bool ValidateSetupOrLog()
    {
        if (digitSprites == null || digitSprites.Count < 10)
        {
            Debug.LogError($"{nameof(ScoreController)}: digitSprites 需要至少 10 个（0~9）。", this);
            return false;
        }
        if (digitImages == null || digitImages.Count < 5)
        {
            Debug.LogError($"{nameof(ScoreController)}: digitImages 需要 5 个 Image。", this);
            return false;
        }
        return true;
    }

    public void SetValue(int value)
    {
        if (!ValidateSetupOrLog()) return;

        if (value < 0) value = 0;
        value %= 100000;

        // digitsArr[0] 是万位，digitsArr[4] 是个位
        int[] digitsArr = new int[5];
        int tmp = value;
        for (int i = 4; i >= 0; i--)
        {
            digitsArr[i] = tmp % 10;
            tmp /= 10;
        }

        int firstToShow = 0;

        if (!padWithZeros)
        {
            // 找到第一位非0；如果全是0，则只显示个位
            firstToShow = 4; // 默认只显示个位（确保 0 会显示）
            for (int i = 0; i < 5; i++)
            {
                if (digitsArr[i] != 0)
                {
                    firstToShow = i;
                    break;
                }
            }
        }

        for (int i = 0; i < 5; i++)
        {
            var img = digitImages[i];
            if (!img) continue;

            bool shouldShow = padWithZeros || i >= firstToShow;

            if (!shouldShow)
            {
                img.transform.gameObject.SetActive(!centerAlign);                    
                img.enabled = false;
                img.sprite = null;
            }
            else
            {
                img.transform.gameObject.SetActive(true);
                img.enabled = true;
                img.sprite = digitSprites[digitsArr[i]];
            }
        }
    }
}
