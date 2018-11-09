using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMap  {
    public Vector3 position;
    public List<GameObject> heatCircle = new List<GameObject>();
    public GameObject heatMap;
    private Color[] savedPix;
    private int brushWeight = 1;
    private Color32 color = new Color(1.0f,0f,0f,0.3f);
    public HeatMap(float x, float y, float z)
    {
        position = new Vector3(x, y, z);
        heatMap = new GameObject();
        heatMap.transform.localPosition = position;
        heatMap.transform.parent = Camera.main.transform;
        heatMap.name = "HeatMap";
    }

    public void addCircle(Vector3 pos)
    {
        GameObject circle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        circle.transform.parent = heatMap.transform;
        circle.transform.localPosition = pos;
        //circle.transform.position = pos;
        circle.transform.localRotation = Quaternion.Euler(90, 0, 0);
        circle.transform.localScale = new Vector3(0.1f,0.001f,0.1f);
        circle.name = "HeatCircle";
        circle.GetComponent<Collider>().enabled = false;
        Material material = circle.GetComponent<Renderer>().material;       
        material.SetFloat("_Mode",2);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		material.SetInt("_ZWrite", 0);
		material.DisableKeyword("_ALPHATEST_ON");
		material.EnableKeyword("_ALPHABLEND_ON");
		material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		material.renderQueue = 3000;
        material.color = new Color(1.0f,0f,0f,0.3f);
        heatCircle.Add(circle);
    }

    public void addHit(RaycastHit hit, Texture2D workingTexture){
        //savedPix = workingTexture.GetPixels(0, 0, workingTexture.width, workingTexture.height);
        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;
        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= workingTexture.width;
        pixelUV.y *= workingTexture.height;

        brush(workingTexture, (int)pixelUV.x, (int)pixelUV.y, color, brushWeight);
    }

    private void brush(Texture2D tex, int x, int y, Color32 color, int size)
    {
        int dim = size * 2 + 1;
        int[] arrayXPos = new int[size];
        int[] arrayXNeg = new int[size];
        int[] arrayYPos = new int[size];
        int[] arrayYNeg = new int[size];
        int[] arrayX = new int[dim];
        int[] arrayY = new int[dim];
        int a = 1;

        for(int i = 0; i < size; i++)
        {
            arrayXPos[i] = x + a;
            arrayYPos[i] = y + a;
            arrayXNeg[i] = x - a;
            arrayYNeg[i] = y - a;
            a++;
        }

        arrayX[0] = x;
        arrayY[0] = y;

        arrayXNeg.CopyTo(arrayX, 1);
        arrayXPos.CopyTo(arrayX, arrayXNeg.Length + 1);

        arrayYNeg.CopyTo(arrayY, 1);
        arrayYPos.CopyTo(arrayY, arrayYNeg.Length + 1);

        for (int i = 0; i < dim; i++)
        {
            for(int z = 0; z < dim; z++)
            {
                tex.SetPixel(arrayX[i], arrayY[z], color);
            }
        }

        tex.Apply();
    }

    public void setActive(bool active)
    {
        heatMap.SetActive(active);
    }
}
