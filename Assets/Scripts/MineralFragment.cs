using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineralFragment : MonoBehaviour
{
    public float cooldownTime = 2.0f;
    public float destroyTime = 30.0f;
    public float currentcooldownTime = 0.0f;
    public bool coolingdown = false;
    public bool grounded = false;
    public enum collectorSource
    {
        PLAYER, BOX
    }
    public enum instanceSource {
        VEIN, BOX
    }

    public instanceSource mineralInstance;
    public AudioClip[] mooSounds;
    public AudioClip[] cowbellSounds;

    void Update()
    {
        UpdateCooldown();
        if (GameManager.instance.IsServer())
        {
            GameManager.instance.CmdUpdateMineralLifeTime(gameObject);
            //UpdateLifeTime();
        }
    }

    public bool Activated(collectorSource source)
    {
        bool response = false;
        switch (source)
        {
            case collectorSource.PLAYER:
                if (grounded) {
                    response = true;
                }
                break;
            case collectorSource.BOX:
                if (!coolingdown && grounded)
                {
                    response = true;
                }
                break;
            default:
                break;
        }
        return response;
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

    private void OnCollisionEnter(Collision other) {
        if(other.collider.CompareTag("Ground")) {
            grounded = true;
            coolingdown = true;

        }
    }

    public void PlayRandomSound(instanceSource source) {
        int audioToPlay = 0;
        switch (source)
        {
            case instanceSource.VEIN:
                audioToPlay = Random.Range(0, mooSounds.Length - 1);
                AudioSource.PlayClipAtPoint(mooSounds[audioToPlay], transform.position, 1);
                break;  
            case instanceSource.BOX:
                audioToPlay = Random.Range(0, cowbellSounds.Length - 1);
                AudioSource.PlayClipAtPoint(cowbellSounds[audioToPlay], transform.position, 1);
                break;
            default:
                break;
        }
    }
}
