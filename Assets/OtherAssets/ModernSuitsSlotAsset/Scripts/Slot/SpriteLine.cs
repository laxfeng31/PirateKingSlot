using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLine : MonoBehaviour
{
    public LineTextureMode textureMode = LineTextureMode.Tile;
    public float tileAmount = 0f;
    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        
    }

    void Update()
    {
        lr.textureMode = textureMode;
        lr.material.SetTextureScale("_MainTex", new Vector2(tileAmount, 1.0f));
    }

    //void OnGUI()
    //{
    //    textureMode = GUI.Toggle(new Rect(25, 25, 200, 30), textureMode == LineTextureMode.Tile, "Tiled") ? LineTextureMode.Tile : LineTextureMode.Stretch;

    //    if (textureMode == LineTextureMode.Tile)
    //    {
    //        GUI.Label(new Rect(25, 60, 200, 30), "Tile Amount");
    //        tileAmount = GUI.HorizontalSlider(new Rect(125, 65, 200, 30), tileAmount, 0.1f, 4.0f);
    //    }
    //}
}
