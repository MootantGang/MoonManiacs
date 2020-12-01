using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{

    // Axis Inputs

    public static float HorizontalMovement()
    {
        float h = 0.0f;
        h += Input.GetAxis("Horizontal");
        return h;
    }

    public static float VerticalMovement()
    {
        float v = 0.0f;
        v += Input.GetAxis("Vertical");
        return v;
    }

    public static Vector3 MovementDirection()
    {
        return new Vector3(HorizontalMovement(), 0.0f, VerticalMovement());
    }

    public static float HorizontalRotation()
    {
        float h = 0.0f;
        h += Input.GetAxis("camera_horizontal");
        //return h;
        return Input.GetAxis("camera_horizontal");
    }

    public static float VerticalRotation()
    {
        float v = 0.0f;
        v += Input.GetAxis("camera_vertical");
        return v;
    }

    // Buttons Inputs

    public static bool SprintButton()
    {
        return Input.GetButtonDown("sprint");
    }

    public static bool InteractionButton()
    {
        return Input.GetButtonDown("interaction_button");
    }

    public static bool MeleeWeaponButton()
    {
        return Input.GetButtonDown("melee_weapon");
    }

    public static bool TrapButton()
    {
        return Input.GetButtonDown("trap");
    }

    public static bool AimGunButton()
    {
        return Input.GetButtonDown("aim_gun");
    }

    public static bool ShootGunButton()
    {
        return Input.GetButtonDown("shoot");
    }

    public static bool Exit() {
        return Input.GetButtonDown("Cancel");
    }
}
