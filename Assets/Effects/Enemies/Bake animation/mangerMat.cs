using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
public class mangerMat : MonoBehaviour
{
    //Component
    private Renderer _renderMutant;
    private bool swipe=false;
    private bool cool = false;
  //  private NavMeshAgent agent;
    //Game Object
    public Transform objMutant;

    public Texture2D texSwiping_pos;
    public Texture2D texSwiping_norm;

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

    //Length of animation
    //swiping 7.8
    //walking 4.3
    //dying 13.8



    // Start is called before the first frame update
    void Start()
    {
     
        //Get Component
        _renderMutant = objMutant.GetComponent<Renderer>();

        //Change Mutant Mat
        _renderMutant.material.SetTexture("_PosTex", texWalking_pos);
        _renderMutant.material.SetTexture("_NmlTex", texWalking_norm);
        _renderMutant.material.SetFloat("_Length", Len_walking);

    }

    // Update is called once per frame
    void Update()
    {
        
        float distance = Vector3.Distance(objMutant.position, player.position);
        
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("스페이스키 누름");
            _renderMutant.material.SetTexture("_PosTex", texDying_pos);
            _renderMutant.material.SetTexture("_NmlTex", texDying_norm);
            _renderMutant.material.SetFloat("_Length", Len_dying);
        }
        else if ((distance < 3.1 || swipe || distance2 < 3.1) && !cool)
        {   
            if (!swipe)
            {   
                during_time += Time.deltaTime;
                swipe = true;
                _renderMutant.material.SetFloat("_DT", 0);
                _renderMutant.material.SetTexture("_PosTex", texSwiping_pos);
                _renderMutant.material.SetTexture("_NmlTex", texSwiping_norm);
                _renderMutant.material.SetFloat("_Length", Len_swiping);
                
            }
            else
            {
                during_time += Time.deltaTime;
                _renderMutant.material.SetFloat("_DT", during_time);
                _renderMutant.material.SetTexture("_PosTex", texSwiping_pos);
                _renderMutant.material.SetTexture("_NmlTex", texSwiping_norm);
                _renderMutant.material.SetFloat("_Length", Len_swiping);

                if (during_time >= 5)
                {
                    swipe = false;
                    during_time = 0;
                    cool = true;
                    cool_time = 300;
                }

            }        

        }
        else
        {
            _renderMutant.material.SetFloat("_DT", Time.time);
            _renderMutant.material.SetTexture("_PosTex", texWalking_pos);
            _renderMutant.material.SetTexture("_NmlTex", texWalking_norm);
            _renderMutant.material.SetFloat("_Length", Len_walking);
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
}
*/