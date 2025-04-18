using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour

{
    public Texture2D cursorTexture; // Drag your PNG here in the Inspector
    public Vector2 hotspot = Vector2.zero; // Will adjust this later
    public CursorMode cursorMode = CursorMode.Auto;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.SetCursor(cursorTexture, hotspot, cursorMode);
    }
}
