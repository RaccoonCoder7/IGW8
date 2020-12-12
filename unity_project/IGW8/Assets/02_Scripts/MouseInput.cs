using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    public Texture2D cursorTexture;
    
    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Cursor.SetCursor(cursorTexture, new Vector2(8,8), CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
