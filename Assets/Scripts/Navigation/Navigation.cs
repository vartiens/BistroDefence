using Photon.Pun.Demo.Asteroids;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Navigation : MonoBehaviour
{
    public float arriveDist;
    public UnityEvent damage;
    public float animationOffset;
    public Transform player;
    public int maxHP;
    public bool shoot;
    public GameObject Bullet;
    public ParticleSystem Shoot_effect;

    private bool active = true;
    private NavMeshAgent agent;
    private bool arrived;
    private Animator animator;
    private int currentHP;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            agent.destination = player.position;
            /*if (idx < points && moveV[idx] != null)
            {
                agent.destination = type == 4 ? playa.position : moveV[idx];
                
                if (Vector3.Distance(agent.destination, transform.position) < arriveDist)
                    idx++;
            }
            else
                idx--;*/
            Attack(player);
        }
    }

    void Attack(Transform target)
    {
        if (target == null)
            return;

        if (Vector3.Distance(target.position, transform.position) < arriveDist)
        {
            if (!arrived) // 원래 5초마다 공격 가능했었음. 이젠 그냥 공격모션이다.
            {
                //agent.isStopped = true; // 공격 모션 나올때는 잠시 정지
                arrived = true;
                if (!shoot) animator.Play("Attack", -1, animationOffset); // Pilot: 0.202f, Boss: 0f
                else animator.Play("Shoot");
            }
        }
        else
        {
            // 조준 애니메이션이 끝나면서 arrived=false로 설정된다.
            // 허나 플레이어가 아직도 근처에 있으므로 arrived가 다시 true로 설정된다.
        }
            
    }

    // 이하 3개는 공격 동작의 Animation Events에서 사용된다.

    public void StartAttack()
    {
        Debug.Log($"{name} started attack");
        agent.isStopped = true;
    }

    public void EndAttack()
    {
        agent.isStopped = false;
        agent.destination = player.position;
        arrived = false;
        Debug.Log($"{name} finished attack");
    }

    public void DoDamage()
    {
        Debug.Log($"{name} doing damage");
        // 범위 내에 있는, Tag가 Player인 모든 오브젝트에 대해 대미지를 준다.
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (GameObject target in targets)
        {
            transform.LookAt(target.transform, Vector3.up);
            if (Vector3.Distance(target.transform.position, transform.position) < arriveDist)
                damage.Invoke();
        }
        
    }

    public GameObject FindClosestPlayer()
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

    public void Shoot()
    {
        GameObject target = FindClosestPlayer();
        
        Vector3 spawn_position = transform.position + transform.TransformDirection(Vector3.up) * 1.7f + transform.TransformDirection(Vector3.forward) * 1.1f;
        Vector3 direction = (spawn_position - target.transform.position).normalized;

        Instantiate(Bullet, spawn_position, Quaternion.LookRotation(direction));
        Instantiate(Shoot_effect, spawn_position, Quaternion.LookRotation(direction));
    }

    public void Attacked(int damage)
    {
        if (currentHP > damage)
        {
            currentHP -= damage;
            Debug.Log($"{gameObject.name}'s Current HP: {currentHP}");
        }
        else if (currentHP >= 0)
        {
            currentHP = -1;
            Debug.Log($"{gameObject.name} Died");
            Destroy(gameObject);
        }
    }

    public void Activate(bool act)
    {
        active = act;
    }
}