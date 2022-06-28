using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDestroy : MonoBehaviour
{
    private SpawnDropScript spawnDropScript;
    private float time;
    private float target;
    private bool once = true;

    public void Start()
    {
        spawnDropScript = GetComponent<SpawnDropScript>();
        target = UnityEngine.Random.Range(5f, 20f);
    }

    public void Update()
    {
        time += Time.deltaTime;

        if (time > target && once)
        {
            once = false;
            spawnDropScript?.Drop();
            Destroy(gameObject);
        }
    }

}
