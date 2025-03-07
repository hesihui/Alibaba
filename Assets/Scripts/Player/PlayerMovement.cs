using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController Controller;
    public GameObject CoinPickup;
    private Vector3 moveDir = new Vector3(0f, 0f, 0f);
    private int originalWidth = 1920;
    private int originalHeight = 1080;
    private bool grounded = false;
    private bool pixelating = false;
    public float fallSpeed = -9.8f;
    public float MaxPixelSize = 200;

    public FullScreenPassRendererFeature FSPRF;
 
    private void Start()
    {
        Player.animator.SetFloat("Speed", Player.Speed / 20);
        

        StartCoroutine(StartGame());
    }
    

    private void Update()
    {
        if (Input.GetKey(KeyCode.H))
        {
            StartCoroutine(StartGame());
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if ((h != 0 || v != 0) && (Player.State == "Idle" || Player.State == "HoldingBomb"))
        {
            Player.animator.SetFloat("Speed", 1);
        }
        else
        {
            Player.animator.SetFloat("Speed", 0);
        }

        float ymove = moveDir.y;

        Vector3 move = new Vector3(h, ymove, v) * Player.Speed * Time.deltaTime;
        Controller.Move(move);
        Gravity();

        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (playerPlane.Raycast(ray, out float hitDist))
        {
            Vector3 targetPoint = ray.GetPoint(hitDist);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 50f * Time.deltaTime);
        }
    }

    private void Gravity()
    {
        if (!grounded)
        {
            moveDir.y = fallSpeed * Time.deltaTime;
        }
        else
        {
            moveDir.y = 0;
        }

        var flags = Controller.Move(moveDir * Time.deltaTime);
        grounded = (flags & CollisionFlags.CollidedBelow) != 0;

        if (transform.position.y < -30 && !pixelating)
        {
            pixelating = true;
            StartCoroutine(BlurScreenAndTeleport());
        }
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(.01f);

        Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/LevelMusic/1/Intro"), 2);
        FSPRF.SetActive(true);
        FSPRF.passMaterial.SetFloat("_PixelSize", MaxPixelSize);

        float pixelationspeed = .005f;
        for(float i = MaxPixelSize; i >= 1; --i)
        {
            yield return new WaitForSeconds(pixelationspeed);
            FSPRF.passMaterial.SetFloat("_PixelSize", i);
        }

        FSPRF.SetActive(false);
        FSPRF.passMaterial.SetFloat("_PixelSize", 1);
        moveDir.y = 0;
    }

    private IEnumerator BlurScreenAndTeleport()
    {

        Player.Audio.volume = .3f;
        Player.SoundEffectSource.pitch = 1;
        Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/LevelMusic/1/Intro"), 2);

        FSPRF.SetActive(true);
        FSPRF.passMaterial.SetFloat("_PixelSize", 1);

        float pixelationspeed = .003f;
        for (int i = 1; i <= MaxPixelSize; ++i)
        {
            yield return new WaitForSeconds(pixelationspeed);
            FSPRF.passMaterial.SetFloat("_PixelSize", i);
        }

        Player.Audio.volume = .5f;
        Player.Controller.enabled = false;
        moveDir.y = 0;
        Player.transform.position = new Vector3(0, 100, 0);
        Player.Controller.enabled = true;

        FSPRF.passMaterial.SetFloat("_PixelSize", MaxPixelSize);
        pixelationspeed = .005f;

        for (float i = MaxPixelSize; i >= 1; --i)
        {
            yield return new WaitForSeconds(pixelationspeed);
            FSPRF.passMaterial.SetFloat("_PixelSize", i);
        }
        FSPRF.SetActive(false);
        pixelating = false;
        moveDir.y = 0;
        HeartScript.TakeDamage(1);
       
    }



   

    private void LateUpdate()
    {
        Vector3 targetPosition = transform.position + new Vector3(0, 109, 0);
        Vector3 newPosition = Vector3.MoveTowards(Player.Camera.transform.position, targetPosition, 500f * Time.deltaTime);
        Player.Camera.transform.position = newPosition;
    }
    private void OnDestroy()
    {
        FSPRF.SetActive(false);
        FSPRF.passMaterial.SetFloat("_PixelSize", 1);


        StopAllCoroutines();
        
    }
    private void OnApplicationQuit()
    {
        FSPRF.SetActive(false);
        FSPRF.passMaterial.SetFloat("_PixelSize", 1);


        StopAllCoroutines();
        
    }
   

    private IEnumerator DestroyItLater(GameObject go, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(go);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb != null)
        {
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            rb.AddForce(pushDir * 5, ForceMode.Impulse);
        }

        if (hit.transform.name.Contains("Coin") ||
            hit.transform.name.Contains("Nickel") ||
            hit.transform.name.Contains("Dime") ||
            hit.transform.name.Contains("Penny"))
        {
            HandleCoinPickup(hit);
        }
        else if(hit.transform.name.Contains("PortalGraph"))
        {
            if (hit.transform.GetComponent<VisualEffect>().aliveParticleCount > 0) Level.LoadNextLevel();
            
        }
        else if (hit.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            if (!Player.AlreadyPickedUpCoins.Contains(hit.transform.GetInstanceID()))
            {
                Player.AlreadyPickedUpCoins.Add(hit.transform.GetInstanceID());
                ItemManager.PickupItem(Player.AvailableItems.First(x => hit.transform.name.Replace("(Clone)","") == x.name));

                if (Player.AvailableItems.First(x => hit.transform.name.Contains(x.name)).name != "Bowl Of Balls")
                {
                    if (hit.transform.name.Contains("Cube Staff"))
                    {
                        Player.AvailableItems.RemoveAll(x => x.name.Contains("Cube Staff"));
                    }
                    else if(hit.transform.name.Contains("Helmet"))
                    {
                        Player.AvailableItems.RemoveAll(x => x.name.Contains("Helmet"));
                    }
                    else
                    {
                        Player.AvailableItems.Remove(Player.AvailableItems.First(x => hit.transform.name.Contains(x.name)));
                    }
                }
          

                GameObject.Destroy(hit.gameObject);
            }
        }
        else if (hit.transform.name.Contains("healthheart"))
        {
            HandleHeartPickup(hit);
        }
        else if (hit.transform.name == "Key(Clone)")
        {
            HandleKeyPickup(hit);
        }
        else if (hit.transform.name == "Bomb(Clone)")
        {
            HandleBombPickup(hit);
        }
        else if (hit.transform.name.Contains("SimpleChest"))
        {
            HandleChestPickup(hit);
        }
        else if (hit.transform.name.Contains("GoldChest"))
        {
            HandleGoldChestPickup(hit);
        }
        else if (hit.transform.name.Contains("Spikes"))
        {
            ImpactReceiver.AddImpactOnGameObject(Player.transform.gameObject, (Player.transform.position - hit.transform.position) * (Player.KnockBack/2));
            HeartScript.TakeDamage(1);
        }
        else if (hit.transform.CompareTag("ShopItem"))
        {
            ShopItem shopItem = ShopManager.Instance.shopItemMapping.FirstOrDefault(x => x.Value == hit.gameObject).Key;

            if (shopItem != null)
            {
                HandleShopItem(shopItem);
            }
        }
       
    }

    private void HandleShopItem(ShopItem shopItem)
    {
        if (Player.Coins >= shopItem.Cost)
        {
            if (ShopManager.Instance.PickupItem(shopItem.name))
            {
                Player.Coins -= shopItem.Cost;
                UpdateCoinText();
                ShopManager.Instance.RemovePurchasedItem(shopItem);
                ShopManager.CheckMoney();
                Player.SoundEffectSource.pitch = Random.Range(.8f, 1.5f);
                Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/CoinPickup"), 2);
            }
        }
    }

        private void HandleHeartPickup(ControllerColliderHit hit)
    {
        if (!Player.AlreadyPickedUpCoins.Contains(hit.transform.GetInstanceID()))
        {
            if (Player.Health < Player.MaxHealth)
            {
                Player.Health += 1;
                HeartScript.DrawHearts();
                Player.AlreadyPickedUpCoins.Add(hit.transform.GetInstanceID());
                Player.Audio.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/Gulp"), 2);
                StartCoroutine(DestroyItLater(Instantiate(CoinPickup, hit.transform.position, Quaternion.identity), 3));
                Destroy(hit.transform.gameObject);
            }
            else { return; }
        }
    }

    private void HandleKeyPickup(ControllerColliderHit hit)
    {
        if (!Player.AlreadyPickedUpCoins.Contains(hit.transform.GetInstanceID()))
        {
            Player.AlreadyPickedUpCoins.Add(hit.transform.GetInstanceID());
            Player.Keys += 1;
            UpdateKeyText();
            Player.SoundEffectSource.pitch = Random.Range(.8f, 1.5f);
            Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/CoinPickup"), 2);
            StartCoroutine(DestroyItLater(Instantiate(CoinPickup, hit.transform.position, Quaternion.identity), 3));
            Destroy(hit.transform.gameObject);
        }
    }

    private void HandleBombPickup(ControllerColliderHit hit)
    {
        if (!Player.AlreadyPickedUpCoins.Contains(hit.transform.GetInstanceID()))
        {
            Player.AlreadyPickedUpCoins.Add(hit.transform.GetInstanceID());
            Player.Bombz += 1;
            UpdateBombText();
            Player.SoundEffectSource.pitch = Random.Range(.8f, 1.5f);
            Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/CoinPickup"), 2);
            StartCoroutine(DestroyItLater(Instantiate(CoinPickup, hit.transform.position, Quaternion.identity), 3));
            Destroy(hit.transform.gameObject);
        }
    }

    private void HandleCoinPickup(ControllerColliderHit hit)
    {
        if (!Player.AlreadyPickedUpCoins.Contains(hit.transform.GetInstanceID()))
        {
            Player.AlreadyPickedUpCoins.Add(hit.transform.GetInstanceID());
            if (hit.transform.name.Contains("Penny")) Player.Coins += 1;
            else if (hit.transform.name.Contains("Nickel")) Player.Coins += 5;
            else if (hit.transform.name.Contains("Dime")) Player.Coins += 10;
            else if (hit.transform.name.Contains("Coin")) Player.Coins += 25;
            UpdateCoinText();
            ShopManager.CheckMoney();
            Player.SoundEffectSource.pitch = Random.Range(.8f, 1.5f);
            Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/CoinPickup"), 2);
            StartCoroutine(DestroyItLater(Instantiate(CoinPickup, hit.transform.position, Quaternion.identity), 3));
            Destroy(hit.transform.gameObject);
        }
    }

   

    private void HandleChestPickup(ControllerColliderHit hit)
    {
        if (!Player.AlreadyPickedUpCoins.Contains(hit.transform.GetInstanceID()))
        {
            Player.AlreadyPickedUpCoins.Add(hit.transform.GetInstanceID());

            StartCoroutine(hit.transform.GetComponent<ChestScript>().MoveObjectUpwards(hit.transform.GetChild(1).transform, 50, 0.5f));
            StartCoroutine(hit.transform.GetComponent<ChestScript>().FlashAndFadeCoroutine());

            GameObject.Destroy(Instantiate(hit.transform.GetComponent<ChestScript>().Stars, hit.transform.position, Quaternion.identity).gameObject, 5);

            DropItemsFromChest(hit.transform);
        }
    }

    private void HandleGoldChestPickup(ControllerColliderHit hit)
    {
        if (!Player.AlreadyPickedUpCoins.Contains(hit.transform.GetInstanceID()))
        {
            if (Player.Keys > 0) { Player.Keys--; } else {
                Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/Rejected"), 1);
                return; }
            Player.Audio.PlayOneShot(Resources.Load<AudioClip>("Sounds/LevelMusic/1/LatchDoor"), 1);
            Player.AlreadyPickedUpCoins.Add(hit.transform.GetInstanceID());
            StartCoroutine(hit.transform.GetComponent<ChestScript>().MoveObjectUpwards(hit.transform.GetChild(1).transform, 50, 0.5f));
            StartCoroutine(hit.transform.GetComponent<ChestScript>().FlashAndFadeCoroutine());
            GameObject.Destroy(Instantiate(hit.transform.GetComponent<ChestScript>().Stars, hit.transform.position, Quaternion.identity).gameObject, 5);
            DropItemsFromGoldChest(hit.transform);
        }
    }

    private void DropItemsFromChest(Transform chestTransform)
    {
        float force = 50f;
        int chestMultiplier = 5;

        Player.Audio.PlayOneShot(Resources.Load<AudioClip>("Sounds/LevelMusic/1/LatchDoor"), 1);

        for (int i = 0; i < chestMultiplier; i++)
        {
            foreach (Drop drop in Player.AvailableDrops)
            {
                int limiter = 0;
                while (Random.value < drop.DropChance && limiter <= 10)
                {
                    limiter++;
                    GameObject droppedItem = Instantiate(drop.Prefab, chestTransform.position + new Vector3(0, 10, 0), Quaternion.identity);
                    droppedItem.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-force, force), Random.Range(30, 50), Random.Range(-force, force)), ForceMode.Impulse);
                    
                }
            }

          
        }
    }

    private void DropItemsFromGoldChest(Transform chestTransform)
    {

        //check if a real item should spawn.
        if(UnityEngine.Random.value > .5f)
        {
            Item ItemRoomItem = Player.AvailableItems[UnityEngine.Random.Range(0, Player.AvailableItems.Count)];
            GameObject IrI = Instantiate(ItemRoomItem.Prefab, chestTransform.transform.position + ItemRoomItem.Prefab.transform.position, ItemRoomItem.Prefab.transform.rotation);
            IrI.transform.localPosition = new Vector3(IrI.transform.localPosition.x, 5, IrI.transform.localPosition.z);
            IrI.transform.localScale = new Vector3(4, 4, 4);
        }
        else { 
        float force = 50f;
        int chestMultiplier = 45;
            for (int i = 0; i < chestMultiplier; i++)
            {
                foreach (Drop drop in Player.AvailableDrops)
                {
                    int limiter = 0;
                    while (Random.value < drop.DropChance && limiter <= 10)
                    {
                        limiter++;
                        GameObject droppedItem = Instantiate(drop.Prefab, chestTransform.position + new Vector3(0, 10, 0), Quaternion.identity);
                        droppedItem.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-force, force), Random.Range(30, 50), Random.Range(-force, force)), ForceMode.Impulse);
                        Player.Audio.PlayOneShot(Resources.Load<AudioClip>("Sounds/Player/CoinNoise"), 1);
                    }
                }
            }
        }
    }

    private void UpdateCoinText()
    {
        Player.txtCoins.text = $"X{Player.Coins:D2}";
    }
    private void UpdateKeyText()
    {
        Player.txtKeys.text = $"X{Player.Keys:D2}";
    }
    private void UpdateBombText()
    {
        Player.txtBombs.text = $"X{Player.Bombz:D2}";
    }
}
