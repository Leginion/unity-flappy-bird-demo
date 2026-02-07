using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    [SerializeField]
    private Vector2 velocity = Vector2.zero;

    public void SetVelocityX(float amount)
    {
        velocity.x = amount;
    }
    public void SetVelocityY(float amount)
    {
        velocity.y = amount;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * (velocity.y * Time.deltaTime));
        transform.Translate(Vector2.right * (velocity.x * Time.deltaTime));
    }
}
