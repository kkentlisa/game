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

        // Автоматически находим карту дорог на сцене
        roadNetwork = Object.FindFirstObjectByType<ClassroomNetwork>();

        if (roadNetwork != null && roadNetwork.allNodes.Count > 0)
        {
            // Телепортируем Учителя на ближайшую стартовую точку сети
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
        // Псевдо-3D эффект: меняем размер учителя в зависимости от координаты Y
        float t = Mathf.InverseLerp(maxYPosition, minYPosition, transform.position.y);
        float currentScale = Mathf.Lerp(minScale, maxScale, t);

        // Определяем направление движения по X и разворачиваем спрайт в нужную сторону
        float directionSign = transform.localScale.x > 0 ? 1f : -1f;
        if (isMoving && targetNodeIndex != -1)
        {
            float deltaX = roadNetwork.allNodes[targetNodeIndex].position.x - transform.position.x;
            if (deltaX > 0.1f) directionSign = 1f;
            else if (deltaX < -0.1f) directionSign = -1f;
        }
        transform.localScale = new Vector3(currentScale * directionSign, currentScale, 1f);

        // Устанавливаем сортировку спрайта в зависимости от Y позиции для правильного наложения
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100f);
        }

        // Управляем анимацией движения
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }
    }

    // Основной корутин для навигации по сети дорог, выбора следующей точки и управления движением
    IEnumerator NavigateNetworkRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            if (roadNetwork == null || roadNetwork.allNodes.Count == 0) yield return null;

            // Выбираем следующую точку для шага
            targetNodeIndex = ChooseNextNode();

            if (targetNodeIndex != -1)
            {
                isMoving = true;
                Vector2 targetPos = roadNetwork.allNodes[targetNodeIndex].position;

                // Плавно идем к точке
                while (Vector2.Distance(rb.position, targetPos) > 0.05f)
                {
                    Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, moveSpeed * Time.deltaTime);
                    rb.MovePosition(newPos);
                    yield return null;
                }

                rb.MovePosition(targetPos);
                currentNodeIndex = targetNodeIndex;
                isMoving = false;

                // Стоим на точке отдыхаем
                yield return new WaitForSeconds(pauseTime);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    // Метод для выбора следующей точки на основе наличия активной записки и расстояния до нее
    int ChooseNextNode()
    {
        var currentNode = roadNetwork.allNodes[currentNodeIndex];
        if (currentNode.connectedNodeIndices.Count == 0) return -1;

        // ЕСЛИ ЕСТЬ ЗАПИСКА: Ищем точку, которая ближе всего к записке
        if (activeNoteTransform != null)
        {
            int closestNodeToNote = roadNetwork.GetClosestNodeIndex(activeNoteTransform.position);

            // Если мы уже стоим в ближайшей к записке точке, покружимся по ее соседям
            if (currentNodeIndex == closestNodeToNote)
            {
                return currentNode.connectedNodeIndices[Random.Range(0, currentNode.connectedNodeIndices.Count)];
            }

            // Иначе выбираем из доступных путей тот узел, который сократит расстояние до записки
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

        // ЕСЛИ ЗАПИСКИ НЕТ: Просто идем на случайного соседа
        return currentNode.connectedNodeIndices[Random.Range(0, currentNode.connectedNodeIndices.Count)];
    }
}