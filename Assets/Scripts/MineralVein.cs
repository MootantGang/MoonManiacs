using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MineralVein : NetworkBehaviour
{
    public GameObject[] mineralPrefabs; // Objects to be shown randomly when dropping mineral from box
    public float cooldownTime = 2.0f;
    public float currentcooldownTime = 0.0f;
    public bool coolingdown = false;
    public int mineralsPerMining = 3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer) {
            UpdateCooldown();
        }
    }

    public List<GameObject> DropMineral()
    {
        List<GameObject> newMinerals = new List<GameObject>();
        if (!coolingdown)
        {
            for (int i = 0; i < mineralsPerMining; ++i) {
                GameObject newMineral = Instantiate(mineralPrefabs[Random.Range(0, mineralPrefabs.Length)]) as GameObject;
                //newMineral.GetComponent<MineralFragment>().mineralInstance = MineralFragment.instanceSource.VEIN;
                newMinerals.Add(newMineral);
                newMineral.transform.position = transform.position + new Vector3(0.0f, 2.0f, 0.0f);
                //GameManager.instance.RpcPlayRandomSound(newMineral, MineralFragment.instanceSource.VEIN);

                Rigidbody rb = newMineral.GetComponent<Rigidbody>();
                Vector3 trajectory = new Vector3(0.0f, 0.0f, 0.0f);
                trajectory.x = Random.Range(-1.0f, 1.0f);
                trajectory.y = Random.Range(1.0f, 5.0f);
                trajectory.z = Random.Range(-1.0f, 1.0f);
                //float force = Random.Range(5.0f, 10.0f);
                float force = 5.0f;
                
                //rb.velocity = trajectory * force;

                rb.AddForce(trajectory * force, ForceMode.Impulse);
            }
            

            // Cooldown
            coolingdown = true;

            return newMinerals;
        }
        return newMinerals;
    }

    public void UpdateCooldown()
    {
        if (coolingdown)
        {
            currentcooldownTime += Time.deltaTime;

            if (currentcooldownTime >= cooldownTime)
            {
                currentcooldownTime = 0.0f;
                coolingdown = false;
            }
        }
    }
}
