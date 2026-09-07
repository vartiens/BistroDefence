using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class EnemyControl : MonoBehaviour
{
    // Enemy HP
    public int maxHP;
    private int currentHP;

    //Component
    private Renderer _renderEnemy;
    private bool attack = false;
    private bool cool = false;
    private bool dead = false;

    //Game Object
    public Texture2D texAttacking_pos;
    public Texture2D texAttacking_norm;
    public float attackDist;

    public Texture2D texDying_pos;
    public Texture2D texDying_norm;

    public Texture2D texWalking_pos;
    public Texture2D texWalking_norm;

    public float lenDying = 7;
    public float lenAttacking = 2;
    public float lenWalking = 1;
    public int attackType; // Melee = 1, Ranged = 2
    public GameObject arrow;
    public float atkCoolDown;

    private float dying_time = 0.0f;
    private float cool_time = 0.0f;
    private float during_time = 0.0f;
    private float _time = 0.0f;

    private Transform player;
    public int meleeDMG;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        Debug.Log($"{gameObject.name}'s Maximum HP: {maxHP}");
     
        //Get Component
        _renderEnemy = GetComponent<Renderer>();
        player = FindClosestPlayer().transform;

        //Change Materials
        _renderEnemy.material.SetTexture("_PosTex", texWalking_pos);
        _renderEnemy.material.SetTexture("_NmlTex", texWalking_norm);
        _renderEnemy.material.SetFloat("_Length", lenWalking);

    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        if (_time > 1.0f) // 1초마다 타겟 탐색 (프레임 단위로 하면 렉걸림)
        {
            player = FindClosestPlayer().transform;
            _time = 0.0f;
        }

        if (dead)
        {
            SendMessage("Dead"); // Archer는 필요함
            GetComponentInParent<NavigationScript>().Dead(); // 사망시 움직임 멈추기
            dying_time += Time.deltaTime;
            _renderEnemy.material.SetFloat("_DT", dying_time);
            _renderEnemy.material.SetTexture("_PosTex", texDying_pos);
            _renderEnemy.material.SetTexture("_NmlTex", texDying_norm);
            _renderEnemy.material.SetFloat("_Length", lenDying);
            if (dying_time > lenDying) Destroy(gameObject);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if ((distance < attackDist || attack ) && !cool)
        {
            if (!attack)
            {   
                during_time = 0;
                attack = true;
                _renderEnemy.material.SetFloat("_DT", 0);
                _renderEnemy.material.SetTexture("_PosTex", texAttacking_pos);
                _renderEnemy.material.SetTexture("_NmlTex", texAttacking_norm);
                _renderEnemy.material.SetFloat("_Length", lenAttacking);
                if (attackType == 2) ShootArrow();
                else if (attackType == 1) player.SendMessage("Attacked", meleeDMG);
            }
              else
            {
                during_time += Time.deltaTime;
                _renderEnemy.material.SetFloat("_DT", during_time);

                if (during_time >= lenAttacking)
                {
                    attack = false;
                    during_time = 0;
                    cool = true;
                    cool_time = 0.0f;
                }

            }        

        }
        else
        {
            if (cool)
            {
                cool_time += Time.deltaTime;
                if(cool_time > atkCoolDown)
                {
                    cool = false;
                }
            }

            _renderEnemy.material.SetFloat("_DT", Time.time);
            _renderEnemy.material.SetTexture("_PosTex", texWalking_pos);
            _renderEnemy.material.SetTexture("_NmlTex", texWalking_norm);
            _renderEnemy.material.SetFloat("_Length", lenWalking);
        }
    }

    private GameObject FindClosestPlayer()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

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
        Transform target = player.transform;
        
        Vector3 spawn_position = transform.position + transform.TransformDirection(Vector3.up) * 1.25f + transform.TransformDirection(Vector3.forward) * 1.0f;
        Vector3 direction = (target.position - spawn_position).normalized;

        Instantiate(arrow, spawn_position, Quaternion.LookRotation(direction));
    }

    public void Attacked(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Debug.Log($"{gameObject.name} has died");
            dead = true;
            BroadcastMessage("Dead");
            return;
        }
        Debug.Log($"Current HP: {currentHP}");
    }

    public void Dead() // Archer의 조각들이 사망 신호를 받기 위해 사용함
    {
        dead = true;
    }

}
