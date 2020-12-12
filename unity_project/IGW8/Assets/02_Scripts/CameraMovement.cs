using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public PlayerController _Player;
    private Vector3 initPos;
    
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position - _Player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 playerPos = _Player.transform.position;
        transform.position = initPos + playerPos;
    }
}
