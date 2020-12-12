using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GFXManager : MonoBehaviour
{
    public static GFXManager Instance = null;

    public GameObject GFXObj;
    private GameObject[] GFXObjs = new GameObject[20];
    private int objCnt;
    
    void Awake()
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

    public void GenerateGFX(Vector3 pos, Quaternion quaternion, string type)
    {
        GFXObjs[objCnt % 20].SetActive(true);
        GFXObjs[objCnt % 20].GetComponent<Animator>().SetTrigger(type);
        GFXObjs[objCnt % 20].transform.position = pos;
        GFXObjs[objCnt % 20].transform.rotation = quaternion;
        objCnt++;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            GFXObjs[i] = Instantiate(GFXObj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
