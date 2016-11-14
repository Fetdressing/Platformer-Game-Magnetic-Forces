﻿using UnityEngine;
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
    [HideInInspector]
    public static float maxDeltaTime = 0.02f; //annars kan man få skumma värden om en frame varar för länge
    [HideInInspector] public static float deltaTime = 0.01f; //räknas ut sen i GameManager, den får ett startvärde så den inte ska vara null just
}
