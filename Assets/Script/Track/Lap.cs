using UnityEngine;
using System.Collections;
using System;


public class Lap : MonoBehaviour {

    public Section[] sections;
    public int currentSection;

    void Awake() {
        //Debug.Log("[Track.Lap.Awake]");
        currentSection = 0;
    }

	// Use this for initialization
	void Start () {
        //Debug.Log("[Track.Lap.Start]");
        for (int i=0;i<sections.Length;++i) {
            sections[i].OnPlayerEnterSection += OnPlayerEnterSection;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy() {
        if (!GameManager.isShutingDown) {
            for (int i=0;i<sections.Length;++i) {
                sections[i].OnPlayerEnterSection -= OnPlayerEnterSection;
            }
        }
    }

    void OnPlayerEnterSection(int sectionIndex) {
        Debug.Log("[Track.Lap.OnPlayerEnterSection] index:" + sectionIndex);
        //if (currentSection==-1) {
            //currentSection = sectionIndex;
            //return;
        //}
        if (currentSection == sectionIndex)
            return;

        Section currSection = sections[currentSection];
        Section nextSection = sections[sectionIndex];
        currentSection = sectionIndex;
        bool moveForward = (nextSection.transform.position.z > currSection.transform.position.z);
        if (moveForward) {
            Debug.Log("[Track.Lap.OnPlayerEnterSection] moveForward");
            // move last section block to fowardest
            int backIndex = sectionIndex-2;
            while (backIndex < 0) {
                backIndex += 4;
            }
            Debug.Log("sectionIndex:" + sectionIndex + " backIndex:" + backIndex);
            Section moveSection = sections[backIndex];
            if (moveSection.transform.position.z > nextSection.transform.position.z)
                return;

            moveSection.transform.position = new Vector3(0, 0, nextSection.transform.position.z + 512.0f);
            if (moveSection.lapSensor!=null) {
                moveSection.lapSensor.lap += 1;
            }
        } else {
            Debug.Log("[Track.Lap.OnPlayerEnterSection] moveBackwoard");
            Section moveSection = sections[(sectionIndex + 2) % 4];
            if ( moveSection.transform.position.z < nextSection.transform.position.z )
                return;

            moveSection.transform.position = new Vector3(0, 0, nextSection.transform.position.z - 512);
            if (moveSection.lapSensor!=null) {
                moveSection.lapSensor.lap -= 1;
            }
        }



    }


}

