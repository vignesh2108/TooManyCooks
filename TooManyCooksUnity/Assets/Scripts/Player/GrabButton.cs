using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabButton : MonoBehaviour
{
    public static GrabButton S;
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
