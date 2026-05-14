using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SleepMechanicController : MonoBehaviour
{
    [Header("Настройки визуализации")]
    public SpriteRenderer[] blackScreens;
    public SpriteRenderer warningSignRenderer;

    [Header("Параметры сна")]
    [Range(0, 1)] public float sleepiness = 0f;
    public float fallAsleepSpeed = 0.1f;
    public float wakeUpForce = 0.25f;

    [Header("Эффекты пробуждения и камеры")]
    public CameraMovement cameraScript;
    public float effectDuration = 2f;
    private bool isHandlingState = true;

    private Vector3 initialSignScale;

    void Start()
    {
        if (warningSignRenderer != null)
        {
            initialSignScale = warningSignRenderer.transform.localScale;
            warningSignRenderer.color = new Color(1f, 1f, 1f, 0f);
        }

        sleepiness = 1f;
        UpdateVignetteAlpha(1f);
        StartCoroutine(WakeUpRoutine());
    }

    IEnumerator WakeUpRoutine()
    {
        isHandlingState = true;
        float timer = 0;

        while (timer < effectDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / effectDuration;

            sleepiness = Mathf.Lerp(1f, 0f, progress);
            UpdateVignetteAlpha(sleepiness);

            if (cameraScript != null)
            {
                float currentStrength = Mathf.Lerp(0.01f, 0f, progress);
                cameraScript.swayAmountX = Mathf.Sin(timer * 3f) * currentStrength;
                cameraScript.swayAmountY = Mathf.Cos(timer * 2.5f) * (currentStrength * 0.6f);
            }
            yield return null;
        }

        isHandlingState = false;
        if (cameraScript != null)
        {
            cameraScript.swayAmountX = 0f;
            cameraScript.swayAmountY = 0f;
        }

        MathLevelController mlc = Object.FindFirstObjectByType<MathLevelController>();
        if (mlc != null) mlc.ApplyDifficultySettings();
    }

    public void SetSleepSpeed(float newSpeed)
    {
        fallAsleepSpeed = newSpeed;
    }

    void Update()
    {
        if (isHandlingState) return;

        sleepiness += fallAsleepSpeed * Time.deltaTime;
        sleepiness = Mathf.Clamp01(sleepiness);
        UpdateVignetteAlpha(sleepiness);

        if (warningSignRenderer != null)
        {
            float signAlpha = Mathf.InverseLerp(0.1f, 0.6f, sleepiness);
            warningSignRenderer.color = new Color(1f, 1f, 1f, signAlpha);

            if (signAlpha > 0.01f)
            {
                float pulse = 1f + Mathf.PingPong(Time.time * 0.2f, 0.04f);
                warningSignRenderer.transform.localScale = initialSignScale * pulse;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            sleepiness -= wakeUpForce;
            if (sleepiness < 0) sleepiness = 0;
        }

        if (sleepiness >= 0.8f)
        {
            HandleFallAsleep();
        }
    }

    void UpdateVignetteAlpha(float alpha)
    {
        foreach (var sprite in blackScreens)
            if (sprite != null) sprite.color = new Color(0, 0, 0, alpha);
    }

    void HandleFallAsleep()
    {
        StartCoroutine(FallAsleepSequence());
    }

    IEnumerator FallAsleepSequence()
    {
        isHandlingState = true;
        float timer = 0;
        float startAlpha = sleepiness;
        float startSignAlpha = (warningSignRenderer != null) ? warningSignRenderer.color.a : 0f;

        while (timer < effectDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / effectDuration;

            float currentAlpha = Mathf.Lerp(startAlpha, 1f, progress);
            UpdateVignetteAlpha(currentAlpha);

            if (warningSignRenderer != null)
            {
                float signAlpha = Mathf.Lerp(startSignAlpha, 0f, progress);
                warningSignRenderer.color = new Color(1f, 1f, 1f, signAlpha);
            }

            if (cameraScript != null)
            {
                float currentStrength = Mathf.Lerp(0f, 0.01f, progress);
                cameraScript.swayAmountX = Mathf.Sin(timer * 3f) * currentStrength;
                cameraScript.swayAmountY = Mathf.Cos(timer * 2.5f) * (currentStrength * 0.6f);
            }
            yield return null;
        }

        UpdateVignetteAlpha(1f);
        if (warningSignRenderer != null) warningSignRenderer.color = new Color(1f, 1f, 1f, 0f);

        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}