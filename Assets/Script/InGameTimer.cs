using UnityEngine;
using System.Collections;

public class InGameTimer : MonoBehaviour {
	
	public UILabel label;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
		float timeSinceLevelLoad = Time.timeSinceLevelLoad;
		int sec = (int)timeSinceLevelLoad;
		int min = (int)(sec / 60);
		if (min > 99 ) {
			min = 99;
		}
		int msec = (int)((timeSinceLevelLoad - sec)*100);
		sec = sec % 60;
		
		label.text = System.String.Format("{0:00}:{1:00}:{2:00}", min, sec, msec);
	}
}
