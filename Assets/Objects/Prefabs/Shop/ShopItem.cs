using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShopItem : ScriptableObject
{
    public GameObject Prefab;
    public int ShopLevel;
    public int Cost;


    public ShopItem Clone()
    {
        return new ShopItem
        {
            name = this.name,
            Prefab = this.Prefab,
            ShopLevel = this.ShopLevel,
            Cost = this.Cost
        };
    }
}
