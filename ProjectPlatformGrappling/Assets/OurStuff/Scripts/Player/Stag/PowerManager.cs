using UnityEngine;
using System.Collections;

public class PowerManager : BaseClass {
    public Renderer hornRenderer;
    private float[] uvStartOffsetHorns = { 0, -2};

    private float maxPower = 100;
    private float currPower;
	// Use this for initialization
	void Start () {
	
	}

    public override void Init()
    {
        base.Init();
        currPower = maxPower;
    }
    // Update is called once per frame
    void Update () {
        AddPower(-10 * Time.deltaTime);
	}

    public void AddPower(float p)
    {
        currPower += p;

        if(currPower > maxPower)
        {
            currPower = maxPower;
        }

        float offsetV = currPower / maxPower;

        hornRenderer.material.SetTextureOffset("_MainTex", new Vector2(uvStartOffsetHorns[0], uvStartOffsetHorns[1] - offsetV));
    }
}
