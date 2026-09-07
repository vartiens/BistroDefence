using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Custom
using UnityEngine.XR;
using Photon.Pun;
//using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class NetworkPlayer : MonoBehaviour
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public Animator leftHandAnimator;
    public Animator rightHandAnimator;

    private PhotonView photonView;

    private Transform headOrigin;
    private Transform leftHandOrigin;
    private Transform rightHandOrigin;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        // the 'XRRig' has been replaced with the 'XROrigin'.
        XROrigin origin = FindObjectOfType<XROrigin>();
        headOrigin = origin.transform.Find("Camera Offset/Main Camera");
        leftHandOrigin = origin.transform.Find("Camera Offset/Left Controller");
        rightHandOrigin = origin.transform.Find("Camera Offset/Right Controller");

        if (photonView.IsMine)
        {
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                item.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            MapPosition(head, headOrigin);
            MapPosition(leftHand, leftHandOrigin);
            MapPosition(rightHand, rightHandOrigin);

            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand), leftHandAnimator);
            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.RightHand), rightHandAnimator);
        }
    }

    void MapPosition(Transform target, Transform originTransform)
    {
        target.position = originTransform.position;
        target.rotation = originTransform.rotation;
    }

    void UpdateHandAnimation(InputDevice targetDevice, Animator handAnimator)
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }
}
