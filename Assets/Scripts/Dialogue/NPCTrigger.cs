using UnityEngine;

public class NPCTrigger : MonoBehaviour
{
    public DialogManager dialogManager; 
    private bool isPlayerNear = false;

    void Update()
    {

        if (isPlayerNear && Input.GetKeyDown(KeyCode.E)) 
        {
            dialogManager.StartDialog();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            isPlayerNear = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
