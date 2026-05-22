using System.Collections;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public Animator hitAnimator;
    public SpriteRenderer hitSpriteRenderer;

    private HitDetector hitDetector;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        hitDetector = GetComponentInChildren<HitDetector>(true);

        if (hitDetector != null)
        {
            hitDetector.ownerName = "Player";
        }
    }

    private void Start()
    {
        GameInput.Instance.OnAttack += GameInputOnAttack;
    }

    private void OnDestroy()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnAttack -= GameInputOnAttack;
        }
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

        if(shouldFlip)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        bool lookingUp = mousePos.y > playerPos.y;
        animator.SetBool("IsLookingUp", lookingUp);
    }
}
