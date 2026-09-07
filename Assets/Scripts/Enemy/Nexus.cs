using UnityEngine;

public class Nexus : MonoBehaviour
{
    public int maxHP;
    private int currentHP;
    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        Debug.Log($"Maximum HP: {maxHP}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Attacked(int damage)
    {
        if (currentHP > damage)
        {
            currentHP -= damage;
            Debug.Log($"Current HP: {currentHP}");
        }
        else if (currentHP >= 0)
        {
            currentHP = -1;
            Debug.Log($"Game Over");
        }
        else
            Debug.Log($"You are already dead");
    }
}
