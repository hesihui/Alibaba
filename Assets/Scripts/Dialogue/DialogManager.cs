using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;
    public GameObject dialogPanel;
    public Text npcNameText;
    public Text dialogText;
    public Button option1Button, option2Button, option3Button;
    public Text option1Text, option2Text, option3Text;

    private string[] currentDialogLines;
    private string[][] currentChoices;
    private string currentNpcName;
    private int dialogIndex = 0;

    void Awake()
    {
        Instance = this;
        dialogPanel.SetActive(false);

        option1Button.onClick.AddListener(() => SelectOption(0));
        option2Button.onClick.AddListener(() => SelectOption(1));
        option3Button.onClick.AddListener(() => SelectOption(2));
    }

    void Update()
    {
        if (dialogPanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            NextDialog();
        }
    }

    public void StartDialog(string[] dialogLines, string[][] choices, string npcName)
    {
        if (dialogLines == null || dialogLines.Length == 0)
        {
            Debug.LogError("Can't find the dialogue of " + npcName );
            return;
        }

        currentDialogLines = dialogLines;
        currentChoices = choices;
        currentNpcName = npcName;
        dialogIndex = 0;
        dialogPanel.SetActive(true);

        if (npcNameText != null)
        {
            npcNameText.text = npcName;
        }
        else
        {
            Debug.LogWarning("Npc Name Text is not bound.");
        }

        ShowText(currentDialogLines[dialogIndex]);
    }

    void NextDialog()
    {
        if (dialogIndex < currentDialogLines.Length - 1)
        {
            dialogIndex++;
            ShowText(currentDialogLines[dialogIndex]);
        }
        else
        {
            ShowChoices();
        }
    }

    void ShowText(string text)
    {
        if (dialogText != null)
        {
            dialogText.text = text;
        }
        else
        {
            Debug.LogWarning("Dialog Text is not bounnd.");
        }

        // Only display option at the end of lines 
        if (dialogIndex == currentDialogLines.Length - 1 && currentChoices.Length > 0)
        {
            ShowChoices();
        }
        else
        {
            option1Button.gameObject.SetActive(false);
            option2Button.gameObject.SetActive(false);
            option3Button.gameObject.SetActive(false);
        }
    }

    void ShowChoices()
    {
        if (currentChoices == null || currentChoices.Length == 0)
        {
            Debug.LogWarning("This NPC doesn't have options.");
            return;
        }

        option1Button.gameObject.SetActive(currentChoices.Length > 0);
        option2Button.gameObject.SetActive(currentChoices.Length > 1);
        option3Button.gameObject.SetActive(currentChoices.Length > 2);

        if (currentChoices.Length > 0 && currentChoices[0].Length > 0)
            option1Text.text = currentChoices[0][0]; 
        if (currentChoices.Length > 1 && currentChoices[1].Length > 0)
            option2Text.text = currentChoices[1][0]; 
        if (currentChoices.Length > 2 && currentChoices[2].Length > 0)
            option3Text.text = currentChoices[2][0]; 
    }
    
void SelectOption(int option)
{
    if (dialogIndex == currentDialogLines.Length - 1)
    {
        if (currentChoices.Length > option && currentChoices[option].Length > 0)
        {
            string selectedOption = currentChoices[option][0];

            Debug.Log("🎮 Player selected: " + selectedOption);

            // 硬编码 NPC 回应
            string npcResponse = "Hmm... interesting choice."; // 默认回应

            if (currentNpcName == "Old Man")
            {
                if (option == 0) npcResponse = "The secrets of this vault are older than time itself… Will you dare to uncover them?";
                if (option == 1) npcResponse = "This place is not what it seems. Keep your eyes open.";
                if (option == 2) npcResponse = "Hmph. Another lost soul ignoring the warnings.";
            }
            else if (currentNpcName == "Peasant")
            {
                if (option == 0) npcResponse = "Some say the vault chooses who can leave... and who must stay.";
                if (option == 1) npcResponse = "Silence? Hah… a rare choice. But sometimes, silence speaks louder than words.";
                if (option == 2) npcResponse = "Suit yourself, but knowledge is the true power.";
            }
            else if (currentNpcName == "Skeleton")
            {
                if (option == 0) npcResponse = "A wise move... the past may hold the key to your future.";
                if (option == 1) npcResponse = "A silent traveler? Hmph. You may last longer than most.";
                if (option == 2) npcResponse = "Leaving so soon? I was just starting to enjoy our little chat.";
            }

            // 显示 NPC 的回应
            ShowText(npcResponse);

            // 如果玩家选择 "Leave"，自动关闭对话
            if (selectedOption.ToLower().Contains("leave"))
            {
                Debug.Log("🛑 Closing dialog automatically.");
                CloseDialog();
                return;
            }

            // 隐藏选项按钮
            option1Button.gameObject.SetActive(false);
            option2Button.gameObject.SetActive(false);
            option3Button.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("⚠️ Warning: Selected option is out of range.");
        }
    }
}


    public void CloseDialog()
    {
        dialogPanel.SetActive(false);
    }
}
