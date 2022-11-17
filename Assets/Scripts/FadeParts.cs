using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeParts : MonoBehaviour
{
    [SerializeField] float timeToFadeOut; // seconds
    [SerializeField] GameObject[] disappearOnFade;
    List<MeshRenderer> parts;
    bool fading = false;
    float currentAlpha = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        parts = new List<MeshRenderer>();
        BuildBodyTree(transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (!fading) return;

        //foreach (MeshRenderer r in parts) {
        //    if (r) {
        //        Color c = r.material.color;
        //        c.a -= Time.deltaTime / timeToFadeOut;
        //        //c.a = 0.5f;

        //        if (c.a < 0.0f) c.a = 0.0f;

        //        // cheat and make pupils disappear
        //        if (r.name == "PupilL" || r.name == "PupilR") {
        //            c.a = 0.0f;
        //        }

        //        r.material.color = c;
        //    }
        //}

        currentAlpha -= Time.deltaTime / timeToFadeOut;

        if (currentAlpha < 0.0f) {
            currentAlpha = 0.0f;
            fading = false;
        }

        SetAlpha(currentAlpha);

        //foreach (GameObject part in disappearOnFade) {
        //    SetPartAlpha(part, 0.0f);
        //}
                // cheat and make pupils disappear
                //if (r.name == "PupilL" || r.name == "PupilR") {
                //    c.a = 0.0f;
                //}            
        
    }

    void BuildBodyTree(Transform t)
    {
        for (int i = 0; i < t.childCount; i++) {
            BuildBodyTree(t.GetChild(i));
        }

        MeshRenderer MeshRenderer = t.GetComponent<MeshRenderer>();

        if (MeshRenderer) {
            //print(t.name + " has a renderer");
            parts.Add(MeshRenderer);
        }
        else {
            //print(t.name + " doesn't have a renderer");
        }

        
    }

    public void StartFading()
    {
        fading = true;
        //print("Start Fading");
        
        foreach (GameObject part in disappearOnFade) {
            Destroy(part);
        }
    }

    void SetAlpha(float alpha)
    {
        foreach (MeshRenderer r in parts) {
            if (r) {
                Color c = r.material.color;
                
                c.a = alpha;
                r.material.color = c;
            }
        }
    }

    void SetPartAlpha(GameObject part, float alpha)
    {
        MeshRenderer r = part.GetComponent<MeshRenderer>();

        if (r) {
            Color c = r.material.color;

            c.a = alpha;
            r.material.color = c;
        }
    }
}
