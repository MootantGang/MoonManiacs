using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Shoot : NetworkBehaviour
{
    private Vector3 targetPoint;
    public float shootDistance = 50f;
    public float shootSpeed = 50f;
    private RaycastHit rayHit;
    private Ray ray;
    public GameObject canon;
    private GameObject laserShoot;
    public GameObject bulletPrefab;
    public AudioClip[] shootSounds;
    public Camera cam;

    public void GetShootLaserInfo(out Vector3 shootDirection, out Vector3 location) {
        ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 direction = new Vector3();
        
        if (Physics.Raycast(ray.origin + cam.transform.forward*10, ray.direction, out rayHit, shootDistance)) {
            direction = (rayHit.point - canon.transform.position).normalized;
        } else {
            direction = (ray.GetPoint(1000) - canon.transform.position).normalized;
        }

        shootDirection = direction;
        location = canon.transform.position;
    }
}
