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
        StageManager.Instance.LoadNextStage();
    }

    public void OnClickNextMap()
    {
        StageManager.Instance.LoadNextMap();
    }

    public void OnClickReloadMap()
    {
        StageManager.Instance.ReloadMap();
    }
}
