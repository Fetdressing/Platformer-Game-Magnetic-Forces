using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
    public Camera cameraPlayerFollow;
    [HideInInspector]
    public Camera currCamera;
	// Use this for initialization
	void Start () {
        currCamera = cameraPlayerFollow;
	}
	

}
