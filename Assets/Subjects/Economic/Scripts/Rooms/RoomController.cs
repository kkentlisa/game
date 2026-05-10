using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RoomController : MonoBehaviour
{
    public Light2D roomLight;
    public Collider2D doorCollider;

    public bool isOpen = false;
    public bool isLightOn = false;

    public void SetupRoom(bool open)
    {
        isOpen = open;

        if (doorCollider != null)
        {
            doorCollider.isTrigger = isOpen;
        }

        if (roomLight != null)
        {
            roomLight.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen && !isLightOn && other.CompareTag("Player"))
        {
            if (roomLight != null)
            {
                roomLight.enabled = true;
                isLightOn = true;
            }
        }
    }
}
