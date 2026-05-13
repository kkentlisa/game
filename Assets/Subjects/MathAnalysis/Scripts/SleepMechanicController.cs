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

    private Vector3 initialSignScale;

    void Start()
    {
        if (warningSignRenderer != null)
        {
            initialSignScale = warningSignRenderer.transform.localScale;
        }
    }

    public void SetSleepSpeed(float newSpeed)
    {
        fallAsleepSpeed = newSpeed;
    }

    void Update()
    {
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

        if (sleepiness >= 1f)
        {
            HandleFallAsleep();
        }
    }

    void UpdateVignetteAlpha(float alpha)
    {
        foreach (var sprite in blackScreens)
        {
            if (sprite != null)
                sprite.color = new Color(0, 0, 0, alpha);
        }
    }

    void HandleFallAsleep()
    {
        Debug.Log("БАМ! Удар головой о парту. Игрок уснул.");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}