using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Stage : MonoBehaviour
{
    public int stageNum;
    public string stageName;
    public List<MapManager> mapList = new List<MapManager>();

    private MapManager nowMap;


    public IEnumerator LoadNextMap()
    {
        int nextMapNum = 1;
        if (nowMap)
        {
            nextMapNum = nowMap.mapNum + 1;
        }
        var nextMap = mapList.Where(x =>
            x.mapNum == nextMapNum).FirstOrDefault();
        if (!nextMap)
        {
            Debug.LogError("해당 맵이 없습니다: " + nextMapNum);
            yield break;
        }

        if (nowMap)
        {
            DestroyImmediate(nowMap.gameObject);
            nowMap = null;
        }

        nowMap = Instantiate(nextMap, Vector3.zero, Quaternion.identity);
        SetPlayerStartPos(nowMap);
        yield return null;
    }

    public IEnumerator ReloadMap()
    {
        if (!nowMap)
        {
            Debug.LogError("현재 맵이 없습니다.");
            yield break;
        }

        int nextMapNum = nowMap.mapNum;
        var nextMap = mapList.Where(x =>
            x.mapNum == nextMapNum).FirstOrDefault();
        if (!nextMap)
        {
            Debug.LogError("해당 맵이 없습니다: " + nextMapNum);
            yield break;
        }

        DestroyImmediate(nowMap.gameObject);
        nowMap = null;

        nowMap = Instantiate(nextMap, Vector3.zero, Quaternion.identity);
        SetPlayerStartPos(nowMap);
        yield return null;
    }

    private void SetPlayerStartPos(MapManager mm)
    {
        var player = GameObject.FindWithTag("Player");
        player.transform.position = mm.playerStartTr.position;
    }
}
