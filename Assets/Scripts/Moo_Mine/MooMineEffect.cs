using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MooMineEffect : MonoBehaviour, IEffectSource
{
    public float mineEffectDuration = 15f;
    /*public delegate void _UnderMooMineEffect(float mineEffectDuration);
    public static event _UnderMooMineEffect UnderMooMineEffect;*/
    public GameObject owner { get; set; }
    public EffectType type {
        get { return EffectType.MOO_MINE; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public void TriggerMine() {
        if (UnderMooMineEffect != null) {
            UnderMooMineEffect(mineEffectDuration);
        }
    }*/

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            //other.GetComponent<PlayerStates>().SubscribeToMooMineEffect();
            other.GetComponent<PlayerBehaviors>().OnEffectEnter(this);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerBehaviors>().OnEffectStay(this);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            //other.GetComponent<PlayerStates>().UnsubscribeFromMooMineEffect();
            other.GetComponent<PlayerBehaviors>().OnEffectExit(this);
        }
    }

    private void OnDestroy() {
        // TODO: Descubrir como hacer el unsubscribe de los que estuviesen suscritos 
    }

    private void OnEnable() {
        Invoke("DestroyMine", mineEffectDuration);
    }

    private void DestroyMine() {
        Destroy(transform.parent.gameObject);
    }

    public GameObject GetGameObject() {
        return transform.parent.gameObject;
    }
}
