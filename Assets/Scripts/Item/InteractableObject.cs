using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string objectName; // Set this in Inspector (must match objects.txt)

    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Interacting with: " + objectName);
            PopupManager.Instance.ShowPopup(objectName);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            Debug.Log("🚶 Player near: " + objectName);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            Debug.Log("🏃 Player left: " + objectName);
        }
    }
}
