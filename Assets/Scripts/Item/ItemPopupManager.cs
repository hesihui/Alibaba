using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;
    public GameObject popupPanel; // UI panel
    public Text popupText; // Text area
    public Button closeButton; // Close button

    private Dictionary<string, string> objectDescriptions = new Dictionary<string, string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        popupPanel.SetActive(false);
        closeButton.onClick.AddListener(ClosePopup);
        LoadDescriptionsFromFile();
    }

    void LoadDescriptionsFromFile()
    {
        TextAsset textFile = Resources.Load<TextAsset>("objects");

        if (textFile == null)
        {
            Debug.LogError("ERROR: objects.txt file not found in Resources/");
            return;
        }

        string[] lines = textFile.text.Split('\n');
        string currentObjectName = "";
        string currentText = "";

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            if (trimmedLine.StartsWith("#") || trimmedLine == "") continue; // Ignore comments and empty lines

            if (trimmedLine.StartsWith("OBJECT_NAME:"))
            {
                if (!string.IsNullOrEmpty(currentObjectName))
                {
                    objectDescriptions[currentObjectName] = currentText.Trim();
                }

                currentObjectName = trimmedLine.Replace("OBJECT_NAME:", "").Trim();
                currentText = "";
            }
            else if (trimmedLine.StartsWith("TEXT:"))
            {
                currentText += trimmedLine.Replace("TEXT:", "").Trim() + "\n";
            }
        }

        if (!string.IsNullOrEmpty(currentObjectName))
        {
            objectDescriptions[currentObjectName] = currentText.Trim();
        }

        Debug.Log("Loaded object descriptions: " + objectDescriptions.Count);
    }

    public void ShowPopup(string objectName)
    {
        if (objectDescriptions.ContainsKey(objectName))
        {
            popupText.text = objectDescriptions[objectName];
            popupPanel.SetActive(true);
            PauseGame();
        }
        else
        {
            Debug.LogError("ERROR: No text found for object: " + objectName);
        }
    }

    void ClosePopup()
    {
        popupPanel.SetActive(false);
        ResumeGame();
    }

    void PauseGame()
    {
        Time.timeScale = 0; // Pause the game
    }

    void ResumeGame()
    {
        Time.timeScale = 1; // Resume the game
    }
}
