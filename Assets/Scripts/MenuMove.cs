using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMove : MonoBehaviour
{
    //Script to enable the clicking and dragging of menus

    public float moveMultiplier, moveSpeed;
    private float distance = 2;
    private bool reset = true;

    public void Move(Valve.VR.SteamVR_Input_Sources dominant)
    {
        //Get reference to player instance
        Valve.VR.InteractionSystem.Player player = Valve.VR.InteractionSystem.Player.instance;
        //Set position to a distance ahead of the user's hand
        transform.position = player.GetHand((int)dominant - 1).transform.position + player.GetHand((int)dominant - 1).transform.forward * distance * player.transform.localScale.x;

        //If the trackpad is also being used
        if (Valve.VR.SteamVR_Input.__actions_default_in_TrackpadTouched.GetState(dominant))
        {
            //If the trackpad values don't need to be reset after taking finger off
            if (!reset)
            {
                //Get the values for the trackpad swipe movement
                Vector2 axis = Valve.VR.SteamVR_Input.__actions_default_in_Trackpad.GetAxis(dominant);
                Vector2 prevAxis = Valve.VR.SteamVR_Input.__actions_default_in_Trackpad.GetLastAxis(dominant);
                float a;
                //Get the difference between them
                a = axis.y - prevAxis.y;
                //Add the difference to the distance multiplied by the multiplier set in the editor
                distance += a * moveMultiplier;
                //If it's too close, dont move it closer
                if (distance < 0.25f * player.transform.localScale.x) distance = 0.25f * player.transform.localScale.x;
            }
            //else do nothing this frame but reset the trackpad positions so it doesn't register the inverse of the movement the user just made in GetLastAxis
            reset = false;
        }
        //Ensure it always resets
        else reset = true;
    }

    void FixedUpdate()
    {
        //Look at the player
        transform.LookAt((2 * transform.position - Valve.VR.InteractionSystem.Player.instance.transform.position) - new Vector3(0,Valve.VR.InteractionSystem.Player.instance.eyeHeight * Valve.VR.InteractionSystem.Player.instance.transform.localScale.y,0));
    }
}
