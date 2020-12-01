using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectHandler {
    void OnEffectEnter(IEffectSource source);
    void OnEffectExit(IEffectSource source);
    void OnEffectStay(IEffectSource source);
}
