using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ItemRoomStoryPopupManager : MonoBehaviour
{
    public GameObject storyPanel; // UI panel for the popup
    public Text storyText; // UI text to display the story
    public Button closeButton; // Button to close the popup

    private bool isPaused = false;

    private void Start()
    {
        LoadStoryText();
        ShowPopup();
        closeButton.onClick.AddListener(ClosePopup);
    }

    void LoadStoryText()
    {
        TextAsset storyFile = Resources.Load<TextAsset>("itemroomstory");

        if (storyFile != null)
        {
            storyText.text = storyFile.text;
        }
        else
        {
            Debug.LogError("Story file not found in Resources/story.txt");
            storyText.text = "Error: Story file missing.";
        }
    }

    void ShowPopup()
    {
        storyPanel.SetActive(true);
        PauseGame();
    }

    void ClosePopup()
    {
        storyPanel.SetActive(false);
        ResumeGame();
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0; // Pause the game
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1; // Resume the game
    }
}
