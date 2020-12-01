using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    public Material standardMaterial;
    public Material outlineMaterial;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OutlineItem() {
        gameObject.GetComponent<MeshRenderer>().material = outlineMaterial;
    }

    public void RestoreStandardMaterial() {
        gameObject.GetComponent<MeshRenderer>().material = standardMaterial;
    }
}
