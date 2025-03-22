using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DeathPanel : MonoBehaviour
{
    // public GameObject DeathPanelObject;
    public TMPro.TextMeshProUGUI DeathPanelText;
    public GameObject player;
    // PlayerMovement playerMovement;

    // public GameObject RestartButton;
    // public GameObject QuitButton;
    // public int deathCount;
    // public int keyCount;
    // public int coinCount;
    // public List<Item> items = new List<Item>();

    void Start()
    {
        // playerMovement = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        Player.DeathPanel.SetActive(false);
        SceneManager.LoadScene("Level1");
        // player.GetComponent<InitalizePlayer>().InitializeGame();
        Player.Health = Player.MaxHealth;
        Player.Coins = 0;
        Player.Keys = 0;
        Player.PlayerItems.Clear(); 
        
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
