using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Radar : MonoBehaviour
{
    [SerializeField] ChickenSpawner chickenSpawner;
    [SerializeField] RawImage radarScreen;
    [SerializeField] PlayerController2 player;
    Texture2D radarTex;
    const float RADAR_DIAMETER = 30.0f;
    const float RADAR_RADIUS = RADAR_DIAMETER / 2.0f;
    int texWidth = 64;
    int texHeight = 64;
    Color32[] buffer;

    // Start is called before the first frame update
    void Start()
    {
        // Copy radar screen texture to editable and reassign it to radar screen
        radarTex = new Texture2D(texWidth, texHeight);
        radarScreen.texture = radarTex;
        buffer = new Color32[texWidth * texHeight];
        
    }

    // Update is called once per frame
    void Update()
    {
        List<GameObject> chickenList = chickenSpawner.GetChickenList();
        //SetColourArray(buffer, Color.white);
        ResetColourArray(buffer);

        //print("Buffer length: " + radarTex.width +", "+ radarTex.height);

        foreach (GameObject chicken in chickenList) {
            // put chickens in radar space (unscaled)
            float radarX = chicken.transform.position.x - player.transform.position.x;
            float radarZ = chicken.transform.position.z - player.transform.position.z;

            if (radarX*radarX + radarZ*radarZ < RADAR_RADIUS * RADAR_RADIUS) {
                float x = radarX / RADAR_DIAMETER * texWidth + (texWidth / 2);
                float z = radarZ / RADAR_DIAMETER * texHeight + (texHeight / 2);

                if ((int)x + (int)z * radarTex.width > buffer.Length) {
                    print("out of range (" + x + ", (" + z + ")");
                }
                else {
                    //buffer[(int)x + (int)z * radarTex.width] = Color.red;

                    //AddDotToColourBuffer((int)x,   (int)z-1, Color.red);
                    //AddDotToColourBuffer((int)x-1, (int)z, Color.red);
                    ////AddDotToColourBuffer((int)x,   (int)z, Color.red);
                    //AddDotToColourBuffer((int)x+1, (int)z, Color.red);
                    //AddDotToColourBuffer((int)x,   (int)z+1, Color.red);

                    AddCircleToColourBuffer((int)x, (int)z, Color.red);
                }
            }

            //buffer[(radarTex.width / 2 + 0) + (radarTex.height / 2 + 0) * radarTex.width] = Color.black;
            //buffer[(radarTex.width / 2 + 1) + (radarTex.height / 2 + 0) * radarTex.width] = Color.black;
            //buffer[(radarTex.width / 2 + 0) + (radarTex.height / 2 + 1) * radarTex.width] = Color.black;
            //buffer[(radarTex.width / 2 + 1) + (radarTex.height / 2 + 1) * radarTex.width] = Color.black;
        }

        radarTex.SetPixels32(buffer);
        radarTex.Apply();
    }

    void CopyTexture(Texture2D dest, Texture2D src) {
        dest.SetPixels(src.GetPixels());
    }

    void SetColourArray(Color[] dest, Color colour)
    {
        for (int i = 0; i < dest.Length; i++) {
            if (!dest[i].Equals(Color.black))
            dest[i] = colour;
        }
    }

    void ResetColourArray(Color32[] dest)
    {
        for (int i = 0; i < dest.Length; i++) {
            
            dest[i].r = 0;
            dest[i].g = 0;
            dest[i].b = 0;
            dest[i].a = 0;

        }
    }

    void AddDotToColourBuffer(int x, int y, Color32 colour)
    {
        if (x >= 0 && x < texWidth && y >= 0 && y < texHeight) {
            buffer[x + y * radarTex.width] = colour;
        } 
    }

    void AddCircleToColourBuffer(int x, int y, Color32 colour)
    {
        AddDotToColourBuffer(x-1, y - 3, colour);
        AddDotToColourBuffer(x,   y - 3, colour);
        AddDotToColourBuffer(x+1, y - 3, colour);

        AddDotToColourBuffer(x-1, y + 3, colour);
        AddDotToColourBuffer(x,   y + 3, colour);
        AddDotToColourBuffer(x+1, y + 3, colour);

        AddDotToColourBuffer(x - 2, y - 2, colour);
        AddDotToColourBuffer(x + 2, y - 2, colour);

        AddDotToColourBuffer(x - 2, y + 2, colour);
        AddDotToColourBuffer(x + 2, y + 2, colour);

        AddDotToColourBuffer(x - 3, y - 1, colour);
        AddDotToColourBuffer(x + 3, y - 1, colour);
        
        AddDotToColourBuffer(x - 3, y + 1, colour);
        AddDotToColourBuffer(x + 3, y + 1, colour);

        AddDotToColourBuffer(x - 3, y, colour);
        AddDotToColourBuffer(x + 3, y, colour);


    }
}
