using UnityEngine;
using System.Collections;

public class ViewDonning : ViewScene {

	void Start () {
	
		progressBar.SetModuleProgress(ModuleType.intro, 1f, false);
		progressBar.SetModuleProgress(ModuleType.doffing, 1f, false);
		progressBar.SetModuleProgress(ModuleType.hazards, 1f, false);


	}
	

}
