using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class UserInput : MonoBehaviour
{
    //Script to handle all user interactions and user-focused data

    public float brushSize;
    public float rotationMultiplier;
    public float translateMultiplier;
    public GameObject meshObject, playerObject, menuMenu, brushMenu, transformMenu, generalMenu;
    public LineRenderer pointer;
    public LineRenderer anchorBeam;
    public TeleportArc arc;
    public TextMesh sizeText;

    private float lineStartWidth;
    private float scaleMultiplier = 1;
    private bool reset = true;
    private bool locked = false;
    private Vector3 lockPoint;
    private Vector3 meshOffset;
    private TerrainMesh terrainMesh;
    private Valve.VR.SteamVR_Input_Sources dominant, nonDominant;

    void Awake()
    {
        lineStartWidth = pointer.startWidth;
        terrainMesh = meshObject.GetComponent<TerrainMesh>();
        //Set initial values for static variables
        UserSettings.anchorBeam = anchorBeam;
        UserSettings.anchorBeam.SetPosition(0, new Vector3((float)(terrainMesh.gridSizeX - 1) / 2, 0, (float)(terrainMesh.gridSizeY - 1) / 2));
        UserSettings.anchorBeam.SetPosition(1, UserSettings.anchorBeam.GetPosition(0) + new Vector3(0, 100, 0));
        UserSettings.brushSelected = 0;
        UserSettings.brushSize = brushSize;
        UserSettings.transformMode = UserSettings.TransformMode.Scale;
        UserSettings.dominantHand = UserSettings.Hand.Right;
        UserSettings.brushMenu = brushMenu;
        UserSettings.transformMenu = transformMenu;
        UserSettings.generalMenu = generalMenu;
        UserSettings.brushMode = UserSettings.BrushMode.Raise;
        UserSettings.objExporter = new OBJExporter();
        UserSettings.mesh = meshObject;
    }

    void Update()
    {
        //Update brush size text in menu to be correct
        sizeText.text = UserSettings.brushSize.ToString();
        //Ensure the pointer width is correct
        pointer.endWidth = playerObject.transform.localScale.x * lineStartWidth;
        pointer.startWidth = playerObject.transform.localScale.x * lineStartWidth;

        //Convert static hand enum to SteamVR hand reference
        switch (UserSettings.dominantHand)
        {
            case UserSettings.Hand.Right:
                dominant = Valve.VR.SteamVR_Input_Sources.RightHand;
                nonDominant = Valve.VR.SteamVR_Input_Sources.LeftHand;
                break;
            case UserSettings.Hand.Left:
                dominant = Valve.VR.SteamVR_Input_Sources.LeftHand;
                nonDominant = Valve.VR.SteamVR_Input_Sources.RightHand;
                break;
        }

        //If not teleporting...
        if (!Valve.VR.SteamVR_Input.__actions_default_in_Teleport.GetState(Valve.VR.SteamVR_Input_Sources.Any))
        {
            //Set the pointer line renderer's start position to the hand
            pointer.SetPosition(0, Player.instance.GetHand((int)dominant - 1).transform.position);
            //Raycast forward from that point, checking if it hits anything
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(new Ray(Player.instance.GetHand((int)dominant - 1).transform.position, Player.instance.GetHand((int)dominant - 1).transform.forward), out hit))
            {
                //If it hits, show the pointer and set the end to the point that was hit
                pointer.enabled = true;
                pointer.SetPosition(1, hit.point);

                //If the hit object is not untagged (which in this case will always be the terrain), hide the circle indicator
                if (hit.collider.tag != "Untagged") terrainMesh.indicatorObject.SetActive(false);
                //If the object is terrain...
                if (hit.collider.tag == "Untagged")
                {
                    //Show the circle indicator
                    terrainMesh.indicatorObject.SetActive(true);
                    //If trigger held on non dominant hand, set lockmode to true, else set it to false
                    if (Valve.VR.SteamVR_Input.__actions_default_in_Trigger.GetState(nonDominant)) locked = true;
                    else locked = false;

                    //If trigger held in dominant hand...
                    if (Valve.VR.SteamVR_Input.__actions_default_in_Trigger.GetState(dominant))
                    {
                        switch (locked)
                        {
                            //If lockmode is enabled
                            case true:
                                //Apply the brush to the lockPoint
                                terrainMesh.BrushTerrain(UserSettings.brushSize, lockPoint);
                                break;
                            //If lockmode is not enabled
                            case false:
                                //Apply the brush to the hit point
                                terrainMesh.BrushTerrain(UserSettings.brushSize, hit.point);
                                //Set lock point to the hit point
                                lockPoint = hit.point;
                                break;
                        }
                    }
                    //Update lockPoint so it can be used by the indicator
                    else lockPoint = hit.point;

                    terrainMesh.UpdateIndicator(lockPoint);
                }
                //If the object is a button...
                else if (hit.collider.tag == "Button")
                {
                    //When trigger pressed with dominant hand...
                    if (Valve.VR.SteamVR_Input.__actions_default_in_Trigger.GetStateDown(dominant))
                    {
                        //Click the button
                        hit.transform.gameObject.GetComponent<MenuButton>().ClickButton(hit.transform.gameObject);
                    }
                }
                //If the object is a menu...
                else if (hit.collider.tag == "Menu")
                {
                    //When the trigger is pressed with dominant hand, move the menu
                    if(Valve.VR.SteamVR_Input.__actions_default_in_Trigger.GetState(dominant))
                    hit.collider.gameObject.GetComponent<MenuMove>().Move(dominant);
                }
            }
            //If the raycast doesn't hit anything
            else
            {
                //Disable pointer and circle indicator
                pointer.enabled = false;
                terrainMesh.indicatorObject.SetActive(false);
            }
        }
        //If the player is teleporting
        else
        {
            //Disable pointer and circle indicator
            pointer.enabled = false;
            terrainMesh.indicatorObject.SetActive(false);
        }

        //If the player presses the grips (entering transform mode)...
        if (Valve.VR.SteamVR_Input.__actions_default_in_Grip.GetState(dominant))
        {
            //If the trackpad is being touched...
            if (Valve.VR.SteamVR_Input.__actions_default_in_TrackpadTouched.GetState(dominant))
            {
                //If this is not the first frame of contact
                if (!reset)
                {
                    //Convert movement into a 2D vector made from the current position on the trackpad minus the previous frame's position
                    Vector2 axis = Valve.VR.SteamVR_Input.__actions_default_in_Trackpad.GetAxis(dominant);
                    Vector2 prevAxis = Valve.VR.SteamVR_Input.__actions_default_in_Trackpad.GetLastAxis(dominant);
                    Vector2 a;
                    a = axis - prevAxis;

                    //Check which transform mode is currently active
                    switch (UserSettings.transformMode)
                    {
                        //If translate mode...
                        case UserSettings.TransformMode.Translate:
                            //Get the forward vector of the camera and set its Y to zero so height is not changed
                            Vector3 forwardNoY = Camera.main.transform.forward;
                            forwardNoY.y = 0;
                            //Get the right vector of the camera and set its Y to zero so height is not changed
                            Vector3 rightNoY = Camera.main.transform.right;
                            rightNoY.y = 0;
                            //Modify the player object's position by the the forward vector multiplied by the input from the trackpad, multiplied by a speed multiplier and multiplied by scale
                            playerObject.transform.position += forwardNoY * a.y * translateMultiplier * scaleMultiplier;
                            //Modify the player object's position by the the right vector multiplied by the input from the trackpad, multiplied by a speed multiplier and multiplied by scale
                            playerObject.transform.position += rightNoY * a.x * translateMultiplier * scaleMultiplier;
                            break;
                            //If rotate mode...
                        case UserSettings.TransformMode.Rotate:
                            //Rotate the player around the terrain's anchor so it looks like the terrain is rotating
                            playerObject.transform.RotateAround(UserSettings.anchorBeam.GetPosition(0), new Vector3(0, 1, 0), a.x * rotationMultiplier);
                            break;
                            //If scale mode...
                        case UserSettings.TransformMode.Scale:
                            //Update scale reference
                            scaleMultiplier += a.y;
                            //If scale is too small, reset it to 1
                            if (scaleMultiplier < 1) scaleMultiplier = 1;
                            //Convert scale multiplier to Vector3 and apply it to the player object
                            Vector3 scaleAmount = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
                            playerObject.transform.localScale = scaleAmount;
                            break;
                    }
                }
                //Set reset to false to enable for next frame, setting the GetLastAxis to the adjusted value
                reset = false;
            }
            //If the trackpad is not being touched, set reset to true so it will reset when next touched
            else reset = true;
        }
        //If menu button pressed in dominant hand...
        if (Valve.VR.SteamVR_Input.__actions_default_in_Menu.GetState(dominant))
        {
            //Set the anchor to the player's position, reducing the Y to 0
            Vector3 posNoY = playerObject.transform.position;
            posNoY.y = 0;
            UserSettings.anchorBeam.SetPosition(0, posNoY);
            UserSettings.anchorBeam.SetPosition(1, posNoY + new Vector3(0, 100, 0));
        }
        //If menu button pressed in non-dominant hand...
        if (Valve.VR.SteamVR_Input.__actions_default_in_Menu.GetStateDown(nonDominant))
        {
            //Show the menu of menus
            menuMenu.SetActive(true);
            //Ensure the parent is for the correct hand (in case dominant hand is changed)
            menuMenu.transform.SetParent(Player.instance.GetHand((int)nonDominant - 1).transform);
            //Set its position so it will be positioned appropriately
            menuMenu.transform.localPosition = new Vector3(0, 0.075f, 0.04f);
            //Set its rotation so it will be angled appropriately
            menuMenu.transform.localRotation = Quaternion.Euler(45,0,0);
        }
        //If menu button is released in non-dominant hand...
        if (Valve.VR.SteamVR_Input.__actions_default_in_Menu.GetStateUp(nonDominant))
        {
            //Hide the menu
            menuMenu.SetActive(false);
        }
    }
}
