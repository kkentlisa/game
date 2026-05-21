using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CoinsPickup : MonoBehaviour
{
    public int value = 1;
    private float lifeTime = 8f;

    private bool collected = false;

    private void Start()
    {
        StartCoroutine(LifeTimeTimer());
    }

        private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;
            ScoreManager.Instance.AddScore("Player", value);
            Destroy(gameObject);
        }
    }

    private IEnumerator LifeTimeTimer()
    {
        yield return new WaitForSeconds(lifeTime);

        if (!collected)
        {
            Destroy(gameObject);
        }
    }

}
