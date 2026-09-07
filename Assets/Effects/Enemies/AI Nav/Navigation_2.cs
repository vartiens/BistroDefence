using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation_2 : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    public Transform player2;
    private int len_swiping = 0; // 7 ms per one clip for 70fps
    private bool swipe = false;

    //private int len_cool = 100;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame 
    void Update()
    {
        float distance = Vector3.Distance(agent.transform.position, player.position);
        float distance2 = Vector3.Distance(agent.transform.position, player2.position);

        if (Input.GetKey(KeyCode.Space))
        {
            agent.isStopped = true;
            agent.destination = player.position;
        }

        else if (distance < 3.1 || distance2 < 3.1 || swipe)
        {
            if (!swipe)
            {
                len_swiping = 100;
                swipe = true;
            }
            else
            {
                len_swiping--;
                agent.isStopped = true;
                if (len_swiping <= 0)
                {
                    swipe = false;
                    //   cool = true;
                    //  len_cool = 250;
                    agent.isStopped = false;

                }
                //else
                //{
                //    agent.isStopped = true;
                //}
            }

            if (distance < distance2)
            {
                agent.destination = player.position;
            }
            else
            {
                agent.destination = player2.position;
            }
        }

        else
        {
            //len_swiping = 80;
            agent.isStopped = false;
            if (distance < distance2)
            {
                agent.destination = player.position;
            }
            else
            {
                agent.destination = player2.position;
            }

            //if (cool)
            //{
            //    len_cool--;
            //    if(len_cool <= 0)
            //    {
            //        cool = false;   
            //    }
            //}
        }
    }
}
