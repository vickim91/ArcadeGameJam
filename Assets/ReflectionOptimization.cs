using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionOptimization : MonoBehaviour
{

    ReflectionProbe refProbe;
    public ReflectionProbe refProbe2;


    void Start()
    {
        refProbe = GetComponentInParent<ReflectionProbe>();
        refProbe.RenderProbe();
        refProbe2.RenderProbe();
        StartCoroutine(Timer());
        StartCoroutine(Timer2());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(3.2f);
        RenderReflection();
    }
    IEnumerator Timer2()
    {
        yield return new WaitForSeconds(2.3f);
        RenderReflection2();
    }


    private void RenderReflection()
    {
        refProbe.RenderProbe();
        StartCoroutine(Timer());
    }

    private void RenderReflection2()
    {
        refProbe.RenderProbe();
        StartCoroutine(Timer2());
    }
}
