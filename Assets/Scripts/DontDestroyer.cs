using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyer : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
