using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager S;
    public GameObject sceneObjectPrefab;
    public bool isMobile;
    private void Awake()
    {
        if (S == null)
        {
            isMobile = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
            S = this;
        } else if (S != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
