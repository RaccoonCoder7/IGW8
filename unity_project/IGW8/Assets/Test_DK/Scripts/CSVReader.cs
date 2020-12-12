using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVReader
{
    public static int lineIndex = -1;
    public static int savedIndex = -1;

    private static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    private static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    private static List<LineData> lineDatas = new List<LineData>();

    public class LineData
    {
        public int maxTime;
        public int eventTime;
        public string mainText;
        public string[] buttonTexts = new string[3];
    }

    public static void ReadCSV(string csvName)
    {
        bool skipFirstLine = true;
        lineDatas.Clear();
        lineIndex = -1;

        TextAsset textData = Resources.Load(csvName) as TextAsset;
        var lines = Regex.Split(textData.text, LINE_SPLIT_RE);
        foreach (var line in lines)
        {
            if (skipFirstLine)
            {
                skipFirstLine = false;
                continue;
            }

            var values = Regex.Split(line, SPLIT_RE);
            LineData lineData = new LineData();
            try
            {
                lineData.maxTime = Convert.ToInt32(values[0]);
                lineData.eventTime = Convert.ToInt32(values[1]);
                lineData.mainText = values[2];
                lineData.buttonTexts[0] = values[3];
                if (values.Length > 4)
                {
                    lineData.buttonTexts[1] = values[4];
                    if (values.Length > 5)
                    {
                        lineData.buttonTexts[2] = values[5];
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("csv값이 잘못되었습니다.");
                break;
            }
            lineDatas.Add(lineData);
        }
    }

    public static LineData GetNextLineData()
    {
        lineIndex++;
        if (lineDatas.Count > lineIndex)
        {
            return lineDatas[lineIndex];
        }

        lineIndex--;
        Debug.LogError("다음 라인이 없습니다.");
        return null;
    }
}
