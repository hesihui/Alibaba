using UnityEngine;
using System.Collections.Generic;

public class NPCTrigger : MonoBehaviour
{
    [Header("NPC Settings")]
    [SerializeField] public string npcName; // Ensure NPC name is set in Inspector
    
    public string[] dialogLines; // Dialog content
    public string[][] choices; // Choice options

    private bool isPlayerNear = false;

    void Start()
    {
        if (string.IsNullOrEmpty(npcName))
        {
            Debug.LogError("ERROR: npcName is not set in the Inspector for " + gameObject.name);
            return;
        }

        LoadDialogFromFile();
    }

    void LoadDialogFromFile()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("dialog");


        string[] lines = textAsset.text.Split('\n');
        List<string> dialogList = new List<string>();
        List<string[]> choiceList = new List<string[]>();

        bool isCurrentNPC = false;

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            if (trimmedLine.StartsWith("#")) continue; // Ignore comments

            if (trimmedLine.StartsWith("NPC_NAME:"))
            {
                string name = trimmedLine.Replace("NPC_NAME:", "").Trim();
                isCurrentNPC = name == npcName; // Load dialog for this NPC


            }

            if (isCurrentNPC)
            {
                if (trimmedLine.StartsWith("DIALOG:"))
                {
                    string[] dialogs = trimmedLine.Replace("DIALOG:", "").Trim().Split('|');
                    dialogList.AddRange(dialogs);
                }
                else if (trimmedLine.StartsWith("OPTION1:"))
                {
                    choiceList.Add(trimmedLine.Replace("OPTION1:", "").Trim().Split('|'));
                }
                else if (trimmedLine.StartsWith("OPTION2:"))
                {
                    choiceList.Add(trimmedLine.Replace("OPTION2:", "").Trim().Split('|'));
                }
                else if (trimmedLine.StartsWith("OPTION3:"))
                {
                    choiceList.Add(trimmedLine.Replace("OPTION3:", "").Trim().Split('|'));
                }
            }
        }

        if (dialogList.Count == 0)
        {
            Debug.LogWarning("WARNING: No dialog found for NPC: " + npcName);
        }

        dialogLines = dialogList.ToArray();
        choices = choiceList.ToArray();
    }

    void Update()
    {
        if (isPlayerNear)
        {
            
            if (Input.GetKeyDown(KeyCode.E))
            {

                DialogManager.Instance.StartDialog(dialogLines, choices, npcName);
            }
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
