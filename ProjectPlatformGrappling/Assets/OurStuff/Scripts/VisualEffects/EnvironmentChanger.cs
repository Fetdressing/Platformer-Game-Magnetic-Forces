using UnityEngine;
using System.Collections;

public class EnvironmentChanger : BaseClass {

    public Color[] fogColors;
    private int currFogColorIndex = 0;
    private Color currColor;
    public float fogColorChangingSpeed = 0.8f;
    
	// Use this for initialization
	void Start () {
        Init();
	}

    public override void Init()
    {
        base.Init();
        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        NextFogColor();
    }

    // Update is called once per frame
    void Update () {

        if (fogColors.Length != 0)
        {
            Color currFogColor = RenderSettings.fogColor;

            if(Mathf.Abs(Mathf.Abs(currFogColor.r - currColor.r) + Mathf.Abs(currFogColor.g - currColor.g) + Mathf.Abs(currFogColor.b - currColor.b)) < 0.01f) //kolla ifall färgerna typ är lika
            {
                //Debug.Log(Time.time.ToString());
                NextFogColor();
            }
            else
            {
                currFogColor = Color.Lerp(currFogColor, currColor, Time.deltaTime * fogColorChangingSpeed);
                //Debug.Log(currFogColor.ToString());
            }
            
            RenderSettings.fogColor = currFogColor;
        }
        //if ((currColor.r - ))
	}

    public void StartfogDensityChange(float dens)
    {
        StartCoroutine(ChangeFogDensity(dens));
    }

    IEnumerator ChangeFogDensity(float density)
    {
        while(Mathf.Abs(RenderSettings.fogEndDistance - density) > 0.1f)
        {
            RenderSettings.fogEndDistance = Mathf.Lerp(RenderSettings.fogEndDistance, density, Time.deltaTime * 0.1f);
            yield return new WaitForEndOfFrame();
        }
    }

    void NextFogColor()
    {
        currFogColorIndex++;
        if(currFogColorIndex >= fogColors.Length)
        {
            currFogColorIndex = 0;
        }

        currColor = fogColors[currFogColorIndex];
    }
}
