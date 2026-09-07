using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Custom

public class MirrorMovement : MonoBehaviour
{
    public Transform playerTarget;
    public Transform mirror; // Mirror Folder (Parent)

    /***** No Use
    private Camera mirrorCamera;

    // Start is called before the first frame update
    void Start()
    {
        mirrorCamera = GameObject.Find("Restaurant Mirror Camera").GetComponent<Camera>();
    }
    *****/

    // Update is called once per frame
    void Update()
    {
        // For Global Position of Mirror Camera
        Vector3 localPlayer = mirror.InverseTransformPoint(playerTarget.position);
        //transform.position = mirror.TransformPoint(new Vector3(localPlayer.x, localPlayer.y, -localPlayer.z));

        // For Global Direction of Mirror Camera
        Vector3 lookAtMirror = mirror.TransformPoint(new Vector3(-localPlayer.x, localPlayer.y, localPlayer.z));
        transform.LookAt(lookAtMirror);

        /***** No Use
        // For Near of Clipping Planes of Mirror Camera
        float distance = Vector3.Distance(Vector3.zero, localPlayer);

        if (distance < 0.01f)
        {
            mirrorCamera.nearClipPlane = 0.01f;
        }
        else
        {
            mirrorCamera.nearClipPlane = distance;
        }
        *****/
    }
}
