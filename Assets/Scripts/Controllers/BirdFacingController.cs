using UnityEngine;

/// <summary>
/// 控制小鸟根据速度自动调整朝向角度
/// </summary>
public class BirdFacingController : MonoBehaviour
{
    [Header("角度设置")]
    [SerializeField]
    [Tooltip("向上飞行时的最大角度")]
    private float maxUpAngle = 30f;

    [SerializeField]
    [Tooltip("向下坠落时的最大角度")]
    private float maxDownAngle = -90f;

    [Header("旋转控制")]
    [SerializeField]
    [Tooltip("旋转平滑速度")]
    private float rotationSpeed = 3f;

    [SerializeField]
    [Tooltip("速度映射到角度的敏感度")]
    private float velocitySensitivity = 10f;

    [Header("引用")]
    [SerializeField]
    private Rigidbody2D rb;

    private void Awake()
    {
        // 自动获取 Rigidbody2D
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        UpdateRotation();
    }

    /// <summary>
    /// 根据速度更新旋转角度
    /// </summary>
    private void UpdateRotation()
    {
        float targetAngle = CalculateTargetAngle();
        ApplyRotation(targetAngle);
    }

    /// <summary>
    /// 计算目标角度
    /// </summary>
    private float CalculateTargetAngle()
    {
        float velocityY = rb.velocity.y;

        if (velocityY > 0)
        {
            // 向上飞行：直接使用最大向上角度
            return maxUpAngle;
        }
        else
        {
            // 向下坠落：根据速度线性插值
            float t = Mathf.Clamp01((-velocityY) / velocitySensitivity);
            return Mathf.Lerp(maxUpAngle, maxDownAngle, t);
        }
    }

    /// <summary>
    /// 应用旋转（平滑过渡）
    /// </summary>
    private void ApplyRotation(float targetAngle)
    {
        // 获取当前角度（转换到 -180 到 180 范围）
        float currentAngle = transform.eulerAngles.z;
        if (currentAngle > 180)
            currentAngle -= 360;

        // 平滑插值到目标角度
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);

        // 应用旋转
        transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    /// <summary>
    /// 立即设置为向上角度（可在跳跃时调用）
    /// </summary>
    public void SetUpwardRotation()
    {
        transform.rotation = Quaternion.Euler(0, 0, maxUpAngle);
    }

    /// <summary>
    /// 重置角度为0
    /// </summary>
    public void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 确保角度范围合理
        maxUpAngle = Mathf.Clamp(maxUpAngle, 0, 90);
        maxDownAngle = Mathf.Clamp(maxDownAngle, -90, 0);
        rotationSpeed = Mathf.Max(0, rotationSpeed);
        velocitySensitivity = Mathf.Max(0.1f, velocitySensitivity);
    }
#endif
}
