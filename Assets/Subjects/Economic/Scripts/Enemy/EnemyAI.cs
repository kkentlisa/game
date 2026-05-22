using UnityEngine;
using UnityEngine.AI;
using GameUtils;
using System;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private State startingState;
    [SerializeField] private float roamingDistanceMax = 7f;
    [SerializeField] private float roamingDistanceMin = 2f;
    [SerializeField] private float roamingTimerMax = 2f;

    [SerializeField] private float chasingDistance = 10f;
    [SerializeField] private float chasingSpeedMultiplier = 1.5f;

    [SerializeField] private float attackingDistance = 2f;
    [SerializeField] private float attackRate = 2f;
    private float nextAttackTime = 0f;

    private NavMeshAgent navMeshAgent;
    private State currentState;
    private float roamingTimer;
    private Vector3 roamPosition;
    private Vector3 startingPosition;

    private float roamingSpeed;
    private float chasingSpeed;

    private float nextCheckDirectionTime = 0f;
    private float checkDirectionDuration = 0.1f;
    private Vector3 lastPosition;

    public event EventHandler OnEnemyAttack;

    public bool IsRunning
    {
        get
        {
            if (navMeshAgent.velocity == Vector3.zero)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [SerializeField] private string enemyName = "Student";
    public string EnemyName => enemyName;

    [SerializeField] private float furnitureAttackRate = 0.8f;
    private float nextFurnitureAttackTime = 0f;

    [SerializeField] private float furnitureAttackingDistance = 1f;

    private FurnitureItem targetFurniture = null;
    private CoinsPickup targetCoin = null;

    private Vector3 stuckCheckPosition;
    private float stuckCheckTimer = 0f;
    private float stuckCheckInterval = 1f;
    private float stuckThreshold = 0.2f;

    private enum State
    {
        Idle,
        Roaming,
        Chasing,
        Attacking,
        MovingToFurniture,
        BreakingFurniture,
        CollectingCoin
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        currentState = startingState;

        roamingSpeed = navMeshAgent.speed;
        chasingSpeed = navMeshAgent.speed * chasingSpeedMultiplier;

        HitDetector hitDetector = GetComponentInChildren<HitDetector>(true);
        if (hitDetector != null) hitDetector.ownerName = enemyName;
    }

    private void Start()
    {
        ScoreManager.Instance.AddScore(enemyName, 0);
    }

    private void Update()
    {
        StateHandler();
        MovementDirectionHandler();
        StuckHandler();
    }

    private void StateHandler()
    {
        switch (currentState)
        {
            case State.Roaming:
                roamingTimer -= Time.deltaTime;
                if (roamingTimer < 0)
                {
                    Roaming();
                    roamingTimer = roamingTimerMax;
                }
                CheckCurrentState();
                break;
            case State.Chasing:
                ChasingTarget();
                CheckCurrentState();
                break;
            case State.Attacking:
                AttackingTarget();
                CheckCurrentState();
                break;
            case State.MovingToFurniture:
                MovingToFurniture();
                CheckCurrentState();
                break;
            case State.BreakingFurniture:
                BreakingFurniture();
                CheckCurrentState();
                break;
            case State.CollectingCoin:
                CollectingCoin();
                CheckCurrentState();
                break;


            default:
            case State.Idle:
                break;
        }
    }

    private void CheckCurrentState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);

        bool shouldTargetPlayer = distanceToPlayer <= chasingDistance && PlayerIsRicher();

        if (shouldTargetPlayer)
        {
            State newState = State.Chasing;

            if (distanceToPlayer <= attackingDistance)
            {
                newState = State.Attacking;
            }

            if (newState != currentState)
            {
                if (newState == State.Chasing)
                {
                    navMeshAgent.ResetPath();
                    navMeshAgent.speed = chasingSpeed;

                }
                else if (newState == State.Attacking)
                {
                    navMeshAgent.ResetPath();
                }
                currentState = newState;
            }
            return;
        }

        if (currentState == State.Chasing || currentState == State.Attacking)
        {
            EnterRoaming();
            return;
        }

        if (currentState == State.Roaming)
        {
            CoinsPickup coin = FindNearestCoin();
            if (coin != null)
            {
                targetCoin = coin;
                currentState = State.CollectingCoin;
                return;
            }

            FurnitureItem furniture = FindNearestFurniture();
            if (furniture != null)
            {
                targetFurniture = furniture;
                navMeshAgent.speed = roamingSpeed;
                currentState = State.MovingToFurniture;
            }
        }
    }

    private void EnterRoaming()
    {
        navMeshAgent.speed = roamingSpeed;
        roamingTimer = 0f;
        currentState = State.Roaming;
    }

    private bool PlayerIsRicher()
    {
        return ScoreManager.Instance.GetScore("Player") > ScoreManager.Instance.GetScore(enemyName);
    }

    private CoinsPickup FindNearestCoin()
    {
        CoinsPickup nearest = null;
        float nearestDist = float.MaxValue;
        float searchRadius = chasingDistance;

        foreach (CoinsPickup coin in FindObjectsByType<CoinsPickup>(FindObjectsSortMode.None))
        {
            if (coin == null) continue;
            float dist = Vector3.Distance(transform.position, coin.transform.position);
            if (dist < searchRadius && dist < nearestDist)
            {
                nearestDist = dist;
                nearest = coin;
            }
        }

        return nearest;
    }

    private FurnitureItem FindNearestFurniture()
    {
        FurnitureItem nearest = null;
        float nearestDist = float.MaxValue;

        foreach (FurnitureItem item in FindObjectsByType<FurnitureItem>(FindObjectsSortMode.None))
        {
            if (item == null || item.isBroken || !item.canBeDestroyed) continue;

            float dist = Vector3.Distance(transform.position, item.transform.position);

            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = item;
            }
        }
        return nearest;
    }

    private void AttackingTarget()
    {
        if (Time.time > nextAttackTime)
        {
            OnEnemyAttack?.Invoke(this, EventArgs.Empty);

            nextAttackTime = Time.time + attackRate;
        }
    }

    private void MovementDirectionHandler()
    {
        if (Time.time > nextCheckDirectionTime)
        {
            if (IsRunning)
            {
                ChangeFacingDirection(lastPosition, transform.position);
            }
            else if (currentState == State.Attacking)
            {
                ChangeFacingDirection(transform.position, Player.Instance.transform.position);
            }
            else if (currentState == State.BreakingFurniture && targetFurniture != null)
            {
                ChangeFacingDirection(transform.position, targetFurniture.transform.position);
            }
            lastPosition = transform.position;
            nextCheckDirectionTime = Time.time + checkDirectionDuration;
        }
    }

    private void ChasingTarget()
    {
        navMeshAgent.SetDestination(Player.Instance.transform.position);
    }

    public float GetRoamingAnimationSpeed()
    {
        return navMeshAgent.speed / roamingSpeed;
    }

    private void Roaming()
    {
        startingPosition = transform.position;
        roamPosition = GetRandomPosition();
        navMeshAgent.SetDestination(roamPosition);
    }

    private Vector3 GetRandomPosition()
    {
        return startingPosition + Utils.GetRandomDir() * UnityEngine.Random.Range(roamingDistanceMin, roamingDistanceMax);
    }

    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        if (sourcePosition.x > targetPosition.x)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void MovingToFurniture()
    {
        if (targetFurniture == null || targetFurniture.isBroken)
        {
            targetFurniture = null;
            EnterRoaming();
            return;
        }

        navMeshAgent.SetDestination(targetFurniture.transform.position);

        if (Vector3.Distance(transform.position, targetFurniture.transform.position) <= furnitureAttackingDistance)
        {
            navMeshAgent.ResetPath();
            currentState = State.BreakingFurniture;
        }
    }

    private void BreakingFurniture()
    {
        if (targetFurniture == null || targetFurniture.isBroken)
        {
            targetFurniture = null;
            EnterRoaming();
            return;
        }

        if (Time.time >= nextFurnitureAttackTime)
        {
            OnEnemyAttack?.Invoke(this, EventArgs.Empty);
            nextFurnitureAttackTime = Time.time + furnitureAttackRate;
        }
    }

    private void CollectingCoin()
    {
        if (targetCoin == null)
        {
            EnterRoaming();
            return;
        }

        navMeshAgent.SetDestination(targetCoin.transform.position);
    }

    public void OnCoinCollected()
    {
        targetCoin = null;
        EnterRoaming();
    }

    public void OnHitReceived()
    {
        targetFurniture = null;
        targetCoin = null;
        EnterRoaming();
    }

    private void StuckHandler()
    {
        if (!IsRunning) return;
        if (currentState == State.Attacking || currentState == State.BreakingFurniture) return;

        stuckCheckTimer -= Time.deltaTime;
        if (stuckCheckTimer > 0f) return;

        stuckCheckTimer = stuckCheckInterval;

        float movedDistance = Vector3.Distance(transform.position, stuckCheckPosition);

        if (movedDistance < stuckThreshold)
        {
            targetFurniture = null;
            targetCoin = null;
            navMeshAgent.ResetPath();
            EnterRoaming();
        }
        stuckCheckPosition = transform.position;
    }
}