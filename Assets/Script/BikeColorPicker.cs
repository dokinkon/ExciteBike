using UnityEngine;
using System.Collections;

public class BikeColorPicker : MonoBehaviour {

    public GameObject colorButtonPrefab;
    public UIDraggableCamera draggableCamera;
	// Use this for initialization
	void Start () {

        /*
        Color[] availableColors = BikeShape.GetAvailableColors();
        for (int i=0;i<availableColors.Length;++i) {
            Color color = availableColors[i];
            GameObject button = (GameObject)Instantiate(colorButtonPrefab);
            ColorButton colorButton = (ColorButton)button.GetComponent<ColorButton>();
            colorButton.SetColor(color) ;
            button.transform.parent = transform;
            button.transform.localPosition = Vector3.zero;
            button.transform.localRotation = Quaternion.identity;
            button.transform.localScale = Vector3.one;

            UIDragCamera dragCamera = button.GetComponent<UIDragCamera>();
            dragCamera.draggableCamera = draggableCamera;
        }

        UITable table = GetComponent<UITable>();
        table.Reposition();
        */
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
