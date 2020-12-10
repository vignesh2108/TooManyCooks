using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaiterButton : MonoBehaviour
{
    public static WaiterButton S;
    private void Awake()
    {
        if (S == null)
        {
            S = this;
        } else if (S != this)
        {
            Destroy(this);
        }
    }
}
