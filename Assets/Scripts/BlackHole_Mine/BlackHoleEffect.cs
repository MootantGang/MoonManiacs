using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleEffect : MonoBehaviour, IEffectSource
{
    public int mineEffectDuration = 10;
    /*public delegate void _UnderBlackHoleEffect(GameObject mine, int mineEffectDuration);
    public static event _UnderBlackHoleEffect UnderBlackHoleEffect;*/
    public GameObject owner { get; set; }
    public EffectType type {
        get { return EffectType.BLACK_HOLE; }
    }
    public GameObject blackHolePrefab;
    private GameObject blackHoleInst;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public void TriggerMine() {
        if (UnderBlackHoleEffect != null) {
            UnderBlackHoleEffect(gameObject, mineEffectDuration);
        }
    }*/

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            //other.GetComponent<PlayerStates>().SubscribeToBlackHoleEffect();
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
            //other.GetComponent<PlayerStates>().UnsubscribeFromBlackHoleEffect();
            other.GetComponent<PlayerBehaviors>().OnEffectExit(this);
        }
    }

    private void OnDestroy() {
        // TODO: Descubrir como hacer el unsubscribe de los que estuviesen suscritos 
    }

    private void OnEnable() {
        Invoke("DestroyMine", mineEffectDuration);
        blackHoleInst = Instantiate(blackHolePrefab, transform.position, Quaternion.identity) as GameObject;
    }

    private void DestroyMine() {
        Destroy(blackHoleInst);
        Destroy(transform.parent.gameObject);
    }

    public GameObject GetGameObject() {
        return transform.parent.gameObject;
    }
}
