using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShoot : MonoBehaviour
{
    public GameObject hitParticles;
    private GameObject particlesReference;
    public AudioClip[] shootSounds;

    void Start()
    {
        PlayRandomSound();
    }

    private void OnCollisionEnter(Collision other) {
        if (other.collider.CompareTag("InventoryBox")) {
            GameManager.instance.CmdDropBoxMineral(other.gameObject);
        }
        particlesReference = Instantiate(hitParticles, transform.position, Quaternion.identity);
        GameManager.instance.CmdShootParticles(particlesReference);
        GameManager.instance.CmdDestroyLaserShoot(gameObject);
    }
    
    public void PlayRandomSound() {
        int audioToPlay = Random.Range(0, shootSounds.Length - 1);
        AudioSource.PlayClipAtPoint(shootSounds[audioToPlay], transform.position, 1.0f);
    }
}
