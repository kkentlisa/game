using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomWanderingAI : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 2f;
    public float pauseTime = 1.0f;

    [Header("Настройки Перспективы (Размер от Y)")]
    public float minYPosition = -5f;
    public float maxYPosition = 0f;
    public float minScale = 0.2f;
    public float maxScale = 0.65f;

    private ClassroomNetwork roadNetwork;
    private int currentNodeIndex = 0;
    private int targetNodeIndex = -1;
    private Transform activeNoteTransform = null;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }

        roadNetwork = Object.FindFirstObjectByType<ClassroomNetwork>();

        if (roadNetwork != null && roadNetwork.allNodes.Count > 0)
        {
            currentNodeIndex = roadNetwork.GetClosestNodeIndex(transform.position);
            transform.position = roadNetwork.allNodes[currentNodeIndex].position;
            StartCoroutine(NavigateNetworkRoutine());
        }
        else
        {
            Debug.LogError("ИИ Учителя: Не найдена ClassroomNetwork на сцене или в ней нет точек!");
        }
    }

    public void SetTargetNote(Transform noteTransform)
    {
        activeNoteTransform = noteTransform;
    }

    public void ClearTargetNote()
    {
        activeNoteTransform = null;
    }

    void Update()
    {
        float t = Mathf.InverseLerp(maxYPosition, minYPosition, transform.position.y);
        float currentScale = Mathf.Lerp(minScale, maxScale, t);

        float directionSign = transform.localScale.x > 0 ? 1f : -1f;
        if (isMoving && targetNodeIndex != -1)
        {
            float deltaX = roadNetwork.allNodes[targetNodeIndex].position.x - transform.position.x;
            if (deltaX > 0.1f) directionSign = 1f;
            else if (deltaX < -0.1f) directionSign = -1f;
        }
        transform.localScale = new Vector3(currentScale * directionSign, currentScale, 1f);

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100f);
        }

        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }
    }

    IEnumerator NavigateNetworkRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            if (roadNetwork == null || roadNetwork.allNodes.Count == 0) yield return null;

            targetNodeIndex = ChooseNextNode();

            if (targetNodeIndex != -1)
            {
                isMoving = true;
                Vector2 targetPos = roadNetwork.allNodes[targetNodeIndex].position;

                while (Vector2.Distance(rb.position, targetPos) > 0.05f)
                {
                    Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, moveSpeed * Time.deltaTime);
                    rb.MovePosition(newPos);
                    yield return null;
                }

                rb.MovePosition(targetPos);
                currentNodeIndex = targetNodeIndex;
                isMoving = false;

                yield return new WaitForSeconds(pauseTime);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    int ChooseNextNode()
    {
        var currentNode = roadNetwork.allNodes[currentNodeIndex];
        if (currentNode.connectedNodeIndices.Count == 0) return -1;

        if (activeNoteTransform != null)
        {
            int closestNodeToNote = roadNetwork.GetClosestNodeIndex(activeNoteTransform.position);

            if (currentNodeIndex == closestNodeToNote)
            {
                return currentNode.connectedNodeIndices[Random.Range(0, currentNode.connectedNodeIndices.Count)];
            }

            int bestNodeIndex = currentNode.connectedNodeIndices[0];
            float minTargetDist = float.MaxValue;

            foreach (int neighborIndex in currentNode.connectedNodeIndices)
            {
                float distToTarget = Vector2.Distance(roadNetwork.allNodes[neighborIndex].position, roadNetwork.allNodes[closestNodeToNote].position);
                if (distToTarget < minTargetDist)
                {
                    minTargetDist = distToTarget;
                    bestNodeIndex = neighborIndex;
                }
            }
            return bestNodeIndex;
        }

        return currentNode.connectedNodeIndices[Random.Range(0, currentNode.connectedNodeIndices.Count)];
    }
}