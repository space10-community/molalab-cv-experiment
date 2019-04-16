using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayEntity : MonoBehaviour
{
    private bool didHit;
    private int raycasts = 0;
    public float raycastRetryDelay;
    public int maxRaycastRetrys;
    
    public GameObject objectTag;
    [HideInInspector]
    public Transform emitter;
    [HideInInspector]
    public string rayName;
    [HideInInspector]
    public List<Vector2> vertices;

    private Ray ray;
    private Camera arCamera;
    private Transform rayShooter;
    private RaycastHit hit;
    private RaycastHit boundaryHit;
    private LineRenderer boundary;

    public GameObject polyObj;

    void Start()
    {
        arCamera = GameObject.FindWithTag("ARCamera").GetComponent<Camera>();
        Debug.Log("raycaster Start()");
        float x = vertices[0].x + (vertices[2].x - vertices[0].x) / 2;
        float y = vertices[0].y + (vertices[2].y - vertices[0].y) / 2;
        Vector2 screenCoordinate = GetScreenCoordinate(x, y);
        // Get a ray as if it was cast from a camera towards a screen
        ray = arCamera.ScreenPointToRay(screenCoordinate);

        // ...and rotate the child ray object towards the ray
        transform.rotation = arCamera.transform.rotation;
        transform.position = arCamera.transform.position;
        rayShooter = transform.GetChild(0);
        rayShooter.rotation = Quaternion.LookRotation(ray.direction, Vector3.up);
        // Now make the raycaster parent a child of the emitter object and reset its position and rotation
        transform.parent = emitter;
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localEulerAngles = new Vector3(0, 0, 0);
        StartCoroutine("Cast");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    Vector2 GetScreenCoordinate (float x, float y) {
        x = x * Screen.width;
        y = y * Screen.height;
        y = Screen.height - y;
        return new Vector2(x, y);
    }

    IEnumerator Cast() {
        // Ray ray = new Ray(rayShooter.position, rayShooter.forward);
        
        if (Physics.Raycast(rayShooter.position, rayShooter.forward, out hit, 10.0f, LayerMask.NameToLayer("MeshLayer")))
        {
            rayShooter.GetComponent<LineRenderer>().enabled = false;
            Debug.Log("Instantiate objectTag");
            objectTag = Instantiate(objectTag, hit.point, transform.rotation);
            objectTag.transform.LookAt(rayShooter, Vector3.up);
            objectTag.transform.GetChild(0).GetComponent<TextMesh>().text = rayName;
            DrawBoundary();
            polyObj = Instantiate(polyObj, hit.point, transform.rotation);
            polyObj.GetComponent<PolyObj>().keywords = rayName;
            didHit = true;
        }
        raycasts += 1;
        if(!didHit && raycasts < maxRaycastRetrys){
            yield return new WaitForSeconds(raycastRetryDelay);
            StartCoroutine("Cast");
        } else {
            // rayShooter.GetComponent<LineRenderer>().enabled = false;
            yield break;
        }
    }
    void DrawBoundary() {
        boundary = objectTag.transform.Find("Boundary").GetComponent<LineRenderer>();
        boundary.enabled = true;
        Debug.Log("DrawBoundary");
        Vector3[] points = new Vector3[vertices.Count];
        for (int i = 0; i < vertices.Count; i++) {
            Vector2 screenCoordinate = GetScreenCoordinate(vertices[i].x, vertices[i].y);
            ray = arCamera.ScreenPointToRay(screenCoordinate);
            transform.parent = transform.root;
            transform.position = arCamera.transform.position;
            transform.rotation = arCamera.transform.rotation;
            // ...and rotate the child ray object towards the ray
            rayShooter.rotation = Quaternion.LookRotation(ray.direction, Vector3.up);
            // Now make the raycaster parent a child of the emitter object and reset its position and rotation
            transform.parent = emitter;
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localEulerAngles = new Vector3(0, 0, 0);
            // yield return new WaitForSeconds(0.5f);
            if (Physics.Raycast(rayShooter.position, rayShooter.forward, out boundaryHit, 10.0f, 1 << LayerMask.NameToLayer("TagPlane")))
            {
                // Convert the hit position to a position local to the boundary object.
                // LineRenderer points are relative to itself
                points[i] = boundary.transform.InverseTransformPoint(boundaryHit.point);
            }
        }
        boundary.positionCount = vertices.Count;
        boundary.SetPositions(points);
        objectTag.transform.Find("Plane").gameObject.SetActive(false);
    }
}
