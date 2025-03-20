using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathPanel : MonoBehaviour
{
    // public GameObject DeathPanelObject;
    public TMPro.TextMeshProUGUI DeathPanelText;
    // public GameObject RestartButton;
    // public GameObject QuitButton;
    // public int deathCount;
    // public int keyCount;
    // public int coinCount;
    // public List<Item> items = new List<Item>();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartGame()
    {
        Player.DeathPanel.SetActive(false);
        Application.LoadLevel(Application.loadedLevel);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // public void SetDeathPanelText(int deathCount, int keyCount, int coinCount, List<Item> items)
    // {
    //     string text = "You died " + deathCount + " times.\n";
    //     text += "You collected " + keyCount + " keys.\n";
    //     text += "You collected " + coinCount + " coins.\n";
    //     text += "You collected the following items:\n";
    //     foreach (Item item in items)
    //     {
    //         text += item.ItemCaption + "\n";
    //     }
    //     DeathPanelText.text = text;
    // }

}
