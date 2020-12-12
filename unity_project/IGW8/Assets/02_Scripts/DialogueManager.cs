using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance = null;
    public GameObject timeouObjs, dialogueObjs;
    public GameObject timeoutGauge;
    public GameObject noWait, choiceObj1, choiceObj2;
    public GameObject cursor;
    public int cursorIdx;

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

        //StartCoroutine("StartDialogue",new string[3]{"명령이다. 물러서!","잠깐, 확인좀 하지","*총을 뽑는다*"});
    }

    public IEnumerator StartDialogue(string[] strings)
    {
        timeouObjs.SetActive(true);
        dialogueObjs.SetActive(true);
        noWait.SetActive(false);
        choiceObj1.SetActive(false);
        choiceObj2.SetActive(false);
        yield return new WaitForSeconds(.5f);
        StartCoroutine("StartTimeout");
        StartCoroutine("ShowChoices", strings);
    }

    IEnumerator StartTimeout()
    {
        timeoutGauge.GetComponent<Image>().fillAmount = 0;
        for (float f = 0; f < 1; f += .001f)
        {
            timeoutGauge.GetComponent<Image>().fillAmount = f;
            yield return null;
        }
    }
    
    IEnumerator ShowChoices(string[] strings)
    {
        
        if (strings.Length == 3)
        {
            noWait.SetActive(true);
            noWait.GetComponent<RectTransform>().localPosition=new Vector3(-120, -9.5f, 0);
            noWait.GetComponentInChildren<Text>().text = strings[2];
            for (float f = 1f; f > 0; f -= .02f)
            {
                Vector3 pos = new Vector3(-60 * f, -9.5f, 0);
                noWait.GetComponent<RectTransform>().localPosition = pos;
                yield return null;
            }
        }
        yield return new WaitForSeconds(1f);
        noWait.SetActive(false);
        choiceObj1.SetActive(true);
        choiceObj2.SetActive(true);
        choiceObj1.GetComponent<RectTransform>().localPosition=new Vector3(-120, 12.7f, 0);
        choiceObj2.GetComponent<RectTransform>().localPosition=new Vector3(-120, -32.8f, 0);
        choiceObj1.GetComponentInChildren<Text>().text = strings[0];
        choiceObj2.GetComponentInChildren<Text>().text = strings[1];
        for (float f = 1f; f > 0; f -= .02f)
        {
            Vector3 pos1 = new Vector3(-60 * f, 12.7f, 0);
            Vector3 pos2 = new Vector3(-60 * f, -32.8f, 0);
            choiceObj1.GetComponent<RectTransform>().localPosition = pos1;
            choiceObj2.GetComponent<RectTransform>().localPosition = pos2;
            yield return null;
        }
    }
}
