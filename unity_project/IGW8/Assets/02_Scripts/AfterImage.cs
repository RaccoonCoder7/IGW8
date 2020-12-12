using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    public GameObject obj;
    private GameObject[] afterImages;
    private int objCnt = 10;
    public PlayerController _Player;
    
    // Start is called before the first frame update
    void Start()
    {
        afterImages=new GameObject[objCnt];
        for (int i = 0; i < objCnt; i++)
        {
            afterImages[i] = Instantiate(obj);
            afterImages[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator GenerateAfterImages(KeyValuePair<Sprite, int> pair)
    {
        for (int i = 0; i < objCnt; i++)
        {
            afterImages[i].SetActive(true);
            afterImages[i].GetComponent<SpriteRenderer>().sprite = pair.Key;
            if (pair.Value < 0) afterImages[i].GetComponent<SpriteRenderer>().flipX = true;
            else afterImages[i].GetComponent<SpriteRenderer>().flipX = false;
            afterImages[i].transform.localPosition = _Player.transform.position;
            afterImages[i].GetComponent<SpriteRenderer>().color=new Color((i+1)/11f,(i+1)/11f,(i+1)/11f);
            yield return new WaitForSeconds(.02f);
        }
        for (int i = 0; i < objCnt; i++)
        {
            afterImages[i].SetActive(false);
            //yield return new WaitForSeconds(.02f);
        }
    }
}
