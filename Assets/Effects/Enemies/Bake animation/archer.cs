using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class archer : MonoBehaviour
{
    public GameObject arrow;

    //Component
    private Renderer _renderArcher;
    private bool shoot = false;
    private bool cool = false;
  //  private NavMeshAgent agent;
    //Game Object
    public Transform player;
    //public Transform player2;
    public Transform objArcher;

    public Texture2D texShooting_pos;
    public Texture2D texShooting_norm;

    public Texture2D texDying_pos;
    public Texture2D texDying_norm;

    public Texture2D texWalking_pos;
    public Texture2D texWalking_norm;

    public float Len_dying = 7;
    public float Len_swiping = 2;
    public float Len_walking = 1;

    //private int dying_time = 138;
    private int cool_time = 300;
    private float during_time = 0;
    private float _time = 0;

    //Length of animation
    //swiping 7.8
    //walking 4.3
    //dying 13.8



    // Start is called before the first frame update
    void Start()
    {
     
        //Get Component
        _renderArcher = objArcher.GetComponent<Renderer>();

        //Change Mutant Mat
        _renderArcher.material.SetTexture("_PosTex", texWalking_pos);
        _renderArcher.material.SetTexture("_NmlTex", texWalking_norm);
        _renderArcher.material.SetFloat("_Length", Len_walking);

    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        if (_time > 1)
        {
            ShootArrow();
            _time = 0;
        }

        float distance = Vector3.Distance(objArcher.position, player.position); 
        //float distance2 = Vector3.Distance(objArcher.position, player2.position); 
        
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("스페이스키 누름");
            _renderArcher.material.SetTexture("_PosTex", texDying_pos);
            _renderArcher.material.SetTexture("_NmlTex", texDying_norm);
            _renderArcher.material.SetFloat("_Length", Len_dying);
        }
        else if ((distance < 20 || shoot /*|| distance2 < 20*/) && !cool)
        {   
            if (!shoot)
            {   
                during_time += Time.deltaTime;
                shoot = true;
                _renderArcher.material.SetFloat("_DT", 0);
                _renderArcher.material.SetTexture("_PosTex", texShooting_pos);
                _renderArcher.material.SetTexture("_NmlTex", texShooting_norm);
                _renderArcher.material.SetFloat("_Length", Len_swiping);
            }
              else
            {
                during_time += Time.deltaTime;
                _renderArcher.material.SetFloat("_DT", during_time);
                _renderArcher.material.SetTexture("_PosTex", texShooting_pos);
                _renderArcher.material.SetTexture("_NmlTex", texShooting_norm);
                _renderArcher.material.SetFloat("_Length", Len_swiping);

                if (during_time >= 5)
                {
                    shoot = false;
                    during_time = 0;
                    cool = true;
                    cool_time = 300;
                }

            }        

        }
        else
        {
            _renderArcher.material.SetFloat("_DT", Time.time);
            _renderArcher.material.SetTexture("_PosTex", texWalking_pos);
            _renderArcher.material.SetTexture("_NmlTex", texWalking_norm);
            _renderArcher.material.SetFloat("_Length", Len_walking);
            if (cool)
            {
                cool_time--;
                if(cool_time <= 0)
                {
                    cool = false;
                }
            }
        }
    }

    private GameObject FindClosestPlayer()
    {
        Debug.Log($"{name} is now firing");
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

        Debug.Log($"We have {targets.Length} targets");
        GameObject target = targets[0];
        float dist = 999999999.0f;
        foreach (GameObject tt in targets)
        {
            float newDist = Vector3.Distance(tt.transform.position, transform.position);
            if (newDist < dist)
            {
                dist = newDist;
                target = tt;
            }
        }

        return target;
    }

    private void ShootArrow()
    {
        GameObject target = FindClosestPlayer();
        
        Vector3 spawn_position = transform.position + transform.TransformDirection(Vector3.up) * 2.25f + transform.TransformDirection(Vector3.forward) * -0.4f;
        Vector3 direction = (target.transform.position - spawn_position).normalized;

        Instantiate(arrow, spawn_position, Quaternion.LookRotation(direction));
    }
}
