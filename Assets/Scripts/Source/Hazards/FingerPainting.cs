using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMSupport;


public class FingerPainting : Hazard {

	public LayerMask selectionMask;

	/// <summary>
	/// The contaminated areas in the right order according to animation
	/// </summary>
	public List<FingerPaintingContaminatedArea> contaminatedAreas = new List<FingerPaintingContaminatedArea>();
	public GameObject hitAreaPrefab;
	public List<GameObject> hitAreas = new List<GameObject>();
	private Avatar avatar;
	public Scene2 sceneManager;
	public GameObject buddy;
	public bool isActive = false;
	public Mode currentMode;
	public GameObject finishedButton;

	public Animator doorAnimator;
	public Animator patientAnimator;
	public Animator waterBagAnimator;
	public CameraPathAnimator cameraPath;

	public enum Mode {
		inActive,
		intro,
		presentation,
		inGame,
		finished
	}



	void Awake() {
		finishedButton.SetActive(false);
		TMMessenger.Instance.AddToMessageQueue(new MessageCallback(this.MessageCallback));
		Init();
	}
	

	override protected void Init() {
		isActive = true;
		currentMode = Mode.inActive;

		foreach (FingerPaintingContaminatedArea area in contaminatedAreas) {
			area.Reset();
		}
	}


	public override void Activate() {

		currentMode = Mode.intro;
		ViewScene.instance.buddyDialog.SetText("Some text here about the virus spreading");
		patientAnimator.SetBool("headUp", false);
	
		sceneManager.DirtyAvatarOn();
		sceneManager.m_Avatar.m_Top.SetActive(true);
		sceneManager.m_Avatar.m_Bottom.SetActive(true);
		sceneManager.m_Avatar.m_FaceShield.SetActive(true);
		avatar = sceneManager.m_Avatar;



	}


	void Finished() {
		avatar.OnAnimationEventReceived -= OnAnimationEventReceived;
	}



	void StartGameMode() {
		currentMode = Mode.inGame;
		finishedButton.SetActive(true);
	}



	void MessageCallback(TMMessageNode Message)
	{
		if(!isActive) return;
		
		if (Message.Message == "BuddyDialogClosed".GetHashCode())
		{
			if (currentMode == Mode.intro) {
				//STARTING 
//				OurCamera.Instance.MoveTo(OurCamera.eCameraFocus.Avatar, 1f);
				buddy.SetActive(false);
//			
//
//				sceneManager.m_Avatar.m_Top.SetActive(true);
//				sceneManager.m_Avatar.m_Bottom.SetActive(true);
//				sceneManager.m_Avatar.m_FaceShield.SetActive(true);

				Debug.Log("subscribe to " + avatar.gameObject.name);
				avatar.OnAnimationEventReceived += OnAnimationEventReceived;
				
				finishedButton.SetActive(false);
				currentMode = Mode.presentation;
				avatar.SetAnimationTrigger("FingerPainting");

				cameraPath.Play();

			}
			else if (currentMode == Mode.presentation) {
				StartGameMode();
			}
			else if (currentMode == Mode.finished) {
				//cleanup and finish
				foreach (FingerPaintingContaminatedArea area in contaminatedAreas) area.Reset();
				foreach (GameObject hitArea in hitAreas) Destroy(hitArea.gameObject);
				ViewHazards.instance.progressBar.SetModuleProgress(ModuleType.hazards, 1f, true);
				StartCoroutine(MoveToNextHazard());
				isActive = false;
			}
		}
	}


	IEnumerator MoveToNextHazard() {
		yield return new WaitForSeconds (1f);

		HazardManager.Instance.MoveToNextHazard();
		
		//finished finger painting
		sceneManager.AllHazardsFinished();


	}



	public void FinishedTapped() {
		//replay animation by showing where virus have speaded
		avatar.SetAnimationTrigger("ReplayFingerPainting");
		doorAnimator.SetTrigger("reset");
		waterBagAnimator.SetTrigger("reset");
		finishedButton.SetActive(false);
		patientAnimator.SetBool("headUp", false);
	}


	//triggered by the animation on Avatar
	void OnAnimationEventReceived (string eventName)
	{
		if (eventName == "FingerPaintingFinished") {
			if (currentMode == Mode.presentation) {
				ViewScene.instance.buddyDialog.SetText("Please highlight the areas whe you think virus might spread");
			}
			else if (currentMode == Mode.inGame) {

				//check for result
				if (AreasMissedCount() == 0) {
					ViewScene.instance.buddyDialog.SetText("Well done");
				}
				else {

					string countSufix = "area";
					if (AreasMissedCount() > 1) countSufix += "s";

					ViewScene.instance.buddyDialog.SetText("Not good, You have missed " + AreasMissedCount().ToString() + " " + countSufix);
				}

				currentMode = Mode.finished;
			}
		}
		else if (eventName == "FingerPaintingContaminateNext") {

			//contaminate only if we are in game mode 
			if (currentMode == Mode.inGame) {
				//contaminateNextArea
				for (int i=0; i<contaminatedAreas.Count; i++) {
					if (!contaminatedAreas[i].IsContaminated) {
						contaminatedAreas[i].IsContaminated = true;
						break;
					}
				}
			}
		}
		else if (eventName == "FingerPaintingDoorClose") {
			doorAnimator.SetTrigger("close");
		}
		else if (eventName == "FingerPaintingHeadUp") {
			patientAnimator.SetBool("headUp", true);
		}
		else if (eventName == "FingerPaintingWaterBag") {
			waterBagAnimator.SetTrigger("play");
		}


	}




	int AreasMissedCount() {

		int result = 0;
		foreach(FingerPaintingContaminatedArea area in contaminatedAreas) {
			if (!area.IsSelected && area.IsContaminated) result++;
		}

		return result;
	}



	void Update() {

		if (isActive) {
			if (Input.GetMouseButtonDown(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;		
				
				// Casts the ray and get the first game object hit
				if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectionMask.value))
				{			
					if (currentMode == Mode.inGame) {
						if (hit.collider.gameObject.GetComponent<FingerPaintingContaminatedArea>() != null) {
							FingerPaintingContaminatedArea area = hit.collider.gameObject.GetComponent<FingerPaintingContaminatedArea>();

							if (!area.IsSelected) area.IsSelected = true;
							
						}
						else {
							Debug.Log(hit.point);
							//hitarea
							GameObject hitArea = (GameObject)Instantiate(hitAreaPrefab);
							hitArea.transform.position = hit.point;
							hitArea.GetComponent<MeshRenderer>().enabled = true;
							hitAreas.Add(hitArea);
						}
					}

				}
			}
		}
	}




}
