using UnityEngine;
using System.Collections;

public class TunnelTrail : MonoBehaviour
{

    public LineTextureMode textureMode = LineTextureMode.Tile;
    public float tileAmount = 1.0f;
    private TrailRenderer tr;

    void Start()
    {
        tr = GetComponent<TrailRenderer>();
    }

    void Update()
    {

        tr.textureMode = textureMode;
        tr.material.SetTextureScale("_MainTex", new Vector2(tileAmount, 1.0f));
        tr.material.SetTextureOffset("_MainTex", new Vector2(Time.timeSinceLevelLoad * 10f, 0f));
    }

}
