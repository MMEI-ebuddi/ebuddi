using UnityEngine;
using System.Collections;

public class AnimEventSwapBucketsForBootWash : MonoBehaviour {

    public GameObject[] BucketsToHide;
    public GameObject BucketToShow;

	public void SwapBucketsForBootWash()
    {
        //foreach (GameObject bucket in BucketsToHide)
        //    bucket.SetActive(false);

        BucketToShow.SetActive(true);
    }
}
