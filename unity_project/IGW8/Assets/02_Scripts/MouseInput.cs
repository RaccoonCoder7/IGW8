using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour //커서용 스크립트
{
    public Texture2D cursorTexture;
    
    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Cursor.SetCursor(cursorTexture, new Vector2(7.5f, 7.5f), CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
