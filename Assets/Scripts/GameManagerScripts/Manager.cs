using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = FindObjectOfType<Manager>();

        if (instance == null)
        {
            instance = new Manager();

            DontDestroyOnLoad(instance);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
