using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayEmitter : MonoBehaviour
{
    public GameObject raycaster;
    // public MeshCollider collide;
    
    private string rayName = "[name]";
    private List<Vector2> rayVertices;
    private GameObject rayEmitter;
    private GameObject raycasterEntity;
    private Camera arCamera;

    [HideInInspector]
    public List<TextureToCloudVision.ObjectAnnotation> detectedObjects;
    
    void Start()
    {
        Debug.Log("rayEmitter Start()");
        arCamera = GameObject.FindWithTag("ARCamera").GetComponent<Camera>();
        for (int i = 0; i < detectedObjects.Count; i++) {
            rayName = detectedObjects[i].name;
            rayVertices = detectedObjects[i].boundingPoly.normalizedVertices;
            PlaceRaycaster(rayName, rayVertices);
        }
    }
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
       /* for (var i = 0; i < Input.touchCount; ++i) {
            if (Input.GetTouch(i).phase == TouchPhase.Began) {
                rayEmitter = new GameObject();
                rayEmitter.transform.position = arCamera.transform.position;
                rayEmitter.transform.rotation = arCamera.transform.rotation;
                Debug.Log(Input.GetTouch(i).position);
                PlaceRaycaster(Input.GetTouch(i).position);
            }
        } */
    }


    void PlaceRaycaster(string rayName, List<Vector2> vertices) {
        // Then place the raycaster container on the camera
        Debug.Log("Instantiate raycaster");
        raycasterEntity = Instantiate(raycaster, arCamera.transform.position, arCamera.transform.rotation);
        raycasterEntity.GetComponent<RayEntity>().emitter = transform;
        raycasterEntity.GetComponent<RayEntity>().vertices = vertices;
        raycasterEntity.GetComponent<RayEntity>().rayName = rayName;
    }
}
