using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
   
    void Start()
    {
        DontDestroyOnLoad(gameObject); 

        if(Player.AvailableItems.Count == 0)
        Player.AvailableItems = Resources.LoadAll<Item>("Items").ToList();
    }

    public static IEnumerator ResetPopUp(GameObject go)
    {
        yield return new WaitForSeconds(5);
        go.SetActive(false);
    }


    
    public static void PickupItem(Item item)
    {
        Player.PlayerItems.Add(item);
        GameObject GO = GameObject.Find("Canvas").transform.Find("PopupPanel").gameObject;
        GO.SetActive(true);
        GO.transform.Find("PopupText").GetComponent<TMP_Text>().text = "You picked up a\n" + item.name +
            "\n" + item.ItemCaption;

        CoroutineManager.Instance.StartCoroutine(ResetPopUp(GO));

       
        if(item.Equipment != "" && item.Equipment != null)
        {

            Debug.Log("Equiping:" + item.Equipment);
            if (item.Equipment.ToLower().Contains("staff"))//if it's a staff we need to find it in the players transform, it's disabled
            {
                //disable all other staffs.
                foreach (Transform child in Player.StaffBone)
                {
                    if (child.gameObject.name != "Bone_end")
                    {
                        child.gameObject.SetActive(false);
                    }
                }
                Player.StaffBone.Find(item.Equipment).gameObject.SetActive(true);
            }
            else
            {
                GameObject.Find(item.Equipment).transform.GetComponent<MeshRenderer>().enabled = true;
            }
        }

        Player.Armor += item.Armor;
        Player.AttackSize += item.AttackSize;
        Player.Damage += item.Damage;
        Player.DamageMultiplier += item.DamageMultiplier;
        Player.Speed += item.MoveSpeed;
        Player.AttackSpeed += item.AttackSpeed; Player.animator.SetFloat("AttackSpeed", Player.AttackSpeed);
        Player.Health += item.Health; if (Player.Health > 20) Player.Health = 20;
        Player.MaxHealth += item.MaxHealth; if(Player.MaxHealth > 20) Player.MaxHealth = 20;    
        HeartScript.DrawHearts();
        Player.ShotSpeed += item.ShotSpeed;

        if (item.Special != "") Player.Specials.Add(item.Special);

        if (item.AttackPrefab != null) { Player.Attack = item.AttackPrefab; }
        if (item.AttackExplosionPrefab != null) { }




        //more shit :D
        if (item.name.Contains("Skull"))
        {
            Renderer[] renderers = Player.transform.GetComponentsInChildren<Renderer>();

            // Loop through each Renderer
            foreach (Renderer renderer in renderers)
            {
                // Ensure the renderer has materials
                if (renderer.materials.Length == 0)
                {
                    Debug.LogWarning("Renderer has no materials assigned: " + renderer.name);
                    continue;
                }

                // Get all materials in the Renderer
                Material[] materials = renderer.materials;

                // Loop through each material
                for (int i = 0; i < materials.Length; i++)
                {
                    // Check if the material's name is "PMaterial"
                    if (materials[i].name.Contains("PMaterial"))
                    {
                        // Create a temporary copy of the material
                        Material tempMat = new Material(materials[i]);

                        // Slightly increase the whiteness (ensure color doesn't go above (1,1,1,1))
                        Color originalColor = tempMat.color;
                        tempMat.color = new Color(
                            Mathf.Min(originalColor.r + 1f, 1f),
                            Mathf.Min(originalColor.g + 1f, 1f),
                            Mathf.Min(originalColor.b + 1f, 1f),
                            originalColor.a
                        );
                        tempMat.SetFloat("_Metallic", 0);

                        // Safeguard before assigning to materials array
                        if (i >= 0 && i < materials.Length)
                        {
                            materials[i] = tempMat;
                        }
                        else
                        {
                            Debug.LogError("Invalid material index: " + i + " for renderer: " + renderer.name);
                        }
                    }
                }

                // Reassign modified materials back to the renderer
                renderer.materials = materials;
            }
        }
    }


   

}
