using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RoomController : MonoBehaviour
{
    public Light2D roomLight;
    public Collider2D doorCollider;

    private GameObject lockedDoorVisual;

    [SerializeField] private GameObject doorNavBlocker;

    public bool isOpen = false;
    public bool isLightOn = false;


    private void Awake()
    {
        if (doorCollider != null)
        {
            Transform visualTransform = doorCollider.transform.Find("DoorVisual");
            if (visualTransform != null)
            {
                lockedDoorVisual = visualTransform.gameObject;
            }
        }
    }

    public void SetupRoom(bool open)
    {
        isOpen = open;

        if (doorCollider != null)
        {
            doorCollider.isTrigger = isOpen;

            if (lockedDoorVisual != null)
            {
                lockedDoorVisual.SetActive(!isOpen);
            }
        }

        if (doorNavBlocker != null)
        {
            doorNavBlocker.SetActive(!isOpen);
        }

        if (roomLight != null)
        {
            roomLight.enabled = false;
            isLightOn = false;
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
