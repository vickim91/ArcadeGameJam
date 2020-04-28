using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAllVoices : MonoBehaviour
{
    public bool killAllVoicesGlobal;
    public static bool killAllVoicesStatic;
    private float timer;

    private void Update()
    {
        if (killAllVoicesGlobal)
        {
            killAllVoicesStatic = true;
        }

        timer += Time.deltaTime;
        if (timer > 0.1f)
        {
            timer = 0;
            killAllVoicesStatic = false;
            killAllVoicesGlobal = false;
        }
    }
}