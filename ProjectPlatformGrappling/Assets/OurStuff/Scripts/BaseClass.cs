using UnityEngine;
using System.Collections;

public class BaseClass : MonoBehaviour {
    [HideInInspector]
    public int initTimes = 0;
    public virtual void Init()
    {
        //initTimes++; //jag vill kanske använda den som jag själv vill i varje class
    }
    public virtual void Dealloc() { }
    public virtual void Reset() { }
}
