using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolyToolkit;

/// <summary>
/// Load in a random object from the Poly library
/// </summary>

public class PolyFetch : MonoBehaviour {
    // public Text statusText;
    public PolyObj polyObj;
    public float height = 0.1f;
    void Start()
    {
      // RandomPoly("chair");
    }
    public void RandomPoly(string keywords) {
      PolyListAssetsRequest req = new PolyListAssetsRequest();
      // Search by keyword:
      req.keywords = keywords;
      // Only curated assets:
      req.curated = true;
      // Limit complexity to medium.
      req.maxComplexity = PolyMaxComplexityFilter.SIMPLE;
      // Only Blocks objects.
      req.formatFilter = PolyFormatFilter.BLOCKS;
      // Order from best to worst.
      req.orderBy = PolyOrderBy.BEST;
      // Up to 20 results per page.
      req.pageSize = 100;

      PolyApi.ListAssets(req, PolyListCallback);
    }

    void PolyListCallback(PolyStatusOr<PolyListAssetsResult> result) {
      if (!result.Ok) {
        // Handle error.
        return;
      }
      // Debug.Log("RESULT 1:");
      // Debug.Log(result);
      // Debug.Log("RESULT 1 VALUE:");
      // Debug.Log(result.Value);
      int assetCount = result.Value.assets.Count;
      int randomItem = Random.Range(0, assetCount);
      string asset = result.Value.assets[randomItem].name;
      polyObj.polyAsset = asset;
      GetPoly(asset);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GetPoly(string asset) {
        // Debug.Log("Requesting asset...");
        // Debug.Log("ASSET:");
        // Debug.Log(asset);
        PolyApi.GetAsset(asset, GetAssetCallback);
        // statusText.text = "Requesting...";
    }
    private void GetAssetCallback(PolyStatusOr<PolyAsset> result) {
    if (!result.Ok) {
      Debug.LogError("Failed to get assets. Reason: " + result.Status);
      // statusText.text = "ERROR: " + result.Status;
      return;
    }
    // Debug.Log("RESULT 2:");
    // Debug.Log(result);
    // Debug.Log("RESULT 2 VALUE:");
    // Debug.Log(result.Value);
    // Debug.Log("Successfully got asset!");

    // Set the import options.
    PolyImportOptions options = PolyImportOptions.Default();
    // We want to rescale the imported mesh to a specific size.
    options.rescalingMode = PolyImportOptions.RescalingMode.FIT;
    // The specific size we want assets rescaled to (fit in a 5x5x5 box):
    options.desiredSize = height;
    // We want the imported assets to be recentered such that their centroid coincides with the origin:
    options.recenter = true;

    // statusText.text = "Importing...";
    PolyApi.Import(result.Value, options, ImportAssetCallback);
  }
  private void ImportAssetCallback(PolyAsset asset, PolyStatusOr<PolyImportResult> result) {
    if (!result.Ok) {
      Debug.LogError("Failed to import asset. :( Reason: " + result.Status);
      // statusText.text = "ERROR: Import failed: " + result.Status;
      return;
    }
    // Debug.Log("Successfully imported asset!");

    // Show attribution (asset title and author).
    // statusText.text = asset.displayName + "\nby " + asset.authorName;

    // Here, you would place your object where you want it in your scene, and add any
    // behaviors to it as needed by your app. As an example, let's just make it
    // slowly rotate:
    // result.Value.gameObject.AddComponent<Rotate>();
    result.Value.gameObject.transform.parent = transform;
    result.Value.gameObject.transform.localPosition = new Vector3(0,0,0);
    result.Value.gameObject.transform.localEulerAngles = new Vector3(0,0,0);
  }
}
