using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSettings
{
    //Class for saving static variables for convenient access
    public enum Hand {
        Left,
        Right
    };
    public enum TransformMode
    {
        Translate,
        Rotate,
        Scale
    };
    public enum BrushMode
    {
        Raise,
        Lower
    }

    public static Hand dominantHand;
    public static TransformMode transformMode;
    public static BrushMode brushMode;
    public static int brushSelected;
    public static float brushSize;
    public static GameObject brushMenu;
    public static GameObject transformMenu;
    public static GameObject generalMenu;
    public static GameObject mesh;
    public static List<GameObject> buttonList;
    public static OBJExporter objExporter;
    public static LineRenderer anchorBeam;
}
