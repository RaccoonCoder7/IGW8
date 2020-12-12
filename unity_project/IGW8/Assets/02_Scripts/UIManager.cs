using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    public Image concentraionBar;
    public Image[] magazineBullets=new Image[12];
    public GameObject[] deathMarks;
    private int markCnt;
    public Sprite[] chamberSprites;

    private void Awake()
    {
        if (null == Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGunChambersUI(int num)
    {
        for (int i = 0; i < 12 - num; i++)
        {
            magazineBullets[i].gameObject.SetActive(false);
        }
        for (int i = 12-num; i < 12 ; i++)
        {
            magazineBullets[i].gameObject.SetActive(true);
        }
    }

    public void SetConcentrationUI(int num)
    {
        concentraionBar.fillAmount = (float) num / 2000;
    }

    public void MarkedForDeath(Vector2 pos)
    {
        deathMarks[markCnt].SetActive(true);
        deathMarks[markCnt].GetComponent<RectTransform>().position = pos;
        markCnt++;
    }

    public void RemoveMarks()
    {
        foreach (var obj in deathMarks)
        {
            obj.SetActive(false);
        }
        markCnt = 0;
    }
}
