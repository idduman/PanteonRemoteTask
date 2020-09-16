using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paint : MonoBehaviour
{
    [HideInInspector]
    public bool isEnabled = false;
    public int counter;
    public int total;

    private Mesh mesh;

    void Start()
    {
        isEnabled = true;
        counter = 0;
        mesh = GetComponent<MeshFilter>().mesh;
        Color[] colors = new Color[mesh.vertices.Length];
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            colors[i] = Color.white;
        }
        mesh.colors = colors;
        total = mesh.colors.Length;
        //Debug.Log("Colors size = " + mesh.colors.Length);
    }

    void Update()
    {
        if (isEnabled && Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "PaintObject")
                {
                    int index = GetVertexIndex(hit, mesh.triangles);
                    //Debug.Log("Vertex index: " + index);
                    ChangeColor(index);
                }
            }
        }
    }

    public void ChangeColor(int index)
    {
        Color[] colors = mesh.colors;
        if (colors[index] != Color.red)
        {
            colors[index] = Color.red;
            counter++;
            mesh.colors = colors;
        }
    }

    public static int GetVertexIndex(RaycastHit hit, int[] triangles)
    {
        var b = hit.barycentricCoordinate;
        int index = hit.triangleIndex * 3;
        if (triangles == null || index < 0 || index + 2 >= triangles.Length)
            return -1;
        if (b.x > b.y)
        {
            if (b.x > b.z)
                return triangles[index]; // x
            else
                return triangles[index + 2]; // z
        }
        else if (b.y > b.z)
            return triangles[index + 1]; // y
        else
            return triangles[index + 2]; // z
    }
}
