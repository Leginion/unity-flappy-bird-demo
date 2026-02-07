using UnityEngine;

public class BirdController : MonoBehaviour
{
    [Header("跳跃冲力")]
    [SerializeField]
    private float jumpForce = 5f;

    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private BirdFacingController facing;

    private Vector3 v3_initial;

    private void Awake()
    {
        v3_initial = transform.localPosition;
    }

    public void ResetBirdPosition()
    {
        transform.localPosition = v3_initial;
    }

    public void Jump()
    {
        // apply velocity
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        // apply facing
        facing.SetUpwardRotation();

        // play sound
        int i = Random.Range(1, 2);
        string sfx_name = $"bird/wing-{i}";
        AudioManager.PlaySFX(sfx_name);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Collide"))
        {
            GameManager.Failed();

            // play sound
            int i = Random.Range(1, 2);
            string sfx_name = $"bird/die-{i}";
            AudioManager.PlaySFXFrom(sfx_name, 300f);
        }
    }
}
