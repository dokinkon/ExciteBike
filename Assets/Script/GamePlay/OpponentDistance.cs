using UnityEngine;
using System.Collections;

namespace GamePlay {

    public class OpponentDistance : MonoBehaviour {

        public TextMesh[] textMeshes;
        private ViewController _viewController;

        // Use this for initialization
        void Start () {
            _viewController = GetComponent<ViewController>();
        }
        
        // Update is called once per frame
        void Update () {
            Bike localBike = _viewController.localBike;
            float z = localBike.transform.position.z;
            int i = 0;
            foreach (Bike bike in _viewController.GetBikes()) {
                if (bike!=localBike) {
                    int distance = (int)(localBike.transform.position.z - bike.transform.position.z);
                    Vector3 position = Vector3.zero;
                    textMeshes[i].text = distance.ToString() + "M";
                    position.x = bike.transform.position.x;
                    position.y = 1;
                    if (distance > 0) {
                        position.z = z - 3;
                        textMeshes[i].color = new Color(255, 0, 0, 255.0f * distance - 10);
                    } else {
                        position.z = z + 5;
                    }

                    textMeshes[i].transform.position = position;
                    i++;
                }
            }
        }
    }

}
