using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnalyticsQuiz
{
    public string Question;
    public string Answer;
    public List<string> Guesses;
    public int TestRun;
	public QuizType type;
	public Dictionary<string, string> additionalAttributes = new Dictionary<string, string>();


    public AnalyticsQuiz()
    {
        Guesses = new List<string>();
    }
}