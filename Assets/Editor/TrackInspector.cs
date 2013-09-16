using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(Track))]
public class TrackInspector : Editor {
    private SerializedObject _trackProxy;
    private Transform _attachTo;
    private Object _grassPrefab;
    private Object _defaultRoadPrefab;
    private Object _trackSidePrefab;
    private int _grassCount;
    private int _roadCount;
    private int _length = 256;

    public static int grassSize = 16;
    
    void OnEnable() {
        _trackProxy = new SerializedObject(target);
    }

    public override void OnInspectorGUI() {
        _trackProxy.Update();
        Track track = (Track)target;
        _attachTo = (Transform)EditorGUILayout.ObjectField("Attach To", _attachTo, typeof(Transform), true);
        _grassPrefab = EditorGUILayout.ObjectField("Grass Prefab", _grassPrefab, typeof(GameObject), true);
        //_grassCount = EditorGUILayout.IntField("Grass Count", _grassCount);
        _defaultRoadPrefab = EditorGUILayout.ObjectField("Default Road Prefab", _defaultRoadPrefab, typeof(GameObject), true);
        //_roadCount = EditorGUILayout.IntField("Road Count", _roadCount);
        _trackSidePrefab = EditorGUILayout.ObjectField("Track Side Prefab", _trackSidePrefab, typeof(GameObject), true);
        if (GUILayout.Button("TEST")) {

            _grassCount = _length / grassSize + 1;
            _roadCount = _grassCount * (grassSize / 8);

            if (_grassPrefab!=null) {
                // create a grass parent node
                GameObject grassParent = new GameObject();
                grassParent.name = "Grass Collection";
                grassParent.transform.parent = _attachTo;
                grassParent.transform.localPosition = Vector3.zero;
                grassParent.transform.localRotation = Quaternion.identity;
                grassParent.transform.localScale = Vector3.one;

                for (int i=0;i<_grassCount;i++) {
                    // create left side
                    GameObject grassClone = (GameObject)Instantiate(_grassPrefab);
                    grassClone.transform.parent = grassParent.transform;
                    grassClone.transform.localPosition = new Vector3(grassSize/2 + 5, 0, i * grassSize + grassSize/2);
                    //grassClone.transform.localRotation = Quaternion.identity;
                    grassClone.transform.localScale = Vector3.one;

                    // create right side
                    grassClone = (GameObject)Instantiate(_grassPrefab);
                    grassClone.transform.parent = grassParent.transform;
                    grassClone.transform.localPosition = new Vector3(-grassSize/2 - 5, 0, i * grassSize + grassSize/2);
                    //grassClone.transform.localRotation = Quaternion.identity;
                    grassClone.transform.localScale = Vector3.one;
                }
            }

            if (_defaultRoadPrefab!=null) {
                GameObject roadCollection = new GameObject();
                roadCollection.name = "Road Collection";
                roadCollection.transform.parent = _attachTo;
                roadCollection.transform.localPosition = Vector3.zero;
                roadCollection.transform.localRotation = Quaternion.identity;
                roadCollection.transform.localScale = Vector3.one;
                for (int i=0;i<_roadCount;++i) {
                    GameObject roadClone = (GameObject)Instantiate(_defaultRoadPrefab);
                    roadClone.transform.parent = roadCollection.transform;
                    roadClone.transform.localPosition = new Vector3(0, 0, i * 8 + 4);
                    roadClone.transform.localScale = Vector3.one;
                }
            }

            if (_trackSidePrefab!=null) {
                GameObject trackSideCollection = new GameObject();
                trackSideCollection.name = "Track Side Collection";
                trackSideCollection.transform.parent = _attachTo;
                trackSideCollection.transform.localPosition = Vector3.zero;
                trackSideCollection.transform.localRotation = Quaternion.identity;
                trackSideCollection.transform.localScale = Vector3.one;

                for (int i=0;i<256 / 8;++i) {
                    // Right TrackSide
                    GameObject trackSide = (GameObject)Instantiate(_trackSidePrefab);
                    trackSide.transform.parent = trackSideCollection.transform;
                    trackSide.transform.localPosition = new Vector3(4.5f, 0, i * 8 + 4);
                    trackSide.transform.localRotation = Quaternion.identity;
                    trackSide.transform.localScale = Vector3.one;

                    // Left Track Side
                    trackSide = (GameObject)Instantiate(_trackSidePrefab);
                    trackSide.transform.parent = trackSideCollection.transform;
                    trackSide.transform.localPosition = new Vector3(-4.5f, 0, i * 8 + 4);
                    trackSide.transform.localRotation = Quaternion.identity;
                    trackSide.transform.localScale = Vector3.one;
                }
            }
        }
    }

}
