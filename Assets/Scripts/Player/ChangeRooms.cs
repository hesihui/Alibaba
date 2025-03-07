using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public class ChangeRooms : MonoBehaviour
{
    public Transform Rooms;
    public GameObject BossDoor;
    public float RoomSpawnOffset = 10;
    public Material BossFireMaterial;
    public FullScreenPassRendererFeature FSPRF;
    private Sprite previousImage;

    private void Start()
    {
        previousImage = Level.DefaultRoomIcon;
        EnableDoors(Player.CurrentRoom);
    }

    UnityEngine.Color CurrentRoomGreen = new UnityEngine.Color(.5f, 1, .5f);
    UnityEngine.Color NormalColor = new UnityEngine.Color(1f, 1, 1f);
    void ChangeRoomIcon(Room CurrentRoom, Room NewRoom)
    {
        
        if(CurrentRoom.AnchorRoom)
        {
            //CurrentRoom.RoomImage.sprite = previousImage;
            CurrentRoom.RoomImage.color = NormalColor;
        }
        else
        {
            //CurrentRoom.AnchorR.RoomImage.sprite = previousImage;
            CurrentRoom.AnchorR.RoomImage.color = NormalColor;
        }

        if (NewRoom.AnchorRoom)
        {
            //previousImage = NewRoom.RoomImage.sprite;
            //NewRoom.RoomImage.sprite = Level.CurrentRoomIcon;
            NewRoom.RoomImage.color = CurrentRoomGreen;
        }
        else
        {
            //previousImage = NewRoom.AnchorR.RoomImage.sprite;
            //NewRoom.AnchorR.RoomImage.sprite = Level.CurrentRoomIcon;
            NewRoom.AnchorR.RoomImage.color = CurrentRoomGreen;
        }
        
    }

    bool changeroomcooldown = false;

    void EndChangeRoomCooldown()
    {
        changeroomcooldown = false;
    }


    public static void RedrawRevealedRooms()
    {
        try
        {
            foreach (Room room in Level.Rooms)
            {
                if (!room.AnchorRoom) continue;

                if (!room.Revealed && !room.Explored) room.RoomImage.color = new UnityEngine.Color(1, 1, 1, 0);
                if (room.Revealed && !room.Explored && !room.SpecialRoom) room.RoomImage.sprite = Level.UnexploredRoom;
                if (room.Explored && !room.SpecialRoom) room.RoomImage.sprite = Level.DefaultRoomIcon;
                if (room.Explored || room.Revealed) room.RoomImage.color = new UnityEngine.Color(1, 1, 1, 1);
                if (Player.CurrentRoom.AnchorRoom)
                {
                    Player.CurrentRoom.RoomImage.color = new UnityEngine.Color(.5f,1,.5f);
                }
                else
                {
                    Player.CurrentRoom.AnchorR.RoomImage.color = new UnityEngine.Color(.5f, 1, .5f);
                }

            }
        }catch(System.Exception ex)
        {
            Debug.Log(ex.Message);
        }

    }

    public static void RevealRooms(Room R)
    {
        if (R.AnchorRoom && R.RoomType != "2x2Rooms")
        {
            foreach (Room room in Level.Rooms)
            {
                // Check surrounding rooms for AnchorRoom
                if (room.Location == R.Location + new Vector2(-1, 0))
                {
                    if (room.AnchorRoom) room.Revealed = true;
                    else room.AnchorR.Revealed = true;
                }
                if (room.Location == R.Location + new Vector2(1, 0) )
                {
                    if (room.AnchorRoom) room.Revealed = true;
                    else room.AnchorR.Revealed = true;
                }
                if (room.Location == R.Location + new Vector2(0, -1))
                {
                    if (room.AnchorRoom) room.Revealed = true;
                    else room.AnchorR.Revealed = true;
                }
                if (room.Location == R.Location + new Vector2(0, 1))
                {
                    if (room.AnchorRoom) room.Revealed = true;
                    else room.AnchorR.Revealed = true;
                }
            }
        }
        else if (R.RoomType == "2x2Rooms")
        {
            foreach (Room room in Level.Rooms)
            {
                // Offsets for the neighboring rooms
                Vector2[] offsets = new Vector2[]
                {
                new Vector2(-1, 1), //left up
                new Vector2(-1, 0), //left down
                new Vector2(0, 2),  //up left
                new Vector2(1, 2),  //up right
                new Vector2(2, 1),  //right up
                new Vector2(2, 0),  //right down
                new Vector2(0, -1), //down left
                new Vector2(1, -1)  //down right
                };

                foreach (var offset in offsets)
                {
                    if (room.Location == R.AnchorR.Location + offset)
                    {
                        if (room.AnchorRoom) room.Revealed = true;
                        else room.AnchorR.Revealed = true;
                    }
                }

                // Ensure the center rooms are revealed as well
                Vector2[] centerOffsets = new Vector2[]
                {
                new Vector2(0, 0),  // center
                new Vector2(1, 0),  // right center
                new Vector2(0, 1),  // top center
                new Vector2(1, 1)   // top right center
                };

                foreach (var offset in centerOffsets)
                {
                    if (room.Location == R.AnchorR.Location + offset)
                    {
                        if (room.AnchorRoom) room.Revealed = true;
                        else room.AnchorR.Revealed = true;
                    }
                }
            }
        }
    }


    void CheckDoor(Vector2 NewLocation, string Direction, Vector3 RoomOffset)
    {
        Player.SoundEffectSource.Stop();
        //clear the drops from the room.
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Drops"))
        {
            GameObject.Destroy(g);
        }

        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Attacks"))
        {
            GameObject.Destroy(g);
        }  

        Vector2 Location = Player.CurrentRoom.Location;
        //where are we?
        if (!Player.CurrentRoom.AnchorRoom)
            Location = Player.CurrentRoom.AnchorR.Location;

            //where are we going?
            Location = Location + NewLocation;

        if (Level.Rooms.Exists(x => x.Location == Location))
        {
            Room R = Level.Rooms.First(x => x.Location == Location);

            //disable the room that you're in
            Transform T2 = GameObject.Find(Player.CurrentRoom.RoomType.ToString()).transform;
            T2.Find(Player.CurrentRoom.RoomNumber.ToString()).gameObject.SetActive(false);


            //Find the new room and activate it
            Transform T = GameObject.Find(R.RoomType.ToString()).transform;
            GameObject NewRoom = T.Find(R.RoomNumber.ToString()).gameObject;
            NewRoom.SetActive(true);

            //Move the player to the door area where he would be coming through
            Player.Controller.enabled = false;

            string door = "Normal";
            if (R.RoomType == "2x2Rooms")
            {

                //compare Player.CurrentRoom to R.Room to find which door we need to be put in front of

                

                if (R.Location.x == Player.CurrentRoom.Location.x
                    && R.Location.y == Player.CurrentRoom.Location.y +1
                    && R.AnchorRoom == true)
                {
                    door = "BottomLeftDoor";
                }
                if (R.Location.x == Player.CurrentRoom.Location.x
                    && R.Location.y == Player.CurrentRoom.Location.y + 1
                    && R.AnchorRoom == false)
                {
                    door = "BottomRightDoor";
                }

                if (R.Location.y == Player.CurrentRoom.Location.y
                   && R.Location.x == Player.CurrentRoom.Location.x + 1
                   && R.AnchorRoom == true)
                {
                    door = "LeftBottomDoor";
                }

                if (R.Location.y == Player.CurrentRoom.Location.y
                  && R.Location.x == Player.CurrentRoom.Location.x + 1
                  && R.AnchorRoom == false)
                {
                    door = "LeftTopDoor";
                }

                if (R.Location.x == Player.CurrentRoom.Location.x
                 && R.Location.y == Player.CurrentRoom.Location.y - 1
                 && R.TopRight)
                {
                    door = "TopRightDoor";
                }
                if (R.Location.x == Player.CurrentRoom.Location.x
                && R.Location.y == Player.CurrentRoom.Location.y - 1
                && !R.TopRight)
                {
                    door = "TopLeftDoor";
                }
                if (R.Location.y == Player.CurrentRoom.Location.y
               && R.Location.x == Player.CurrentRoom.Location.x - 1
               && R.TopRight)
                {
                    door = "RightTopDoor";
                }
                if (R.Location.y == Player.CurrentRoom.Location.y
               && R.Location.x == Player.CurrentRoom.Location.x - 1
               && !R.TopRight)
                {
                    door = "RightBottomDoor";
                }

                transform.position = NewRoom.transform.Find("Doors").transform.Find(door).position + RoomOffset;
            }
            else if(R.RoomType == "BossRooms")
            {
                transform.position = NewRoom.transform.Find("BossSpawnSpot").position;
            }
            else
            {
                transform.position = NewRoom.transform.Find("Doors").transform.Find(Direction).position + RoomOffset;
            }
            Player.Controller.enabled = true;

            ChangeRoomIcon(Player.CurrentRoom, R);

            Player.CurrentRoom = R;

            EnableDoors(R,door);

            Player.CurrentRoom.Explored = true;
            if(Player.CurrentRoom.AnchorRoom == false) Player.CurrentRoom.AnchorR.Explored = true;

            RevealRooms(R);
            RedrawRevealedRooms();


            Transform Enemies = NewRoom.transform.Find("Enemies");
            if (Enemies != null)
            {
                Player.CurrentRoom.Cleared = false;
                Level.EnemyCount = Enemies.childCount;
            }
            else
            {
                Level.EnemyCount = 0;


                Transform Doors = NewRoom.transform.Find("Doors");
                GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Door");

                Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/LevelMusic/1/LatchDoor"), 3);

                Animator A;
                foreach(GameObject g in objectsWithTag)
                {
                    if(g.TryGetComponent<Animator>(out A))
                    A.enabled = true;
                }

            }


        }



        //lastly redraw the A* grid
        GameObject.Find("A*").GetComponent<Grid>().CreateGrid();

    }

    

    bool CheckIfRoomAlreadyHasX(Transform T)
    {
        for (int i = 0; i < T.childCount; ++i)
        {
            if (T.GetChild(i).name.Contains("DrawX"))
            {
                return true;
            }
        }

        return false;
    }

    public void EnableDoors(Room R, string door = "Normal")
    {
            Transform T2 = GameObject.Find(Player.CurrentRoom.RoomType.ToString()).transform;
            Transform T = T2.Find(R.RoomNumber.ToString());
            Transform Doors = T.Find("Doors");

            Debug.Log("Opening Doors: Room Type: " + T2.ToString()) ;
            Debug.Log("Room Number: " + T2.ToString());


            for (int i = 0; i < Doors.childCount; i++)
            {
                Doors.GetChild(i).gameObject.SetActive(false);
            }

           

        if (R.RoomType == "2x2Rooms")
        {
            Vector2 curpos = R.AnchorR.Location;
            {
                if(Level.Rooms.Exists(x => x.Location == curpos + new Vector2(-1,1)))
                {
                    if (Level.Rooms.First(x => x.Location == curpos + new Vector2(-1, 1)).RoomType == "BossRooms")
                    {
                        Instantiate(BossDoor, Doors.Find("LeftTopDoor").position, Doors.Find("LeftTopDoor").rotation * Quaternion.Euler(-90,0,0));
                    }
                    else
                    {
                        Doors.Find("LeftTopDoor").gameObject.SetActive(true);
                    }
                }
                if (Level.Rooms.Exists(x => x.Location == curpos + new Vector2(-1, 0)))
                {
                    Doors.Find("LeftBottomDoor").gameObject.SetActive(true);
                }
                if (Level.Rooms.Exists(x => x.Location == curpos + new Vector2(0, 2)))
                {
                    Doors.Find("TopLeftDoor").gameObject.SetActive(true);
                }
                if (Level.Rooms.Exists(x => x.Location == curpos + new Vector2(1, 2)))
                {
                    Doors.Find("TopRightDoor").gameObject.SetActive(true);
                }
                if (Level.Rooms.Exists(x => x.Location == curpos + new Vector2(2, 1)))
                {
                    Doors.Find("RightTopDoor").gameObject.SetActive(true);
                }
                if (Level.Rooms.Exists(x => x.Location == curpos + new Vector2(2, 0)))
                {
                    Doors.Find("RightBottomDoor").gameObject.SetActive(true);
                }
                if (Level.Rooms.Exists(x => x.Location == curpos + new Vector2(0, -1)))
                {
                    Doors.Find("BottomLeftDoor").gameObject.SetActive(true);
                }
                if (Level.Rooms.Exists(x => x.Location == curpos + new Vector2(1, -1)))
                {
                    Doors.Find("BottomRightDoor").gameObject.SetActive(true);
                }
            }
        }
        else if(R.RoomType == "BossRooms")
        {
            //We don't need to enable any doors for the boss room

        }
        else
        {

            Vector3 offset = new Vector3(0, 13, 0);

            //Check what doors should be there
            //Left
            {
                Vector2 NewPosition = R.Location + new Vector2(-1, 0);
                if (Level.Rooms.Exists(x => x.Location == NewPosition))
                {

                    if (Level.Rooms.First(x => x.Location == NewPosition).RoomType == "BossRooms")
                    {
                        Instantiate(BossDoor, Doors.Find("LeftDoor").position+ offset, Doors.Find("LeftDoor").rotation * Quaternion.Euler(-90,-90,0));
                    }
                    else
                    {
                        Doors.Find("LeftDoor").gameObject.SetActive(true);
                    }
                   
                }
            }
            //Up
            {
                Vector2 NewPosition = R.Location + new Vector2(0, 1);
                if (Level.Rooms.Exists(x => x.Location == NewPosition))
                {
                    if (Level.Rooms.First(x => x.Location == NewPosition).RoomType == "BossRooms")
                    {
                        Instantiate(BossDoor, Doors.Find("TopDoor").position+ offset, Doors.Find("TopDoor").rotation * Quaternion.Euler(-90, -90, 0));
                    }
                    else
                    {
                        Doors.Find("TopDoor").gameObject.SetActive(true);
                    }
                }
            }
            //Down
            {
                Vector2 NewPosition = R.Location + new Vector2(0, -1);
                if (Level.Rooms.Exists(x => x.Location == NewPosition))
                {
                    if (Level.Rooms.First(x => x.Location == NewPosition).RoomType == "BossRooms")
                    {
                        Instantiate(BossDoor, Doors.Find("BottomDoor").position+ offset, Doors.Find("BottomDoor").rotation * Quaternion.Euler(-90, -90, 0));
                    }
                    else
                    {
                        Doors.Find("BottomDoor").gameObject.SetActive(true);
                    }
                }
            }
            //Right
            {
                Vector2 NewPosition = R.Location + new Vector2(1, 0);
                if (Level.Rooms.Exists(x => x.Location == NewPosition))
                {
                    if (Level.Rooms.First(x => x.Location == NewPosition).RoomType == "BossRooms")
                    {
                        Instantiate(BossDoor, Doors.Find("RightDoor").position+ offset, Doors.Find("RightDoor").rotation * Quaternion.Euler(-90, -90, 0));
                    }
                    else
                    {
                        Doors.Find("RightDoor").gameObject.SetActive(true);
                    }
                }
            }

        }

    }

     
    private IEnumerator StartBossRoom()
    {
        float MaxPixelSize = 200;
        yield return new WaitForSeconds(.01f);

        Player.SoundEffectSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/LevelMusic/1/Intro"), 2);
        FSPRF.SetActive(true);
        FSPRF.passMaterial.SetFloat("_PixelSize", MaxPixelSize);

        float pixelationspeed = .005f;
        for (float i = MaxPixelSize; i >= 1; --i)
        {
            yield return new WaitForSeconds(pixelationspeed);
            FSPRF.passMaterial.SetFloat("_PixelSize", i);
        }

        FSPRF.SetActive(false);
        FSPRF.passMaterial.SetFloat("_PixelSize", 1);
    }

    private void EnableBossRoom()
    {
        Transform T2 = GameObject.Find(Player.CurrentRoom.RoomType.ToString()).transform;
        T2.Find(Player.CurrentRoom.RoomNumber.ToString()).gameObject.SetActive(false);


        

        Room BossRoom = Level.Rooms.First(x => x.RoomType == "BossRooms");
        Player.CurrentRoom = BossRoom;
        T2 = GameObject.Find(Player.CurrentRoom.RoomType.ToString()).transform;

        GameObject.Destroy(GameObject.Find("BossDoor(Clone)"));

        Player.Controller.enabled = false;
        transform.position = T2.Find(Player.CurrentRoom.RoomNumber.ToString()).Find("BossSpawnSpot").transform.position;
        Player.Controller.enabled = true;

        Player.Audio.Stop();
        Player.Audio.clip = Resources.Load<AudioClip>("Sounds/LevelMusic/1/BossTheme");
        Player.Audio.Play();

        StartCoroutine(StartBossRoom());


        T2.Find(Player.CurrentRoom.RoomNumber.ToString()).gameObject.SetActive(true);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (changeroomcooldown ||
            hit.gameObject.layer == LayerMask.NameToLayer("Floor") ||
            Player.CurrentRoom.Cleared != true
            )
        {
            return;
        }
        else
        {
            changeroomcooldown = true;
            Invoke(nameof(EndChangeRoomCooldown), Level.RoomChangeTime);
        }


       

        if(hit.transform.name.Contains("BossDoor"))
        {
            EnableBossRoom();
            Player.SoundEffectSource.Stop();
            return;
        }



        if (hit.gameObject.name == "LeftDoor" || 
            hit.gameObject.name == "LeftTopDoor" ||
            hit.gameObject.name == "LeftBottomDoor")
        {
            Player.Camera.transform.position = new Vector3(50, 109, 0);

            if(hit.gameObject.name == "LeftTopDoor") CheckDoor(new Vector2(-1, 1), "RightDoor", new Vector3(-RoomSpawnOffset, 0, 0));
            else CheckDoor(new Vector2(-1, 0), "RightDoor", new Vector3(-RoomSpawnOffset, 0, 0));

        }

        else if (hit.gameObject.name == "RightDoor" ||
             hit.gameObject.name == "RightTopDoor" ||
            hit.gameObject.name == "RightBottomDoor"
            )
        {
            Player.Camera.transform.position = new Vector3(-50, 109, 0);

            if(hit.gameObject.name =="RightBottomDoor") CheckDoor(new Vector2(2, 0), "LeftDoor", new Vector3(RoomSpawnOffset, 0, 0));
            else if(hit.gameObject.name == "RightTopDoor") CheckDoor(new Vector2(2, 1), "LeftDoor", new Vector3(RoomSpawnOffset, 0, 0));
            else CheckDoor(new Vector2(1, 0), "LeftDoor", new Vector3(RoomSpawnOffset, 0, 0));
            
        }

        else if (hit.gameObject.name == "TopDoor" ||
             hit.gameObject.name == "TopLeftDoor" ||
            hit.gameObject.name == "TopRightDoor")
        {
            Player.Camera.transform.position = new Vector3(0, 109, -50);

            if(hit.gameObject.name == "TopLeftDoor") CheckDoor(new Vector2(0, 2), "BottomDoor", new Vector3(0, 0, RoomSpawnOffset));
            else if(hit.gameObject.name == "TopRightDoor") CheckDoor(new Vector2(1, 2), "BottomDoor", new Vector3(0, 0, RoomSpawnOffset));
            else CheckDoor(new Vector2(0, 1), "BottomDoor", new Vector3(0, 0, RoomSpawnOffset));

        }

        else if (hit.gameObject.name == "BottomDoor"||
             hit.gameObject.name == "BottomLeftDoor" ||
            hit.gameObject.name == "BottomRightDoor")
        {
            Player.Camera.transform.position = new Vector3(0, 109, 50);

            if(hit.gameObject.name == "BottomRightDoor") CheckDoor(new Vector2(1, -1), "TopDoor", new Vector3(0, 0, -RoomSpawnOffset));
            else CheckDoor(new Vector2(0, -1), "TopDoor", new Vector3(0, 0, -RoomSpawnOffset));
        }



        


    }


}
