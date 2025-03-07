using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Progress;

public static class Level
{
    public static bool AlreadyStarted = false;

    public static float Height = 500;
    public static float Width = 500;

    public static float Scale = 1f;
    public static float IconScale = .1f;
    public static float padding = .01f;

    public static float RoomGenerationChance = .5f;

    public static int RoomLimit = 4;

    public static Sprite TreasureRoomIcon;
    public static Sprite BossRoomIcon;
    public static Sprite ShopRoomIcon;
    public static Sprite UnexploredRoom;
    public static Sprite DefaultRoomIcon;
    public static Sprite CurrentRoomIcon;
    public static Sprite SecretRoom;

    public static GameObject SecretRoomExplosion;
    public static GameObject SecretRoomDoor;
    public static bool SecretRoomExploded = false;
    public static GameObject XMark;

    public static List<Room> Rooms = new List<Room>();

    public static float RoomChangeTime = .1f;

    public static int EnemyCount = 0;

   public static void LoadNextLevel()
    {
        EnemyCount = 0;
        Rooms.Clear();
        ImpactReceiver.forcesOnGameObjects.Clear();


        SceneManager.sceneLoaded += InitalizePlayer.OnSceneLoaded;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);

    }

}

public class Room
{
    public int RoomNumber = 6;
    public Vector2 Location;
    public Image RoomImage;
    public Sprite RoomSprite;
    public bool Revealed = false;
    public bool Explored = false;
    public bool Cleared = true;
    public string RoomType = "Rooms";
    public bool AnchorRoom = true;
    public Room AnchorR;
    public bool TopRight = false;
    public bool SpecialRoom = false;

    
}
