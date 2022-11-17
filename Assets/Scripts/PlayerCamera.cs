using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float zoomOutSpeed = 1.0f;
    Vector3 distance;
    const float ROOT_3 = 1.7320508075688772935274463415059f;
    float yTarget = 3.0f, zTarget = -3.0f;
    float zoomOutAmount = 0.05f;//0.01f;//0.3f;//0.05f;
    float yTargetOld, zTargetOld;
    bool resetting;

    // Start is called before the first frame update
    void Start()
    {
        distance = new Vector3(0.0f, yTarget, zTarget);

        yTargetOld = yTarget;
        zTargetOld = zTarget;

        resetting = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + distance; ;
        //transform.LookAt(player.transform.position+new Vector3(0.0f,2.0f,0.0f));

        if (!resetting) {
            if (distance.y < yTarget) {
                distance.y += zoomOutSpeed * Time.deltaTime;
            }
            else {
                distance.y = yTarget; // to avoid overshoot
            }

            if (distance.z > zTarget) {
                distance.z -= zoomOutSpeed * ROOT_3 * Time.deltaTime;
            }
            else {
                distance.z = zTarget; // to avoid overshoot
            }
        }
        else { // resetting!
            if (distance.y > yTarget) {
                distance.y -= zoomOutSpeed * Time.deltaTime;
            }
            else {
                distance.y = yTarget; // to avoid overshoot
            }

            if (distance.z < zTarget) {
                distance.z += zoomOutSpeed * ROOT_3 * Time.deltaTime;
            }
            else {
                distance.z = zTarget; // to avoid overshoot
                resetting = false;
            }
        }
    }

    public void IncreaseDistance()
    {
        yTarget += zoomOutAmount;
        zTarget -= zoomOutAmount * ROOT_3;
    }

    public void IncreaseDistance(float customZoom)
    {
        yTarget += customZoom;
        zTarget -= customZoom * ROOT_3;
    }

    public void SetDistance(float distance)
    {
        yTarget = distance;
        zTarget = -distance * ROOT_3; 
    }

    public void SetDistance(float yDistance, float zDistance)
    {
        yTarget = yDistance;
        zTarget = -zDistance;
    }

    public void Reset()
    {
        yTarget = yTargetOld;
        zTarget = zTargetOld;

        resetting = true;
    }
}
