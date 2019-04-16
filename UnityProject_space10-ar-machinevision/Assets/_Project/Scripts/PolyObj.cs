using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolyObj : MonoBehaviour
{
    // public string title;
    public string polyAsset;
    public string keywords;
    // private Canvas canvas;
    // private Text titleObj;
    private PolyFetch polyObject;
    // Start is called before the first frame update
    void Awake()
    {
        // canvas = transform.Find("Canvas").GetComponent<Canvas>();
        // titleObj = canvas.transform.Find("ID").GetComponent<Text>();
        polyObject = transform.Find("PolyFetch").GetComponent<PolyFetch>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateContent() {
        // titleObj.text = "ID: " + title;
        /* if (polyAsset.Length > 0) {
            polyObject.GetPoly(polyAsset);
        } */
    }
    void Start() {
        // const string glyphs= "abcdefghijklmnopqrstuvwxyz0123456789";
        /* string myString = "";
        for(int i = 0; i < 6; i++)
        {
            myString += glyphs[Random.Range(0, glyphs.Length)];
        }
        titleObj.text = "ID: " + myString; */
        polyObject.RandomPoly(keywords);
    }
}
