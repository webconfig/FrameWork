using System;
using UnityEngine;
using System.Collections.Generic;

public class collider_triger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (App.Instance._run==1)
        {
            App.Instance.appdomain.Invoke("HotFix_Project.Main", "GameTriggerEnter",null, gameObject.name,other.gameObject.name);
        }
    }
}
