using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType{MOO_MINE, BLACK_HOLE, STUN};

public interface IEffectSource {
    GameObject owner { get; set; }
    EffectType type { get;}

    GameObject GetGameObject();
}
