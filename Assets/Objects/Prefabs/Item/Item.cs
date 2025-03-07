using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public GameObject Prefab; //The item that shows on the ground

    public string ItemCaption;

    public string Equipment; //If the item changes your characters appearance this will enable the game object to be enabled.
                             //This has to be a string because you can't link game objects in the scene to scriptable objects.
                             //The mesh renderer will be what gets enabled.


    public int ItemLevel; //The item level will be used similar to the "Quality" in TBOI

    public float Armor = 0;
    public float AttackSize = 0;
    public float Damage = 0;
    public float DamageMultiplier = 0;
    public float MoveSpeed = 0;
    public float AttackSpeed = 0;
    public float ShotSpeed = 0;
    public float Health = 0;
    public float MaxHealth = 0;

    public string Special = ""; // Special will be used if the item received does something extra.

    public GameObject AttackPrefab; //used to replace the attack if the attack changes.
    public GameObject AttackExplosionPrefab; //used to replace the attack explosion if the attack changes.
    
}
