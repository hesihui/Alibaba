using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InitalizePlayer : MonoBehaviour
{
    public Camera cam;
    public Camera fallingcam;

    public GameObject PixelatedPanel;
    public AudioSource AS;
    public AudioSource SES;


    public Animator PlayerAnimator;
    public GameObject PlayerAttack;
    public GameObject PlayerAttackExplosion;
    public GameObject PlayerAttackDecal;
    public Transform PlayerTransform;
    public CharacterController PlayerController;
    public GameObject SecretRoomExplosion;
    public GameObject XMark;
    public GameObject SecretRoomDoor;
    public GameObject Bomb;
    public GameObject BombExplosion;
    public GameObject PlayerStaff;
    public Sprite FullHeart;
    public Sprite HalfHeart;
    public Sprite EmptyHeart;
    public GameObject HealthPanel;
    public GameObject DamagePanel;
    public RuntimeAnimatorController HeartAnimatorController;


    public TMP_Text txtCoins;
    public TMP_Text txtKeys;
    public TMP_Text txtBombs;

    public GameObject ItemRoomSpawnSpot;
    public Transform StaffBone;

    public int FrameRate = 240;

    public FullScreenPassRendererFeature FSPRF;
    void Start()
    {

        Application.targetFrameRate = FrameRate;
        Player.txtCoins = txtCoins;
        Player.txtKeys = txtKeys;
        Player.txtBombs = txtBombs;
        Player.Audio = AS;
        Player.SoundEffectSource = SES;
        Player.Camera = cam;
        Player.FallingCamera = fallingcam;
        Player.PixelatedPanel = PixelatedPanel;
        Player.AttackExplosion = PlayerAttackExplosion;
        Player.AttackDecal = PlayerAttackDecal;
        Player.Attack = PlayerAttack;
        Player.Bomb = Bomb;
        Player.BombExplosion = BombExplosion;
        Player.FSPRF = FSPRF;

        Player.StaffBone = StaffBone;
        Player.transform = PlayerTransform;
        Player.animator = PlayerAnimator;
        Player.Controller = PlayerController;
        Player.PlayerStaff = PlayerStaff;
        Player.FullHeart = FullHeart;
        Player.EmptyHeart = EmptyHeart;
        Player.HalfHeart = HalfHeart;
        Player.HeartPanel = HealthPanel;
        Player.DamagePanel = DamagePanel;
        Player.HeartAnimator = HeartAnimatorController;
        Level.SecretRoomExplosion = SecretRoomExplosion;
        Level.SecretRoomDoor = SecretRoomDoor;
        Level.XMark = XMark;
        InitializeItemRoomItem();


        DontDestroyOnLoad(Player.Audio.gameObject);
        DontDestroyOnLoad(GameObject.Find("Main"));
        DontDestroyOnLoad(GameObject.Find("UI"));

    }

    GameObject IrI;
    public void InitializeItemRoomItem()
    {
        if ((IrI!=null))
        {
            GameObject.Destroy(IrI);
        }


        float totalWeight = 0;
        foreach (var item in Player.AvailableItems)
        {
            totalWeight += 1f / item.ItemLevel;
        }

        float randomWeight = Random.Range(0, totalWeight);

        Item selectedItem = null;
        float cumulativeWeight = 0;

        foreach (var item in Player.AvailableItems)
        {
            cumulativeWeight += 1f / item.ItemLevel;

            if (randomWeight <= cumulativeWeight)
            {
                selectedItem = item;
                break;
            }
        }


        foreach (Room R in Level.Rooms)
        {

            if (R.RoomType == "ItemRooms")
            {
                Transform T = GameObject.Find("ItemRooms").transform;

                Debug.Log("The room it's checking is: " + R.RoomNumber);
               ItemRoomSpawnSpot = T.Find(R.RoomNumber.ToString()).Find("ItemSpawnSpot").gameObject;
            }
        }

        Item ItemRoomItem = selectedItem;

        Debug.Log("Item Room Item:" + ItemRoomItem);
        Debug.Log("ItemRoomSpawnSpot:" + ItemRoomSpawnSpot);

        IrI = Instantiate(ItemRoomItem.Prefab,ItemRoomSpawnSpot.transform.position + 
            ItemRoomItem.Prefab.transform.position,
            ItemRoomItem.Prefab.transform.rotation,
            ItemRoomSpawnSpot.transform);
       
    }

    public static IEnumerator PixelateTransition()
    {
        float MaxPixelSize = 200;
        yield return new WaitForSeconds(.01f);

        Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/LevelMusic/1/Intro"), 2);
        Player.FSPRF.SetActive(true);
        Player.FSPRF.passMaterial.SetFloat("_PixelSize", MaxPixelSize);

        float pixelationspeed = .005f;
        for (float i = MaxPixelSize; i >= 1; --i)
        {
            yield return new WaitForSeconds(pixelationspeed);
            Player.FSPRF.passMaterial.SetFloat("_PixelSize", i);
        }

        Player.FSPRF.SetActive(false);
        Player.FSPRF.passMaterial.SetFloat("_PixelSize", 1);
    }

    public static void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {

        Player.Audio.Stop();
        Player.Audio.clip = Resources.Load<AudioClip>("Sounds/LevelMusic/1/cwkkbackup");
        Player.Audio.Play();

        GameObject.Find("Map").GetComponent<GenerateLevel>().Regenerate();
        GameObject.Find("ItemAndShopManager").GetComponent<ShopManager>().Start();

        CoroutineManager.Instance.StartCoroutine(PixelateTransition());

        Player.Controller.enabled = false;
        Player.transform.position = new Vector3(0, 200, 0);
        Player.Controller.enabled = true;
        Player.transform.GetComponent<InitalizePlayer>().InitializeItemRoomItem();


        Player.transform.GetComponent<ChangeRooms>().EnableDoors(Player.CurrentRoom);
    }

    public class CoroutineManager : MonoBehaviour
    {
        private static CoroutineManager _instance;

        public static CoroutineManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("CoroutineManager");
                    _instance = go.AddComponent<CoroutineManager>();
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            _instance = this;
        }

    }
}
