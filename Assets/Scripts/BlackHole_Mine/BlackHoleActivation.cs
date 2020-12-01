using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleActivation : MonoBehaviour
{
    public GameObject fieldEffectArea;
    public GameObject owner;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }    
    
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && other.gameObject != owner) {
            ActivateMine();
        }
    }

    private void ActivateMine() {
        fieldEffectArea.SetActive(true);
    }
}
