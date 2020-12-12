using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectUIManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject container;
    public Text mainText;
    public RectTransform progressBar;
    public RectTransform progressBar_red;
    public RectTransform progressBar_white;
    public SelectButtonManager[] buttonArr = new SelectButtonManager[3];

    [Header("Setting")]
    public float mainTextStreamSpeed = 0.02f;

    private Vector3 progressBarStartPos;
    private float progressBarMaxValue;
    private string mainTextStr;
    private Coroutine coroutine;

    public static SelectUIManager Instance;

    public void StartSelect(Action[] btnEvents)
    {
        var lineData = CSVReader.GetNextLineData();
        if (lineData == null)
        {
            return;
        }

        mainTextStr = lineData.mainText;
        SetButtons(btnEvents, lineData.buttonTexts);
        container.SetActive(true);
        coroutine = StartCoroutine(Progress(lineData.maxTime, lineData.eventTime));
    }

    public Action[] GetTestActionArr()
    {
        Action[] actionArr = new Action[3];
        actionArr[0] = () => { Debug.Log("버튼1 클릭됨"); };
        actionArr[1] = () => { Debug.Log("버튼2 클릭됨"); };
        actionArr[2] = () => { Debug.Log("버튼3 클릭됨"); };
        return actionArr;
    }

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

        progressBarMaxValue = progressBar.rect.width;
        progressBarStartPos = new Vector3(
            -progressBarMaxValue
            , progressBar.anchoredPosition.y
            , 0);

        ResetUI();
    }

    private void SetButtons(Action[] btnEvents, string[] buttonTexts)
    {
        for (int i = 0; i < btnEvents.Length; i++)
        {
            int index = i;
            buttonArr[i].button.onClick.RemoveAllListeners();
            buttonArr[i].button.onClick.AddListener(() =>
            {
                btnEvents[index].Invoke();
                OnClickButton_internal();
            });
            buttonArr[i].text.text = buttonTexts[i];
        }
    }

    private void OnClickButton_internal()
    {
        StopCoroutine(coroutine);
        container.SetActive(false);
        ResetUI();
    }

    private void ResetUI()
    {
        mainText.text = string.Empty;
        ResetButtons();
        ResetProgressBar();
    }

    private void ResetButtons()
    {
        buttonArr[1].button.gameObject.SetActive(false);
        buttonArr[2].button.gameObject.SetActive(false);
    }

    private void ResetProgressBar()
    {
        progressBar_red.anchoredPosition = progressBarStartPos;
        progressBar_white.anchoredPosition = progressBarStartPos;
    }

    private IEnumerator Progress(float maxTime, float eventTime)
    {
        int textIndex = 0;
        float currentTime = 0f;
        float textTime = 0f;
        float eventPercent = 1 - (eventTime / maxTime);
        bool eventEnd = false;
        progressBar_red.anchoredPosition = new Vector3(
            -(progressBarMaxValue * eventPercent)
            , progressBar.anchoredPosition.y
            , 0);

        while (currentTime < maxTime)
        {
            textTime += Time.deltaTime;
            if (textIndex < mainTextStr.Length && textTime > mainTextStreamSpeed)
            {
                textIndex++;
                mainText.text = mainTextStr.Substring(0, textIndex);
                textTime = 0f;
            }

            currentTime += Time.deltaTime;
            float percent = 1 - (currentTime / maxTime);
            progressBar_white.anchoredPosition = new Vector3(
                -(progressBarMaxValue * percent)
                , progressBar.anchoredPosition.y
                , 0);

            if (!eventEnd && eventPercent > percent)
            {
                eventEnd = true;
                var showBtn = buttonArr.Where(x => !string.IsNullOrEmpty(x.text.text));
                foreach (var btn in showBtn)
                {
                    btn.gameObject.SetActive(true);
                }
            }
            yield return null;
        }

        buttonArr[0].button.onClick.Invoke();
    }
}
