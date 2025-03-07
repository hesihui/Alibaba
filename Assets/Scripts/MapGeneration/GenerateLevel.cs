using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GenerateLevel : MonoBehaviour
{

    public string StartRoomType = "StartRooms";
    public string StartRoomNumber = "0";

    public Sprite CurrentRoom;
    public Sprite BRoom;
    public Sprite EmptyRoom;
    public Sprite ShopRoom;
    public Sprite TreasureRoom;
    public Sprite UnexploredRoom;
    public Sprite SecretRoom;

    public GameObject DebugCheatMenu;

    private bool BossRoomGenerated = false;


    void DrawRoomOnMap(Room R)
    {
        if (R.AnchorRoom)
        {
            string TileName = "MapTile";
            if (R.RoomNumber == 1) TileName = "BossRoomTile";
            if (R.RoomNumber == 2) TileName = "ShopRoomTile";
            if (R.RoomNumber == 3) TileName = "ItemRoomTile";
            if (R.RoomType == "2x2Rooms") TileName = "2x2Room";
            GameObject MapTile = new GameObject(TileName);
            Image RoomImage = MapTile.AddComponent<Image>();
            RoomImage.sprite = R.RoomSprite;
            R.RoomImage = RoomImage;
            RectTransform rectTransform = RoomImage.GetComponent<RectTransform>();
            if (R.RoomType == "2x2Rooms")
            {

                rectTransform.sizeDelta = (new Vector2(Level.Height * 2, Level.Width * 2) * Level.IconScale + 
                    new Vector2(Level.padding * Level.Height, Level.padding * Level.Height)) * Level.Scale;
               
                float offset = (((Level.Height * Level.IconScale) + ((Level.padding * Level.Height))) /2) * Level.Scale ;


                rectTransform.position = ((R.Location * (Level.IconScale * Level.Height + (Level.padding * Level.Height * Level.Scale))) + 
                    new Vector2(offset, offset))*Level.Scale;
            }
            else
            {
                rectTransform.sizeDelta = new Vector2(Level.Height, Level.Width) * Level.IconScale * Level.Scale;
                rectTransform.position = (R.Location * (Level.IconScale * Level.Height + (Level.padding * Level.Height))) * Level.Scale ;
            }


            RoomImage.transform.SetParent(transform, false);
        }
        Level.Rooms.Add(R);
       // Debug.Log("Drawing Room:" + R.RoomNumber + " at location:" + R.Location);
    }


    int RandomRoomNumber()
    {
        return Random.Range(6,GameObject.Find("Rooms").transform.childCount+6);
    }


    bool CheckIfRoomExists(Vector2 v)
    {
        return (Level.Rooms.Exists(x => x.Location == v));
    }


    bool CheckIfRoomsAroundGeneratedRoom(Vector2 v, string direction)
    {

        switch (direction)
        {
            case "Right":
                {
                    //Check Down,left,and up
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(v.x - 1, v.y)) ||
                       Level.Rooms.Exists(x => x.Location == new Vector2(v.x, v.y - 1)) ||
                       Level.Rooms.Exists(x => x.Location == new Vector2(v.x, v.y + 1)))
                        return true;
                    break;
                }
            case "Left":
                {
                    //Check Down,Right,and up
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(v.x + 1, v.y)) ||
                       Level.Rooms.Exists(x => x.Location == new Vector2(v.x, v.y - 1)) ||
                       Level.Rooms.Exists(x => x.Location == new Vector2(v.x, v.y + 1)))
                        return true;
                    break;
                }
            case "Up":
                {
                    //Check Down,Right,and Left
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(v.x + 1, v.y)) ||
                       Level.Rooms.Exists(x => x.Location == new Vector2(v.x -1, v.y)) ||
                       Level.Rooms.Exists(x => x.Location == new Vector2(v.x, v.y - 1)))
                        return true;
                    break;
                }
            case "Down":
                {
   
                    if (Level.Rooms.Exists(x => x.Location == new Vector2(v.x, v.y+1)) ||
                       Level.Rooms.Exists(x => x.Location == new Vector2(v.x -1, v.y)) ||
                       Level.Rooms.Exists(x => x.Location == new Vector2(v.x + 1, v.y)))
                        return true;
                    break;
                }

        }



        return false;
    }



    int failsafe = 0;


    void Generate(Room room)
    {
        failsafe++;
        if (failsafe > 50)
        {
            return;
        }

        DrawRoomOnMap(room);


        //Left
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(-1, 0) + room.Location;
            newRoom.RoomSprite = Level.DefaultRoomIcon;
            newRoom.RoomNumber = RandomRoomNumber();

            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Right"))
                {
                    if (Mathf.Abs(newRoom.Location.x) < Level.RoomLimit && Mathf.Abs(newRoom.Location.y) < Level.RoomLimit)
                    {
                   
                        Generate(newRoom);
                    }

                }
            }
        }

        //Right
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(1, 0) + room.Location;
            newRoom.RoomSprite = Level.DefaultRoomIcon;
            newRoom.RoomNumber = RandomRoomNumber();
            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Left"))
                {
                    if (Mathf.Abs(newRoom.Location.x) < Level.RoomLimit && Mathf.Abs(newRoom.Location.y) < Level.RoomLimit)
                    {
          
                        Generate(newRoom);
                    }
                }
            }
        }

        //Up
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(0, 1) + room.Location;
            newRoom.RoomSprite = Level.DefaultRoomIcon;
            newRoom.RoomNumber = RandomRoomNumber();
            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Down"))
                {
                    if (Mathf.Abs(newRoom.Location.x) < Level.RoomLimit && Mathf.Abs(newRoom.Location.y) < Level.RoomLimit)
                    {
         
                        Generate(newRoom);
                    }
                }
            }
        }
        //Down
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(0, -1) + room.Location;
            newRoom.RoomSprite = Level.DefaultRoomIcon;
            newRoom.RoomNumber = RandomRoomNumber();
            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Up"))
                {
                    if (Mathf.Abs(newRoom.Location.x) < Level.RoomLimit && Mathf.Abs(newRoom.Location.y) < Level.RoomLimit)
                    {
          
                        Generate(newRoom);
                    }
                }
            }
        }


        
    }

    private void GenerateBossRoom()
    {
        float MaxNumber = 0;
        Vector2 FarthestRoom = Vector2.zero;

        foreach(Room R in Level.Rooms)
        {
            if(Mathf.Abs(R.Location.x) + Mathf.Abs(R.Location.y) >= MaxNumber)
            {
                MaxNumber = Mathf.Abs(R.Location.x) + Mathf.Abs(R.Location.y);
                FarthestRoom = R.Location;
            }
            

        }

  
        Room BossRoom = new Room();
        BossRoom.RoomSprite = Level.BossRoomIcon;
        BossRoom.RoomType = "BossRooms";
        BossRoom.RoomNumber = Random.Range(0,GameObject.Find("BossRooms").transform.childCount);
        BossRoom.SpecialRoom = true;
        //Left
        if (!CheckIfRoomExists(FarthestRoom + new Vector2(-1, 0)))
        {
            if (!CheckIfRoomsAroundGeneratedRoom(FarthestRoom + new Vector2(-1, 0), "Right"))
            {
                BossRoom.Location = FarthestRoom + new Vector2(-1, 0);
            }
        }

        //Right
        if (!CheckIfRoomExists(FarthestRoom + new Vector2(1, 0)))
        {
            if (!CheckIfRoomsAroundGeneratedRoom(FarthestRoom + new Vector2(1, 0), "Left"))
            {
                BossRoom.Location = FarthestRoom + new Vector2(1, 0);
            }
        }

        //Up
        if (!CheckIfRoomExists(FarthestRoom + new Vector2(0, 1)))
        {
            if (!CheckIfRoomsAroundGeneratedRoom(FarthestRoom + new Vector2(0, 1), "Down"))
            {
                BossRoom.Location = FarthestRoom + new Vector2(0, 1);
            }
        }
        //Down
        if (!CheckIfRoomExists(FarthestRoom + new Vector2(0, -1)))
        {
            if (!CheckIfRoomsAroundGeneratedRoom(FarthestRoom + new Vector2(0, -1), "Up"))
            {
                BossRoom.Location = FarthestRoom + new Vector2(0, -1);
            }
        }

        DrawRoomOnMap(BossRoom);

    }

    void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        System.Random rng = new System.Random();

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

    }

    private bool GenerateSpecialRoom(Sprite MapIcon, string RoomType)
    {
        List<Room> ShuffledList = new List<Room>(Level.Rooms);
        ShuffleList(ShuffledList);

        Room SpecialRoom = new Room();
        SpecialRoom.RoomSprite = MapIcon;
        SpecialRoom.RoomNumber = Random.Range(0, GameObject.Find(RoomType).transform.childCount);
        SpecialRoom.RoomType = RoomType;
        SpecialRoom.SpecialRoom = true;

        bool FoundAvailableLocation = false;

        foreach (Room R in ShuffledList)
        {
            Vector2 SpecialRoomLocation = R.Location;


            if (R.RoomNumber < 6) continue;


            //Left
            if (!CheckIfRoomExists(SpecialRoomLocation + new Vector2(-1, 0)))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(SpecialRoomLocation + new Vector2(-1, 0), "Right"))
                {
                    SpecialRoom.Location = SpecialRoomLocation + new Vector2(-1, 0);
                    FoundAvailableLocation = true;
                }
            }

            //Right
            else if (!CheckIfRoomExists(SpecialRoomLocation + new Vector2(1, 0)))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(SpecialRoomLocation + new Vector2(1, 0), "Left"))
                {
                    SpecialRoom.Location = SpecialRoomLocation + new Vector2(1, 0);
                    FoundAvailableLocation = true;
                }
            }

            //Up
            else if (!CheckIfRoomExists(SpecialRoomLocation + new Vector2(0, 1)))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(SpecialRoomLocation + new Vector2(0, 1), "Down"))
                {
                    SpecialRoom.Location = SpecialRoomLocation + new Vector2(0, 1);
                    FoundAvailableLocation = true;
                }
            }
            //Down
            else if (!CheckIfRoomExists(SpecialRoomLocation + new Vector2(0, -1)))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(SpecialRoomLocation + new Vector2(0, -1), "Up"))
                {
                    SpecialRoom.Location = SpecialRoomLocation + new Vector2(0, -1);
                    FoundAvailableLocation = true;
                }
            }

            if (FoundAvailableLocation) { 
                DrawRoomOnMap(SpecialRoom);
                return true;
            }

        }

        return false;

    }

    private bool GenerateSecretRoom()
    {
        List<Room> ShuffledList = new List<Room>(Level.Rooms);
        ShuffleList(ShuffledList);

        foreach (Room R in ShuffledList)
        {
            // x and y < 3 and > -3 starting room is at 0,0
            if (Mathf.Abs(R.Location.x) > 2 || Mathf.Abs(R.Location.y) > 2 || R.Location == Vector2.zero)
            {
                continue;
            }

            // Define the directions
            Vector2[] directions = { new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, -1) };

            foreach (Vector2 direction in directions)
            {
                Vector2 newLocation = R.Location + direction;

                // Check if a room already exists at the new location
                if (!Level.Rooms.Exists(x => x.Location == newLocation))
                {
                    if (Mathf.Abs(newLocation.x) > 1 || Mathf.Abs(newLocation.y) >1) //Prevents it from being drawn next to the start room.
                    {
                        CreateNewRoom(newLocation);
                        return true;
                    }
                }
            }
        }


        return false;
    }


    //Used for Secret Room
    void CreateNewRoom(Vector2 location)
    {
        Room SR = new Room
        {
            Location = location,
            RoomSprite = Level.SecretRoom,
            Explored = false,
            Revealed = false,
            SpecialRoom = true,
            RoomNumber = 4
        };

        DrawRoomOnMap(SR);
    }

    private void Awake()
    {
        Level.DefaultRoomIcon = EmptyRoom;
        Level.BossRoomIcon = BRoom;
        Level.CurrentRoomIcon = CurrentRoom;
        Level.ShopRoomIcon = ShopRoom;
        Level.TreasureRoomIcon = TreasureRoom;
        Level.UnexploredRoom = UnexploredRoom;
        Level.SecretRoom = SecretRoom;
    }

    int maxtries = 0;

    void Start()
    {

        maxtries++;

        Room StartRoom = new Room();
        StartRoom.Location = new Vector2(0, 0);
        StartRoom.RoomSprite = Level.DefaultRoomIcon;
        StartRoom.Explored = true;
        StartRoom.Revealed = true;
        StartRoom.SpecialRoom = true;
        StartRoom.RoomNumber = Random.Range(0,GameObject.Find("StartRooms").transform.childCount);

       

        StartRoom.RoomType = "StartRooms";

        Player.CurrentRoom = StartRoom;

        //Drawing the starting room
        DrawRoomOnMap(StartRoom);

        //Left
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(-1, 0);
            newRoom.RoomSprite = Level.DefaultRoomIcon;
            newRoom.RoomNumber = RandomRoomNumber();
            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Right"))
                    Generate(newRoom);
            } 
        }

        //Right
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(1, 0);
            newRoom.RoomSprite = Level.DefaultRoomIcon;
            newRoom.RoomNumber = RandomRoomNumber();
            if (!CheckIfRoomExists(newRoom.Location))
            {
                if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Left"))
                    Generate(newRoom);
            }
        }

        //Up
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(0, 1);
            newRoom.RoomSprite = Level.DefaultRoomIcon;
            newRoom.RoomNumber = RandomRoomNumber();
            if (!CheckIfRoomExists(newRoom.Location))
                {
                    if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Down"))
                        Generate(newRoom);
                }
        }
        //Down
        if (Random.value > Level.RoomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.Location = new Vector2(0, -1);
            newRoom.RoomSprite = Level.DefaultRoomIcon;
            newRoom.RoomNumber = RandomRoomNumber();
            if (!CheckIfRoomExists(newRoom.Location))
                    {
                        if (!CheckIfRoomsAroundGeneratedRoom(newRoom.Location, "Up"))
                            Generate(newRoom);
                    }
        }

        GenerateBossRoom();

       bool treasure = GenerateSpecialRoom(Level.TreasureRoomIcon, "ItemRooms");
       bool shop = GenerateSpecialRoom(Level.ShopRoomIcon, "ShopRooms");
        //bool secret = GenerateSecretRoom();

        if (!treasure || !shop)
        {
            if (maxtries > 30)
            {
                Debug.Log("The maximum number of tries was hit. Aborting map generation");
                return;
            }

            Regenerate();

        }
        else
        {

            UpdateRooms();

            ChangeRooms.RevealRooms(StartRoom);
            ChangeRooms.RedrawRevealedRooms();

            //only enable the start room after the regenerations have finished.
            if (StartRoomType == "") StartRoomType = "StartRooms";

            Transform StartRooms = GameObject.Find(StartRoomType).transform;
            if (StartRoomNumber == "") StartRooms.Find(StartRoom.RoomNumber.ToString()).gameObject.SetActive(true);
            else
            {
               GameObject t = StartRooms.Find(StartRoomNumber).gameObject;
                t.SetActive(true);
                Transform enemiesTransform = t.transform.Find("Enemies");

                if (enemiesTransform != null)
                {
                    int childCount = enemiesTransform.childCount;
                    Level.EnemyCount = childCount;

                }
             
            }



        }

    }

    public void UpdateRooms()
    {
        try
        {
           

            List<Room> RoomsToReplace = new List<Room>();



            foreach(Room room in Level.Rooms)
            {
                if (room.SpecialRoom || room.RoomType == "2x2Rooms") continue;

                bool Above = Level.Rooms.Exists(x => x.Location == room.Location + new Vector2(0, 1) && x.SpecialRoom);
                bool Right = Level.Rooms.Exists(x => x.Location == room.Location + new Vector2(1, 0) && x.SpecialRoom);
                bool RightCornerwise = Level.Rooms.Exists(x => x.Location == room.Location + new Vector2(1, 1) && x.SpecialRoom);

                if ((!Above && !Right && !RightCornerwise))
                {
                    if(Random.value > .7f) //30% chance
                    {
                        
                        RoomsToReplace.Add(room);


                    }
                }
            }

            foreach(Room room in RoomsToReplace)
            {

                {
                    Room matchingroom = Level.Rooms.FirstOrDefault(x => x.Location == room.Location);
                    if (matchingroom != null && matchingroom.RoomType == "2x2Rooms") continue;
                }
                {
                    Room matchingroom = Level.Rooms.FirstOrDefault(x => x.Location == room.Location + new Vector2(0, 1));
                    if (matchingroom != null && matchingroom.RoomType == "2x2Rooms") continue;
                }
                {
                    Room matchingroom = Level.Rooms.FirstOrDefault(x => x.Location == room.Location + new Vector2(1, 0));
                    if (matchingroom != null && matchingroom.RoomType == "2x2Rooms") continue;
                }
                {
                    Room matchingroom = Level.Rooms.FirstOrDefault(x => x.Location == room.Location + new Vector2(1, 1));
                    if (matchingroom != null && matchingroom.RoomType == "2x2Rooms") continue;
                }

                //check all 8 directions for the boss room
                if (Level.Rooms.Exists(x => x.Location == room.Location + new Vector2(-1, 1) && (x.RoomType == "BossRooms" || x.RoomType == "2x2Rooms"))) continue;
                if (Level.Rooms.Exists(x => x.Location == room.Location + new Vector2(-1, 0) && (x.RoomType == "BossRooms" || x.RoomType == "2x2Rooms"))) continue;
                if (Level.Rooms.Exists(x => x.Location == room.Location + new Vector2(0, 2) && (x.RoomType == "BossRooms" || x.RoomType == "2x2Rooms"))) continue;
                if (Level.Rooms.Exists(x => x.Location == room.Location + new Vector2(1, 2) && (x.RoomType == "BossRooms" || x.RoomType == "2x2Rooms"))) continue;
                if (Level.Rooms.Exists(x => x.Location == room.Location + new Vector2(2, 1) && (x.RoomType == "BossRooms" || x.RoomType == "2x2Rooms"))) continue;
                if (Level.Rooms.Exists(x => x.Location == room.Location + new Vector2(2, 0) && (x.RoomType == "BossRooms" || x.RoomType == "2x2Rooms"))) continue;
                if (Level.Rooms.Exists(x => x.Location == room.Location + new Vector2(0, -1) && (x.RoomType == "BossRooms" || x.RoomType == "2x2Rooms"))) continue;
                if (Level.Rooms.Exists(x => x.Location == room.Location + new Vector2(1, -1) && (x.RoomType == "BossRooms" || x.RoomType == "2x2Rooms"))) continue;




                {
                    Room r = Level.Rooms.FirstOrDefault(x => x.Location == room.Location);
                    if (r != null) { r.RoomImage.color = new Color(0, 0, 0, 0); }
                }
                {
                    Room r = Level.Rooms.FirstOrDefault(x => x.Location == room.Location + new Vector2(0, 1));
                    if (r != null) { r.RoomImage.color = new Color(0, 0, 0, 0); }
                }
                {
                    Room r = Level.Rooms.FirstOrDefault(x => x.Location == room.Location + new Vector2(1, 0));
                    if (r != null) { r.RoomImage.color = new Color(0, 0, 0, 0); }
                }
                {
                    Room r = Level.Rooms.FirstOrDefault(x => x.Location == room.Location + new Vector2(1, 1));
                    if (r != null) { r.RoomImage.color = new Color(0, 0, 0, 0); }
                }


                Level.Rooms.RemoveAll(x => x.Location == room.Location);
                Level.Rooms.RemoveAll(x => x.Location == room.Location + new Vector2(0, 1));
                Level.Rooms.RemoveAll(x => x.Location == room.Location + new Vector2(1, 0));
                Level.Rooms.RemoveAll(x => x.Location == room.Location + new Vector2(1, 1));


                int roomNumber = Random.Range(0, GameObject.Find("2x2Rooms").transform.childCount);

                Room AnchRoom;

                {
                    Room R = new Room();
                    R.Location = room.Location;
                    R.RoomType = "2x2Rooms";
                    R.RoomNumber = roomNumber;
                    R.RoomSprite = Level.DefaultRoomIcon;
                    R.AnchorR = R;
                    AnchRoom = R;
                    DrawRoomOnMap(R);
                }
                {
                    Room R = new Room();
                    R.Location = room.Location + new Vector2(0, 1);
                    R.RoomType = "2x2Rooms";
                    R.RoomNumber = roomNumber;
                    R.RoomSprite = Level.DefaultRoomIcon;
                    R.AnchorRoom = false;
                    R.AnchorR = AnchRoom;
                    DrawRoomOnMap(R);
                }
                {
                    Room R = new Room();
                    R.Location = room.Location + new Vector2(1, 0);
                    R.RoomType = "2x2Rooms";
                    R.RoomNumber = roomNumber;
                    R.RoomSprite = Level.DefaultRoomIcon;
                    R.AnchorRoom = false;
                    R.AnchorR = AnchRoom;
                    DrawRoomOnMap(R);
                }
                {
                    Room R = new Room();
                    R.Location = room.Location + new Vector2(1, 1);
                    R.RoomType = "2x2Rooms";
                    R.RoomNumber = roomNumber;
                    R.TopRight = true;
                    R.RoomSprite = Level.DefaultRoomIcon;
                    R.AnchorRoom = false;
                    R.AnchorR = AnchRoom;
                    DrawRoomOnMap(R);
                }


            }


            foreach (Room R in Level.Rooms)
            {
                if (R.RoomType != "Rooms") continue;

                bool top = Level.Rooms.Exists(x => x.Location == R.Location + new Vector2(0, 1));
                bool bottom = Level.Rooms.Exists(x => x.Location == R.Location + new Vector2(0, -1));
                bool left = Level.Rooms.Exists(x => x.Location == R.Location + new Vector2(-1, 0));
                bool right = Level.Rooms.Exists(x => x.Location == R.Location + new Vector2(1, 0));

                if (top && bottom && !left && !right)
                {
                    R.RoomType = "TopBottomRooms";
                    R.RoomNumber = Random.Range(0, GameObject.Find("TopBottomRooms").transform.childCount);
                }

                else if (!top && !bottom && left && right)
                {
                    R.RoomType = "LeftRightRooms";
                    R.RoomNumber = Random.Range(0, GameObject.Find("LeftRightRooms").transform.childCount);
                }

            }

        }
        catch(Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
   


    bool regenerating = false;
    void StopRegenerating()
    {
        regenerating = false;
    }

   public void Regenerate()
    {
        regenerating = true;
        failsafe = 0;
        Level.Rooms.Clear();
        Invoke(nameof(StopRegenerating), 1);
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            Destroy(child.gameObject);
        }


        Start();

    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Tab) && !regenerating)
        {

            maxtries = 0;
            Transform Doors = GameObject.Find("StartRooms").transform.Find(Player.CurrentRoom.RoomNumber.ToString()).transform.Find("Doors");
            for (int i = 0; i < Doors.childCount; i++)
            {
                Doors.GetChild(i).gameObject.SetActive(false);
            }
            Transform StartRooms = GameObject.Find("StartRooms").transform;
            for (int i = 0; i < StartRooms.childCount; i++)
            {
                StartRooms.GetChild(i).gameObject.SetActive(false);
            }
            Regenerate();


            if (Level.Rooms.Exists(x => x.Location == new Vector2(1,0)))
            {
                Doors.Find("RightDoor").gameObject.SetActive(true);
            }
            if (Level.Rooms.Exists(x => x.Location == new Vector2(-1, 0)))
            {
                Doors.Find("LeftDoor").gameObject.SetActive(true);
            }
            if (Level.Rooms.Exists(x => x.Location == new Vector2(0,1)))
            {
                Doors.Find("TopDoor").gameObject.SetActive(true);
            }
            if (Level.Rooms.Exists(x => x.Location == new Vector2(0,-1)))
            {
                Doors.Find("BottomDoor").gameObject.SetActive(true);
            }
        }

        if (Input.GetKey(KeyCode.P) && !regenerating)
        {
            regenerating = true;
            Invoke(nameof(StopRegenerating), 1);

            string log = "Room List:\n-----------------\n";
     
            foreach (Room R in Level.Rooms)
            {
                if(R.AnchorRoom)
                log += "Room Type:" + R.RoomType
                    + " Room#:" + R.RoomNumber 
                    + " Location: " + R.Location
                    + " Revealed: " + R.Revealed.ToString()
                    + " Explored: " + R.Explored.ToString()
                    + "\n";
            }
            Debug.Log(log);

            Debug.Log(Player.State);
            
        }

        if(Input.GetKey(KeyCode.M) && !regenerating)
        {
            regenerating = true;
            Invoke(nameof(StopRegenerating), 1);

            foreach(Room R in Level.Rooms)
            {
                R.Revealed = true;
                R.Explored = true;
                ChangeRooms.RedrawRevealedRooms();
            }

        }

        if (Input.GetKey(KeyCode.O) && !regenerating)
        {
            regenerating = true;
            Invoke(nameof(StopRegenerating), 1);
            if (DebugCheatMenu.activeInHierarchy == true) DebugCheatMenu.SetActive(false);
            else DebugCheatMenu.SetActive(true);
        }

        if(Input.GetKey(KeyCode.I) && !regenerating)
        {
            regenerating = true;
            Invoke(nameof(StopRegenerating), 1);

            foreach (Room R in Level.Rooms)
            {
                if(R.RoomType == "ItemRooms")
                {
                    Transform T2 = GameObject.Find(Player.CurrentRoom.RoomType.ToString()).transform;
                    T2.Find(Player.CurrentRoom.RoomNumber.ToString()).gameObject.SetActive(false);

                    GameObject.Find("ItemRooms").transform.Find(R.RoomNumber.ToString()).gameObject.SetActive(true);

                    Player.CurrentRoom = R;
                }
            }
        }

        if (Input.GetKey(KeyCode.R) && !regenerating)
        {
            regenerating = true;
            Invoke(nameof(StopRegenerating), 1);


            Player.transform.GetComponent<InitalizePlayer>().InitializeItemRoomItem();
          
        }
        

    }

}
