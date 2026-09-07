using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Custom
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class NetworkPlayerSpawn : MonoBehaviourPunCallbacks
{
    public InputHelpers.Button inputHelpers = InputHelpers.Button.MenuButton;
    public XRNode controller = XRNode.LeftHand;
    
    private GameObject spawnedPlayerPrefab;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, transform.rotation);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }

    /***** No Use
    // Start is called before the first frame update
    void Start()
    {
        
    }
    *****/

    // Update is called once per frame
    void Update()
    {
        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(controller), inputHelpers, out bool isPressed);

        if (isPressed)
        {
            PhotonNetwork.Disconnect();
            PhotonNetwork.LoadLevel(0);
        }
    }
}
