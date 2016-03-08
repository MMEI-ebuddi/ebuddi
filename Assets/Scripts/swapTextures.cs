using UnityEngine;
using System.Collections;

public class swapTextures : MonoBehaviour {
	public GameObject useThis;
	public Texture[] textures;
	public int currentTexture;
	
	public void newTexture () {
		currentTexture++;
		currentTexture %= textures.Length;
		useThis.GetComponent<Renderer>().material.mainTexture = textures[currentTexture];
	
	}

	public void newTexture02 () {
		currentTexture++;
		currentTexture %= textures.Length;
		useThis.GetComponent<Renderer>().material.mainTexture = textures[currentTexture];
		
	}
}
