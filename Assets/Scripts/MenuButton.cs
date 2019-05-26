using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    //Script to be attached to buttons that contains the behaviour when they are clicked

    public MenuButtonTypes.ButtonType buttonType;

    //On button click...
    public void ClickButton(GameObject button)
    {
        //Find the type of button and...
        switch (buttonType)
        {
            ///////////////////////////////////////////////
            //                 Brush Menu                //
            ///////////////////////////////////////////////
            case MenuButtonTypes.ButtonType.BrushMenu:
                //If menu is not open, open it
                if (!UserSettings.brushMenu.activeInHierarchy)
                {
                    UserSettings.brushMenu.SetActive(true);
                    //Enable the green filter to show it's open
                    button.GetComponent<SpriteRenderer>().enabled = true;
                }
                //Else close it and turn off the filter
                else
                {
                    UserSettings.brushMenu.SetActive(false);
                    button.GetComponent<SpriteRenderer>().enabled = false;
                }
                break;

            case MenuButtonTypes.ButtonType.Brush1:
                //Enable the green filter to show it's selected
                button.GetComponent<SpriteRenderer>().enabled = true;
                //Update the static variable for other scripts to reference
                UserSettings.brushSelected = 0;
                //Disable filter on the other buttons
                UserSettings.buttonList[2].GetComponent<SpriteRenderer>().enabled = false;
                UserSettings.buttonList[3].GetComponent<SpriteRenderer>().enabled = false;
                UserSettings.buttonList[4].GetComponent<SpriteRenderer>().enabled = false;
                UserSettings.buttonList[5].GetComponent<SpriteRenderer>().enabled = false;
                break;

            case MenuButtonTypes.ButtonType.Brush2:
                //Enable the green filter to show it's selected
                button.GetComponent<SpriteRenderer>().enabled = true;
                //Update the static variable for other scripts to reference
                UserSettings.brushSelected = 1;
                //Disable filter on the other buttons
                UserSettings.buttonList[1].GetComponent<SpriteRenderer>().enabled = false;
                UserSettings.buttonList[3].GetComponent<SpriteRenderer>().enabled = false;
                UserSettings.buttonList[4].GetComponent<SpriteRenderer>().enabled = false;
                UserSettings.buttonList[5].GetComponent<SpriteRenderer>().enabled = false;
                break;

            case MenuButtonTypes.ButtonType.Brush3:
                //Enable the green filter to show it's selected
                button.GetComponent<SpriteRenderer>().enabled = true;
                //Update the static variable for other scripts to reference
                UserSettings.brushSelected = 2;
                //Disable filter on the other buttons
                UserSettings.buttonList[1].GetComponent<SpriteRenderer>().enabled = false;
                UserSettings.buttonList[2].GetComponent<SpriteRenderer>().enabled = false;
                UserSettings.buttonList[4].GetComponent<SpriteRenderer>().enabled = false;
                UserSettings.buttonList[5].GetComponent<SpriteRenderer>().enabled = false;
                break;

            case MenuButtonTypes.ButtonType.Brush4:
                //Enable the green filter to show it's selected
                button.GetComponent<SpriteRenderer>().enabled = true;
                //Update the static variable for other scripts to reference
                UserSettings.brushSelected = 3;
                //Disable filter on the other buttons
                UserSettings.buttonList[1].GetComponent<SpriteRenderer>().enabled = false;
                UserSettings.buttonList[2].GetComponent<SpriteRenderer>().enabled = false;
                UserSettings.buttonList[3].GetComponent<SpriteRenderer>().enabled = false;
                UserSettings.buttonList[5].GetComponent<SpriteRenderer>().enabled = false;
                break;

            case MenuButtonTypes.ButtonType.Brush5:
                //Enable the green filter to show it's selected
                button.GetComponent<SpriteRenderer>().enabled = true;
                //Update the static variable for other scripts to reference
                UserSettings.brushSelected = 4;
                //Disable filter on the other buttons
                UserSettings.buttonList[1].GetComponent<SpriteRenderer>().enabled = false;
                UserSettings.buttonList[2].GetComponent<SpriteRenderer>().enabled = false;
                UserSettings.buttonList[3].GetComponent<SpriteRenderer>().enabled = false;
                UserSettings.buttonList[4].GetComponent<SpriteRenderer>().enabled = false;
                break;

            case MenuButtonTypes.ButtonType.BrushPlus:
                //If not too big already, increase brush size.
                if(UserSettings.brushSize < 6)
                UserSettings.brushSize+= 0.25f;
                break;

            case MenuButtonTypes.ButtonType.BrushMinus:
                //If not too small already, reduce brush size
                if (UserSettings.brushSize > 0.25f)
                UserSettings.brushSize -= 0.25f;
                break;

            case MenuButtonTypes.ButtonType.BrushUp:
                //Set the static variable mode to raise terrain
                UserSettings.brushMode = UserSettings.BrushMode.Raise;
                //Enable the green filter to show it's selected
                button.GetComponent<SpriteRenderer>().enabled = true;
                //Disable filter on other button
                UserSettings.buttonList[9].GetComponent<SpriteRenderer>().enabled = false;
                break;

            case MenuButtonTypes.ButtonType.BrushDown:
                //Set the static variable mode to lower terrain
                UserSettings.brushMode = UserSettings.BrushMode.Lower;
                //Enable the green filter to show it's selected
                button.GetComponent<SpriteRenderer>().enabled = true;
                //Disable filter on other button
                UserSettings.buttonList[8].GetComponent<SpriteRenderer>().enabled = false;
                break;


            ///////////////////////////////////////////////
            //                General Menu               //
            ///////////////////////////////////////////////
            case MenuButtonTypes.ButtonType.GeneralMenu:
                //If menu is not open, open it
                if (!UserSettings.generalMenu.activeInHierarchy)
                {
                    UserSettings.generalMenu.SetActive(true);
                    //Enable the green filter to show it's open
                    button.GetComponent<SpriteRenderer>().enabled = true;
                }
                //Else close it and disable the filter
                else
                {
                    UserSettings.generalMenu.SetActive(false);
                    button.GetComponent<SpriteRenderer>().enabled = false;
                }
                break;

            case MenuButtonTypes.ButtonType.Export:
                //Export the mesh
                UserSettings.objExporter.Export(UserSettings.mesh);
                break;

            case MenuButtonTypes.ButtonType.LeftHanded:
                //Set the dominant hand to Left
                UserSettings.dominantHand = UserSettings.Hand.Left;
                //Enable the filter
                button.GetComponent<SpriteRenderer>().enabled = true;
                //Disable the other filter
                UserSettings.buttonList[18].GetComponent<SpriteRenderer>().enabled = false;
                break;

            case MenuButtonTypes.ButtonType.RightHanded:
                //Set dominant hand to right
                UserSettings.dominantHand = UserSettings.Hand.Right;
                //Enable the filter
                button.GetComponent<SpriteRenderer>().enabled = true;
                //Disable the other filter
                UserSettings.buttonList[17].GetComponent<SpriteRenderer>().enabled = false;
                break;

            case MenuButtonTypes.ButtonType.Reset:
                //Reload the scene
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
                break;


            ///////////////////////////////////////////////
            //               Transform Menu              //
            ///////////////////////////////////////////////
            case MenuButtonTypes.ButtonType.TransformMenu:
                //If not open, open it
                if (!UserSettings.transformMenu.activeInHierarchy)
                {
                    UserSettings.transformMenu.SetActive(true);
                    //Enable the filter
                    button.GetComponent<SpriteRenderer>().enabled = true;
                }
                //Else close it and disable the filter
                else
                {
                    UserSettings.transformMenu.SetActive(false);
                    button.GetComponent<SpriteRenderer>().enabled = false;
                }
                break;

            case MenuButtonTypes.ButtonType.Translate:
                //Set the mode to translate
                UserSettings.transformMode = UserSettings.TransformMode.Translate;
                //Enable the filter
                button.GetComponent<SpriteRenderer>().enabled = true;
                //Disable the other filters
                UserSettings.buttonList[12].GetComponent<SpriteRenderer>().enabled = false;
                UserSettings.buttonList[13].GetComponent<SpriteRenderer>().enabled = false;
                break;

            case MenuButtonTypes.ButtonType.Rotate:
                //Set the mode to rotate
                UserSettings.transformMode = UserSettings.TransformMode.Rotate;
                //Enable the filter
                button.GetComponent<SpriteRenderer>().enabled = true;
                //Disable the other filters
                UserSettings.buttonList[11].GetComponent<SpriteRenderer>().enabled = false;
                UserSettings.buttonList[13].GetComponent<SpriteRenderer>().enabled = false;
                break;

            case MenuButtonTypes.ButtonType.Scale:
                //Set the mode to scale
                UserSettings.transformMode = UserSettings.TransformMode.Scale;
                //Enable the filter
                button.GetComponent<SpriteRenderer>().enabled = true;
                //Disable the other filters
                UserSettings.buttonList[11].GetComponent<SpriteRenderer>().enabled = false;
                UserSettings.buttonList[12].GetComponent<SpriteRenderer>().enabled = false;
                break;

            case MenuButtonTypes.ButtonType.Anchor:
                //Get the position of the player, setting the Y to 0
                Vector3 posNoY = Valve.VR.InteractionSystem.Player.instance.transform.position;
                posNoY.y = 0;
                //Set the LineRenderer (anchorBeam) positions
                UserSettings.anchorBeam.SetPosition(0, posNoY);
                UserSettings.anchorBeam.SetPosition(1, posNoY + new Vector3(0, 100, 0));
                break;
        }
    }
}
