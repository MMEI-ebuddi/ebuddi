using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Question model with multiple answers, only one correct
/// </summary>
public class Question : ScriptableObject {
	

	public string question = "";
	public int correctAnswer = -1;
	[HideInInspector] public List<string> answersGiven;
	public List<string> possibleAnswers;


	void Start() {
		answersGiven = new List<string>();
	}


	public bool Answer(string answer) {

		if (!possibleAnswers.Contains(answer)) {
			Debug.LogError("Given unexpexted answer");
			return false;
		}

		if (!answersGiven.Contains(answer)) answersGiven.Add(answer);

		return (correctAnswer == possibleAnswers.IndexOf(answer));
	}





}
