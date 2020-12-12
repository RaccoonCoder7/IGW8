using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    public Image concentraionBar;
    public Image leftGunChamber, rightGunChamber;
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
        leftGunChamber.sprite = chamberSprites[num / 2];
        rightGunChamber.sprite = chamberSprites[num - num / 2];
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
