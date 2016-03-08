using UnityEngine;
using System.Collections;

public static class Config {



	public static string HospitalName() {
		return PlayerPrefs.GetString("hospitalName", "");
	}


}
