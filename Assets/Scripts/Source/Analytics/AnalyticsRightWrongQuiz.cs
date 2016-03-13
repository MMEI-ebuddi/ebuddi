using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnalyticsRightWrongQuiz
{
    public string Question;
    public string Answer;
    public List<string> Guesses;
    public int TestRun;

	public AnalyticsRightWrongQuiz()
    {
        Guesses = new List<string>();
    }
}