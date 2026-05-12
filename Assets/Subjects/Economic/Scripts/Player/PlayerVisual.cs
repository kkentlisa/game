using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public Animator hitAnimator;
    public SpriteRenderer hitSpriteRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GameInput.Instance.OnAttack += GameInputOnAttack;
    }

    private void GameInputOnAttack()
    {
        hitAnimator.SetTrigger("Attack");
    }

    private void Update()
    {
        animator.SetBool("IsRunning", Player.Instance.IsRunning());
        HandleVisuals();
    }

    private void HandleVisuals()
    {
        Vector3 mousePos = GameInput.Instance.GetMousePosition();
        Vector3 playerPos = Player.Instance.GetPlayerPosition();

        bool shouldFlip = mousePos.x < playerPos.x;
        spriteRenderer.flipX = shouldFlip;

        if (hitSpriteRenderer != null)
        {
            hitSpriteRenderer.flipX = shouldFlip;
        }

        bool lookingUp = mousePos.y > playerPos.y;
        animator.SetBool("IsLookingUp", lookingUp);
    }
}
