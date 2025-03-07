using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class Player
{
    public static List<Drop> AvailableDrops = new List<Drop>();

    public static List<ShopItem> AvailableShopItems = new List<ShopItem>();
    public static List<ShopItem> ShopItems = new List<ShopItem>();

    public static List<Item> AvailableItems = new List<Item>();
    public static List<Item> PlayerItems = new List<Item>();

    public static AudioSource Audio;
    public static AudioSource SoundEffectSource;

    public static FullScreenPassRendererFeature FSPRF;

    public static string State = "Idle";
    public static bool Invincible = false;
    public static Transform transform;
    public static GameObject PlayerStaff;
    public static Animator animator;

    public static List<int> AlreadyPickedUpCoins = new List<int>();

    public static Material DissolveBossDoorMaterial;

    public static int Coins = 0;
    public static int Keys = 0;
    public static int Bombz = 0;

    public static TMP_Text txtCoins;
    public static TMP_Text txtKeys;
    public static TMP_Text txtBombs;


    public static List<string> Specials = new List<string>();
    public static float Speed = 40f;
    public static float KnockBack = 20;
    public static float EnemyKnockBack = 10;
    public static float Damage = 1;
    public static float DamageBlockedByArmor = 0;
    public static float Armor = 0;
    public static float AttackSize = 1;
    public static float DamageMultiplier = 1;
    public static float AttackSpeed = 4;
    public static float ShotSpeed = 0;

    public static float Health = 3f;
    public static float MaxHealth = 3f;

    public static GameObject Attack;
    public static GameObject AttackExplosion;
    public static GameObject AttackDecal;

    public static Transform StaffBone;

    public static LinkedList<GameObject> Bombs = new LinkedList<GameObject>();
    public static GameObject Bomb;
    public static GameObject BombExplosion;

    public static List<Material> materials = new List<Material>();

    public static GameObject HeartPanel;
    public static GameObject DamagePanel;
    public static Sprite FullHeart;
    public static Sprite HalfHeart;
    public static Sprite EmptyHeart;
    public static RuntimeAnimatorController HeartAnimator;
    

    public static Camera Camera;
    public static Camera FallingCamera;
    public static RenderTexture RT;
    public static GameObject PixelatedPanel;

    public static Room CurrentRoom;

    public static CharacterController Controller;

}
