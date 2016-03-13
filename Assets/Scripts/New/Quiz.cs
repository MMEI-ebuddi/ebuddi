using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Quiz : MonoBehaviour {


	public Texture2D sourceImage;
	public List<Texture2D> images;
    public string Question;


	public void Setup(Texture2D source)  {

		//check if we are not asking about next step of last image
		if (images.IndexOf(source) == images.Count-1) {
			Debug.LogError("Setting up quiz with last image on the list isnt allowed");
			return;
		}

		this.sourceImage = source;
	}



	public bool Answer(Texture2D answer) {

		if (sourceImage == null) Debug.LogError("sourceImage must be assigned before answer is given");

		int currentIndex = images.IndexOf(sourceImage);
		return (images.IndexOf(answer) == currentIndex + 1); 

	}


    public Texture2D GetCorrectAnswer()
    {
        int currentIndex = images.IndexOf(sourceImage);

        return images[currentIndex + 1];
    }

	//retorn possible answers with random order
	public List<Texture2D> GetPossibleAnswers() {

		List<Texture2D> result = new List<Texture2D>();

		foreach (Texture2D image in images) {

			//if not source image add randomly to result
			if (image != this.sourceImage) {
				result.Insert(Random.Range(0, result.Count+1), image);
			}
		}
		return result;
	}




}
