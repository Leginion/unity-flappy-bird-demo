using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    public void ResetVelocity() {
        rb.velocity = Vector2.zero;
    }
}
