using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseButton : MonoBehaviour
{
    public static UseButton S;
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
