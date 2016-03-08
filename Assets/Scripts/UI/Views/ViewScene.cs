using UnityEngine;
using System.Collections;

public class ViewScene : UIView {

	public static ViewScene instance;
	public BuddyDialog buddyDialog;
	public MovieDialog movieDialog;
	public UIMasterProgressBar progressBar;



	public virtual void Awake() {
		instance = this;
		base.Awake();
		buddyDialog.Hide();
		if (movieDialog != null) movieDialog.Hide();
	}









}
