using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bullet_ctrl : MonoBehaviour
{
    public float Bullet_Speed;
    public float Lifetime;
    public float bulletDMG;
    public ParticleSystem blood_effect;
    public ParticleSystem spark_effect;
    public ParticleSystem wood_effect;
    public ParticleSystem stone_effect;
    public bool shotByEnemy;

    private void OnCollisionEnter(Collision collision)
    {
        if (!shotByEnemy && collision.gameObject.CompareTag("Flesh"))
            Instantiate(blood_effect, transform.position, transform.rotation);
        else if (collision.gameObject.CompareTag("Metal"))
            Instantiate(spark_effect, transform.position, transform.rotation);
        else if (collision.gameObject.CompareTag("Wood"))
            Instantiate(wood_effect, transform.position, transform.rotation);
        else if (collision.gameObject.CompareTag("Stone"))
            Instantiate(stone_effect, transform.position, transform.rotation);

        Debug.Log($"I Hit {collision.gameObject.name}");
        if (!shotByEnemy || collision.gameObject.CompareTag("Player"))
            collision.gameObject.SendMessage("Attacked", bulletDMG);
        Destroy(gameObject);
    }

    void Start()
    {
        Destroy(gameObject, Lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.back * Bullet_Speed * Time.deltaTime);
    }
}
