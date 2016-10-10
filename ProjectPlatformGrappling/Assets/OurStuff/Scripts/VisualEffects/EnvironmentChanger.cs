using UnityEngine;
using System.Collections;

public class EnvironmentChanger : MonoBehaviour {
    public Color[] fogColors;
    private int currFogColorIndex = 0;
    private Color currColor;
    public float fogColorChangingSpeed = 0.8f;
	// Use this for initialization
	void Start () {
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
