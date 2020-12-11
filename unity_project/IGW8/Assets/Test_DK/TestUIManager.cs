using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void OnClickNextStage()
    {
        Debug.Log("NextStage");
        StageManager.Instance.LoadNextStage();
    }

    public void OnClickNextMap()
    {
        Debug.Log("NextMap");
        StageManager.Instance.LoadNextMap();
    }
}
