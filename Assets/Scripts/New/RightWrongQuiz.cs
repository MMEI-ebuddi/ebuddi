using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum QuizType {
	pre,
	post,
	training
}

public class RightWrongQuiz : MonoBehaviour {

	public Texture2D correctImage;
	public Texture2D incorrectImage;
	public string question;
//	public bool isQuestionnaire;
//	public bool isPreQuestionnaire;
	public QuizType type;

	/// <summary>
	/// true for all single questions, false for questions in questionnaire that are not the last ones
	/// </summary>
	public bool isLastQuestion = true;

	

	public bool Answer(Texture2D answer) {

		return (answer == correctImage);
	}

	
	//possible answers with random order
	public List<Texture2D> GetPossibleAnswers() {
		
		List<Texture2D> result = new List<Texture2D>();
		
		result.Add(correctImage);
		result.Add(incorrectImage);
		result.Shuffle<Texture2D>();

		return result;
	}
}
