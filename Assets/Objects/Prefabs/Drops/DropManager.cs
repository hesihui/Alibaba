using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropManager : MonoBehaviour
{

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Player.AvailableDrops = Resources.LoadAll<Drop>("Drops").ToList();
    }

    
}
