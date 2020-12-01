using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colortest : MonoBehaviour
{
    Color mycolor;

    private Color[] Colors = new Color[6] { new Color(1, 0, 0, 1), new Color(0, 1, 0, 1), new Color(0, 0, 1, 1), new Color(1, 1, 0, 1), new Color(0.5f, 0, 1, 1), new Color(0, 1, 1, 1) };
    public short buba = 1;

    [Range(0.0f, 1.0f)]
    public float red;
    [Range(0.0f, 1.0f)]
    public float green;
    [Range(0.0f, 1.0f)]
    public float blue;

    void Update()
    {
        mycolor = new Color(red, green, blue, 1);
        GetComponent<MeshRenderer>().material.color = Colors[buba];
    }
}
