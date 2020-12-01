using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShootParticle : MonoBehaviour
{
    public float destroyTime = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateParticlesLife();
    }

    public void UpdateParticlesLife()
    {
        destroyTime -= Time.deltaTime;
        if (destroyTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
