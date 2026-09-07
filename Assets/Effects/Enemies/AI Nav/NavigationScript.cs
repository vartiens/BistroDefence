using POpusCodec.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR.Interaction.Toolkit;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

public class NavigationScript : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;
    public float arriveDist;
    public float len_attack; // Attack animation length
    private bool attack = false;
    private bool dead = false; // Stop moving if dead
    private float _time = 0, _searchT = 0;
    public float searchPeriod;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = FindClosestPlayer().transform;
        agent.destination = player.position;
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

    // Issue: destination은 좌표이므로 처음 계산된 좌표에서 재계산이 이뤄지지 않는다.
    // 즉 공격 등으로 경로가 리셋되기 전까지는 플레이어의 잔상을 쫓게 된다.
    private void TargetChange()
    {
        Vector3 ss = agent.velocity;
        Transform newP = FindClosestPlayer().transform;

        // 계속 destination을 바꾸면 움직임이 끊긴다.
        // 속도를 보존하게 하면 끊기지 않는다.
        Debug.Log($"{name} is targeting {newP.name} now");
        player = newP;
        agent.destination = player.position;
        agent.isStopped = false;
        
        // 속도를 보존했다가 경로 변경후 복구
        agent.velocity = ss;
    }

    // Update is called once per frame 
    void Update()
    {
        if (dead)
        {
            agent.ResetPath();
            agent.isStopped = true;
            return;
        }
        
        float distance = Vector3.Distance(agent.transform.position, player.position);
        if (distance < arriveDist || attack)
        {
            if (name.Contains("Archer"))
            {
                float angle;
                Vector3 t = transform.position, p = player.position;
                angle = Mathf.Atan2(p.x - t.x, p.z - t.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, angle + 90.0f, 0);
            }

            if (!attack)
            {
                _time = 0;
                attack = true;
                agent.ResetPath();
            }
            else
            {
                _time += Time.deltaTime;
                agent.isStopped = true;
                if (_time > len_attack)
                {
                    attack = false;
                    _time = 0;
                    TargetChange();
                }
            }
            
        }

        else
        {
            agent.isStopped = false;
            _searchT += Time.deltaTime;
            if (_searchT > searchPeriod) // n초마다 타겟 탐색 (프레임 단위로 하면 렉걸림)
            {
                TargetChange();
                _searchT = 0.0f;
            }
        }
    }

    public void Dead()
    {
        Debug.Log("I am dead");
        agent.ResetPath();
        agent.isStopped = true;
        dead = true;
    }
}
