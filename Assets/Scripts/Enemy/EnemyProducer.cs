using UnityEngine;
using System.Collections;
using Photon.Realtime;

[System.Serializable]
public class EnemyProperties
{
    public GameObject player;
    public Vector3 startPos;
    public int maxEnemies;
    public float spawnInterval;

    public bool GeneratesMobs { get; set; }
    public int RemainEnemies { get; set; }
    public float Time { get; set; }

    public void Init()
    {
        // 처음 생성 시 소환 몹 수 설정
        RemainEnemies = maxEnemies;
        GeneratesMobs = true;
    }
};

public class EnemyProducer : MonoBehaviour
{
    public EnemyProperties[] enemies;

    // Start is called before the first frame update
    void Start()
    {
        //foreach (EnemyProperties e in enemies)
            //e.remainEnemies = e.maxEnemies;
        for (int i = 0; i < enemies.Length; i++) enemies[i].Init();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].GeneratesMobs)
            {
                enemies[i].Time += Time.deltaTime;
                if (enemies[i].Time > enemies[i].spawnInterval) // 지정된 시간(기본값: 10초)마다 적 소환
                {
                    if (enemies[i].RemainEnemies > 0)
                    {
                        GameObject duplicate = Instantiate(enemies[i].player, enemies[i].startPos, new Quaternion());
                        enemies[i].RemainEnemies--;
                    }
                    enemies[i].Time = 0;
                }
            }
        }
    }

    // Index: 1, 2, 3
    public void GenerateMobs(int index)
    {
        enemies[index - 1].GeneratesMobs = true;
    }

    public void StopMobGen(int index)
    {
        enemies[index - 1].GeneratesMobs = false;
    }
    
}
