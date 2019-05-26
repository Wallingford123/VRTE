using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonTypes : MonoBehaviour
{
    //Script attached to manager to create enum of all buttons and store list of button objects for UserSettings script

    public enum ButtonType
    {
        BrushMenu,

        Brush1,
        Brush2,
        Brush3,
        Brush4,
        Brush5,
        BrushPlus,
        BrushMinus,
        BrushUp,
        BrushDown,



        GeneralMenu,

        Export,
        LeftHanded,
        RightHanded,
        Reset,



        TransformMenu,

        Translate,
        Rotate,
        Scale,
        Anchor
    }
    public List<GameObject> buttons;

    private void Start()
    {
        UserSettings.buttonList = buttons;
    }
}
