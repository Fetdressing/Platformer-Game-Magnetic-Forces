using UnityEngine;
using System.Collections;

public class BaseClass : MonoBehaviour {
    protected int initTimes = 0;
    [HideInInspector]
    public bool isLocked;
    protected bool bActivated; //för att kolla med deactivate funktionen
    public virtual void Init()
    {
        //initTimes++; //jag vill kanske använda den som jag själv vill i varje class
    }
    public virtual void Dealloc() { }
    public virtual void Reset() { }
    public virtual void UpdateLoop() { }
    public virtual void Deactivate() { }
}
