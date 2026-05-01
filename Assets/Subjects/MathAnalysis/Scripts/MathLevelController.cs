using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class MathLevelController : MonoBehaviour
{
    public enum Difficulty { Grade3, Grade4, Grade5 }

    [Header("Текущая пересдача")]
    public Difficulty currentDifficulty;

    [Header("Ссылки на объекты")]
    public CameraMovement cameraScript;
    public SpriteRenderer bgRenderer;

    [Header("Настройки виньеток")]
    public SpriteRenderer[] vignetteSprites;

    void Start()
    {
        ApplyDifficultySettings();
    }

    void ApplyDifficultySettings()
    {
        StopAllCoroutines();

        if (bgRenderer != null) bgRenderer.color = Color.white;

        switch (currentDifficulty)
        {
            case Difficulty.Grade3:
                SetupGrade3();
                break;
            case Difficulty.Grade4:
                SetupGrade4();
                break;
            case Difficulty.Grade5:
                SetupGrade5();
                break;
        }
    }

    private void SetVignettesAlpha(float alpha)
    {
        foreach (SpriteRenderer sprite in vignetteSprites)
        {
            if (sprite != null)
            {
                sprite.color = new Color(1f, 1f, 1f, alpha);
            }
        }
    }

    private void SetupGrade3()
    {
        if (cameraScript != null) cameraScript.shakeIntensity = 0f;
        SetVignettesAlpha(0f);
    }

    private void SetupGrade4()
    {
        if (cameraScript != null) cameraScript.shakeIntensity = 0.5f;
        SetVignettesAlpha(0.7f);
    }

    private void SetupGrade5()
    {
        if (cameraScript != null) cameraScript.shakeIntensity = 1.2f;
        SetVignettesAlpha(0.9f);

        StartCoroutine(FlickerVignette());
    }

    IEnumerator FlickerVignette()
    {
        while (currentDifficulty == Difficulty.Grade5)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f));

            if (Random.value > 0.5f)
            {
                SetVignettesAlpha(1f); 
                yield return new WaitForSeconds(0.1f);
                SetVignettesAlpha(0.9f); 
            }
        }
    }
}