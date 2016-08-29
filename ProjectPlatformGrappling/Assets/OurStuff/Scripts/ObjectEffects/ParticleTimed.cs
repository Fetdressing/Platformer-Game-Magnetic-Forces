using UnityEngine;
using System.Collections;

public class ParticleTimed : MonoBehaviour {
    private GameObject thisObject;
    private ParticleSystem ps;
    public float lifeTime = 3;

    [HideInInspector]
    public bool isReady = true;
	// Use this for initialization
	void Start () {
        thisObject = this.gameObject;
        ps = thisObject.GetComponent<ParticleSystem>();
        isReady = true;
    }
    void Awake()
    {
        thisObject = this.gameObject;
        ps = thisObject.GetComponent<ParticleSystem>();
        isReady = true;
    }
	
	public void StartParticleSystem()
    {
        thisObject.SetActive(true);
        ps.Play();
        StartCoroutine(RunParticleSystem());
    }
    IEnumerator RunParticleSystem()
    {
        isReady = false;
        yield return new WaitForSeconds(lifeTime);
        isReady = true;
        thisObject.SetActive(false);
    }
}
