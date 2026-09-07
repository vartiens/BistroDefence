using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

public class Test : MonoBehaviour
{
    public UnityEvent startSpawning;
    public UnityEvent stopSpawning;
    public float waitTime;

    private float time = 0;
    private bool spawning = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > waitTime)
        {
            if (!spawning)
                startSpawning.Invoke();
            else
                stopSpawning.Invoke();
            spawning = !spawning;
            time = 0;
        }
    }
}
