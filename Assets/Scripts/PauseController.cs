using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(Global.PAUSE_KEY)) {
            if (isPaused) {
                isPaused = false;
                Time.timeScale = 1.0f;
                // banish pause menu
            }
            else {
                isPaused = true;
                Time.timeScale = 0.0f;
                // display pause menu
            }
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
