using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeRotation : MonoBehaviour
{
    private Transform child;
    void Start()
    {
        child = transform.GetChild(0);
        // THis doesn't seem to be doing anything... but anyway...
        UnityEngine.XR.XRDevice.DisableAutoXRCameraTracking(child.GetComponent<Camera>(), true);
    }

    void LateUpdate()
    {
        // For the scope of this project we didn't manage to disable the VR cameras rotation.
        // The VR cameras documentation doesn't take into account that anyone would want a 
        // non-rotating VR camera. Therefore we are doing this "hack" where we rotate
        // this parent of the VR camera with the negative rotation of the child effectively
        // zeroing out the rotation.
        transform.localRotation = Quaternion.Inverse(child.localRotation);
    }
}
