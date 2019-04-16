using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SixDegrees;
// using Artngame.PDM;
public class MeshParticles : MonoBehaviour
{
    SDMesh sdMesh;
    // SKinColoredMasked sKinColoredMasked;
    ParticleSystem ps;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        sdMesh = FindObjectOfType<SDMesh>();
        // sKinColoredMasked = GetComponent<SKinColoredMasked>();
        InvokeRepeating("SetMeshes", 3.0f, 3.0f);
        StartCoroutine("SetEmission");
    }

    void Update()
    {
        
    }
    void SetMeshes() {
        Debug.Log("SetMeshes()");
        MeshFilter[] meshFilters = sdMesh.GetComponentsInChildren<MeshFilter>();
        // Debug.Log(meshFilters.Length);
        CombineInstance[] instance = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            // Debug.Log("combining.."+meshFilters[i].gameObject.transform.name);
            instance[i].mesh = meshFilters[i].sharedMesh;
            instance[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }
        // Debug.Log(instance);
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(instance);
        // Debug.Log(combinedMesh);
        var shape = ps.shape;
        shape.mesh = combinedMesh;
        // Debug.Log(shape.mesh);
        // sKinColoredMasked.meshesList.AddRange(sdMesh.GetComponentsInChildren<MeshFilter>());
    }
    IEnumerator SetEmission () {
        yield return new WaitForSeconds(3.1f);
        var emission = ps.emission;
        emission.rateOverTime = 500f;
    }

}
