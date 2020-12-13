using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    public Transform parentTr;
    public List<GameObject> tiles;
    public int tileIndex;
    public int generateCount;
    public bool noWidth = true;
    public bool isRight = true;
    public bool noHeight = true;
    public bool isUp = true;
    public float tileSize = 0.5f;

    [ContextMenu("GenerateTile")]
    public void GenerateTile()
    {
        for (int i = 0; i < generateCount; i++)
        {
            float posX = transform.position.x;
            if(!noWidth)
            {
                posX += ((isRight ? 0.5f : -0.5f) * i);
            }

            float posY = transform.position.y;
            if(!noHeight)
            {
                posY += ((isUp ? 0.5f : -0.5f) * i);
            }

            Vector3 generatePos = new Vector3(posX, posY, transform.position.z);
            Instantiate(tiles[tileIndex], generatePos, tiles[tileIndex].transform.rotation, parentTr);
        }
    }
}
