using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class gun_ctrl : MonoBehaviour
{
    [SerializeField] private InputActionReference fireActionReference;

    public GameObject Bullet;
    public ParticleSystem Shoot_effect;
    private bool reroad_flag = true;
    public float reroad_time;

    // Start is called before the first frame update
    void Start()
    {
        fireActionReference.action.performed += Onfire;
    }

    IEnumerator reroad()
    {
        reroad_flag = false;
        yield return new WaitForSeconds(reroad_time);
        reroad_flag = true;
    }

    // Update is called once per frame
    void Update()
    {
        //OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger)

    }

    private void Onfire(InputAction.CallbackContext obj)
    {
        if (reroad_flag)
        {
            Vector3 spawn_position = transform.position + transform.TransformDirection(Vector3.up) * 0.1285f + transform.TransformDirection(Vector3.back) * 0.13f;
            Instantiate(Bullet, spawn_position, Quaternion.Euler(transform.eulerAngles));
            Instantiate(Shoot_effect, spawn_position, Quaternion.Euler(transform.eulerAngles + new Vector3(0.0f, 90.0f, 0.0f)));
            StartCoroutine(reroad());
        }
    }
}
