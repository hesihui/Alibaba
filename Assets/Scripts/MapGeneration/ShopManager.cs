using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }
    public Dictionary<ShopItem, GameObject> shopItemMapping = new Dictionary<ShopItem, GameObject>();

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
    }

    public void Start()
    {
        if (Player.AvailableShopItems.Count == 0)
        {
            Player.AvailableShopItems = Resources.LoadAll<ShopItem>("ShopItems").ToList();
        }

        Transform ShopTransform = GameObject.Find("ShopRooms")
            .transform
            .Find(Level.Rooms.First(x => x.RoomType == "ShopRooms")
            .RoomNumber.ToString());



        Horizontal5Formation(ShopTransform);
    }

    private void Horizontal5Formation(Transform T)
    {
        float spacing = 15f;

        for (int i = 0; i < 5; i++)
        {
            int random = Random.Range(0, Player.AvailableShopItems.Count);
            float xOffset = (i - 2) * spacing;
            Vector3 newPosition = T.position + new Vector3(xOffset, 0, 0);
            GameObject go = Instantiate(Player.AvailableShopItems[random].Prefab, newPosition, Quaternion.identity, T);

            // Set the color to red because we won't have enough money for any items by default
            go.transform.Find("txtShopItem").GetComponent<TextMeshPro>().color = Color.red;
            go.transform.Find("txtShopItem").GetComponent<TextMeshPro>().text = Player.AvailableShopItems[random].Cost.ToString();

            ShopItem item = Player.AvailableShopItems[random].Clone();
            Player.ShopItems.Add(item);

            // Store the instantiated game object and its corresponding ShopItem data
            shopItemMapping[item] = go;
        }
    }

    public static void CheckMoney()
    {
        foreach (var kvp in Instance.shopItemMapping)
        {
            ShopItem item = kvp.Key;
            GameObject go = kvp.Value;

            if (Player.Coins >= item.Cost)
            {
                go.transform.Find("txtShopItem").GetComponent<TextMeshPro>().color = Color.white;
            }
            else
            {
                go.transform.Find("txtShopItem").GetComponent<TMP_Text>().color = Color.red;
            }
        }
    }

    public void RemovePurchasedItem(ShopItem shopItem)
    {
        if (shopItemMapping.TryGetValue(shopItem, out GameObject shopItemObject))
        {
            shopItemMapping.Remove(shopItem);
            Player.ShopItems.Remove(shopItem);
            Destroy(shopItemObject);
        }
    }


    public bool PickupItem(string Name)
    {
        switch(Name)
        {
            case "Key":
                {
                    Player.Keys += 1;
                    Player.txtKeys.text = $"X{Player.Keys:D2}";
                    return true;
                }

            case "Bomb":
                {
                    Player.Bombz += 1;
                    Player.txtBombs.text = $"X{Player.Bombz:D2}";
                    return true;
                }

            case "Heart":
                {
                    if (Player.Health < Player.MaxHealth)
                    {
                        Player.Health += 1;
                        HeartScript.DrawHearts();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            case "Bowl Of Balls":
                {
                    Player.MaxHealth += 1;
                    Player.Health += 1;
                    HeartScript.DrawHearts();
                    return true;
                }
            case "Map":
                {
                    foreach (Room R in Level.Rooms)
                    {
                        R.Revealed = true;
                        R.Explored = true;
                    }

                    ChangeRooms.RedrawRevealedRooms();
                    return true;
                }
        }


        return false;
    }
}
