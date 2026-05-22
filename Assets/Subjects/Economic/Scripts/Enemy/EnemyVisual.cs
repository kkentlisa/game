using UnityEngine;

public class EnemyVisual : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;

    [SerializeField] private Animator hitAnimator;

    private Animator visualAnimator;



    private void Awake()
    {
        visualAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (enemyAI != null) enemyAI.OnEnemyAttack += HandleAttack;
    }

    private void OnDisable()
    {
        if (enemyAI != null) enemyAI.OnEnemyAttack -= HandleAttack;
    }

    private void Update()
    {
        visualAnimator.SetBool("IsRunning", enemyAI.IsRunning);
        visualAnimator.SetFloat("ChasingSpeedMultiplier", enemyAI.GetRoamingAnimationSpeed());
    }

    private void HandleAttack(object sender, System.EventArgs e)
    {
        hitAnimator?.SetTrigger("Attack");
    }
}
