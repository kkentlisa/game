using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("Инвентарь")]
    public GameObject candyPrefab; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DropCandy();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * speed;
    }

    void DropCandy()
    {
        GameObject candy = Instantiate(candyPrefab, transform.position, Quaternion.identity);
        
        AssistantFollow assistant = FindObjectOfType<AssistantFollow>();
        if (assistant != null)
        {
            assistant.DistractWithCandy(candy);
        }
    }
}