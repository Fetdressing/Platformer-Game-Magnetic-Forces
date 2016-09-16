﻿using UnityEngine;
using System.Collections;

public class UVDisplacer : MonoBehaviour {
    public float scrollSpeedU = 0.5F;
    public float scrollSpeedV = 0.5F;
    private Renderer rend;
    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    void Update()
    {
        float offsetU = Time.time * scrollSpeedU;
        float offsetV = Time.time * scrollSpeedV;

        rend.material.SetTextureOffset("_MainTex", new Vector2(offsetU, offsetV));
    }
}
