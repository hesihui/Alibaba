using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public GameObject dialogPanel; // Bind the dialog panel
    public Text dialogText; // Bind the dialog text (Legacy UI)
    public Button option1Button, option2Button, option3Button; // Choice buttons
    public Text option1Text, option2Text, option3Text; // Text components for buttons

    private int dialogIndex = 0;

    // Dialog content
    private string[] dialogLines = new string[]
    {
        "Hello, traveler!",
        "This was once a great kingdom, but now it lies in ruins.",
        "What would you like to do?"
    };

    // Choices for dialog
    private string[][] choices = new string[][]
    {
        new string[] { "Continue the story", "Ask about the quest", "Leave" },
        new string[] { "Learn more about the kingdom's history", "Ask about the monsters", "End the conversation" }
    };

    void Start()
    {
        dialogPanel.SetActive(false); // Hide the dialog panel at the start

        // Add button click listeners
        option1Button.onClick.AddListener(() => SelectOption(0));
        option2Button.onClick.AddListener(() => SelectOption(1));
        option3Button.onClick.AddListener(() => SelectOption(2));
    }

    public void StartDialog()
    {
        dialogPanel.SetActive(true);

        if (dialogIndex < dialogLines.Length)
        {
            ShowText(dialogLines[dialogIndex]);
            dialogIndex++; // 让对话推进
        }
        else
        {
            ShowChoices(0);
        }
    }

    void ShowText(string text)
    {
        dialogText.text = text;

        // 如果是最后一条对话，就显示选项按钮
        if (dialogIndex == dialogLines.Length)
        {
            ShowChoices(0);
        }
        else
        {
            option1Button.gameObject.SetActive(false);
            option2Button.gameObject.SetActive(false);
            option3Button.gameObject.SetActive(false);
        }
    }

    void ShowChoices(int choiceIndex)
    {
        option1Button.gameObject.SetActive(true);
        option2Button.gameObject.SetActive(true);
        option3Button.gameObject.SetActive(true);

        option1Text.text = choices[choiceIndex][0];
        option2Text.text = choices[choiceIndex][1];
        option3Text.text = choices[choiceIndex][2];
    }

    void SelectOption(int option)
    {
        if (dialogIndex == dialogLines.Length)
        {
            switch (option)
            {
                case 0:
                    ShowText("This kingdom was once ruled by a mighty king...");
                    break;
                case 1:
                    ShowText("Recently, monsters have been spotted near the castle...");
                    break;
                case 2:
                    CloseDialog();
                    break;
            }
        }
    }

    public void CloseDialog()
    {
        dialogPanel.SetActive(false);
        dialogIndex = 0; // 重置对话索引
    }
}
