using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class StageManager : MonoBehaviour
{
    [Tooltip("개발용으로만 사용됩니다. 입력된 스테이지부터 실행합니다.")]
    public int startStageNum;
    [HideInInspector]
    public Stage nowStage;
    [SerializeField]
    private List<Stage> stageList = new List<Stage>();
    [SerializeField]
    private GameObject dim;
    public List<GameObject> ddoll = new List<GameObject>();

    public static StageManager Instance = null;

    private void Awake()
    {
        if (null == Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(dim);

            // 테스트용 코드
            var player = GameObject.FindWithTag("Player");
            DontDestroyOnLoad(player);

            var mainCam = Camera.main;
            DontDestroyOnLoad(mainCam.gameObject);

            foreach (var obj in ddoll)
            {
                DontDestroyOnLoad(obj);
            }
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        var stage = stageList.Where(x =>
            x.stageNum == startStageNum).FirstOrDefault();

        if (!stage)
        {
            Debug.LogError("시작 스테이지가 리스트에 없습니다: " + startStageNum);
            return;
        }

        StartCoroutine(LoadStage(stage));
    }

    public void LoadNextStage()
    {
        int nextStageIndex = nowStage.stageNum + 1;
        var nextStage = stageList.Where(x =>
            x.stageNum == nextStageIndex).FirstOrDefault();
        if (!nextStage)
        {
            Debug.LogError("해당 스테이지가 없습니다: " + nextStageIndex);
            return;
        }

        StartCoroutine(LoadStage(nextStage));
    }

    public void LoadNextMap()
    {
        if (!nowStage)
        {
            Debug.LogError("현재 스테이지에 대한 정보가 없습니다.");
            return;
        }

        StartCoroutine(LoadNextMap_internal());
    }

    public void ReloadMap()
    {
        if (!nowStage)
        {
            Debug.LogError("현재 스테이지에 대한 정보가 없습니다.");
            return;
        }

        CSVReader.lineIndex = CSVReader.savedIndex;
        StartCoroutine(ReloadMap_internal());
    }

    private IEnumerator LoadStage(Stage stage)
    {
        // TODO: 플레이어 조작 중지
        dim.SetActive(true);
        AsyncOperation async = SceneManager.LoadSceneAsync(stage.stageName);
        async.allowSceneActivation = true;
        while (!async.isDone)
        {
            yield return null;
        }

        if (nowStage)
        {
            DestroyImmediate(nowStage.gameObject);
        }
        nowStage = Instantiate(stage);
        CSVReader.ReadCSV(nowStage.stageTextName);
        yield return StartCoroutine(LoadNextMap_internal());
        dim.SetActive(false);
        // TODO: 플레이어 조작 가능
    }

    private IEnumerator LoadNextMap_internal()
    {
        // TODO: 플레이어 조작 중지
        dim.SetActive(true);
        CSVReader.savedIndex = CSVReader.lineIndex;
        yield return nowStage.StartCoroutine(nowStage.LoadNextMap());
        yield return new WaitForSeconds(0.5f);
        dim.SetActive(false);
        // TODO: 플레이어 조작 가능
    }

    private IEnumerator ReloadMap_internal()
    {
        // TODO: 플레이어 조작 중지
        dim.SetActive(true);
        yield return nowStage.StartCoroutine(nowStage.ReloadMap());
        yield return new WaitForSeconds(0.5f);
        dim.SetActive(false);
        // TODO: 플레이어 조작 가능
    }
}
