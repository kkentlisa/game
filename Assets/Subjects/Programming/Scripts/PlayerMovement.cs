using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        
        var noFriction = new PhysicsMaterial2D { friction = 0f, bounciness = 0f };
        var col = GetComponent<Collider2D>();
        if (col != null) col.sharedMaterial = noFriction;
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * speed;
    }
}
