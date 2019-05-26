# VRTE (Virtual Reality Terrain Editor)

THIS APPLICATION REQUIRES A HTC VIVE!

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
TLDR;
Controls (Right hand mode):

LTrigger - Lock Brush
RTrigger - Apply Brush / Interact with menus

LGrip  - Transformation mode
RGrip - Transformation mode

LMenu - Open Menu of Menus
RMenu - Set Anchor

LTrackPad - Swipe to apply Transformations (Press for Teleport)
RTrackPad - Swipe to apply Transformations (Press for Teleport)
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Full Controls + Explanations:

Point by aiming your dominant hand (default is right hand) in a direction. 
 - A line will be projected from the controller to the target when you are pointing at something
 - If pointing at terrain, a red circle will indicate the approximate area of influence
 - If pointing at a menu, the line will not continue past the menu.

While pointing at terrain, with your dominant hand press 'Trigger' (button on the back) to apply the brush.
 - With your non-dominant hand, you can press trigger to lock the position of the brush, limiting the influence to that spot until released.

While pointing at a menu, with your dominant hand press 'Trigger' to interact. 
 - While pointing at a button, interacting will press the button and perform the corresponding action. 
 - While pointing at the menu (not on a button), the menu will follow your movement until released, allowing for moving of menus.

To open the 'menu' menu, with your non-dominant hand press and hold 'Menu' (small circle button above trackpad), making it appear above the controller.
 - With your dominant hand, you can interact with the menu to open three additional menus that will persist
 - The 'menu' menu cannot be moved by interacting with it.

Use the menus to:
 - Change the brush, brush size and whether it raises or lowers terrain.
 - Change the Transformation mode to translate, rotate or scale, and set the anchor position.
 - Export the mesh, choose your dominant hand, and reset/discard the terrain.

To navigate the terrain: 
 - Press down the track pad with either hand to teleport to a nearby position.
 - Hold down the grip button (long side button) with your dominant hand and swipe along the track pad to apply transformations to the mesh.
    - This can be used to teleport farther distances with scale, move quickly with translate, and negate the need to turn around (avoiding cable issues) with rotate.
    - Scale can also be used to gain better perspective on the environment and a better position to point the brush (for example when making taller areas).

To quickly place the rotation anchor, press the 'Menu' button on your dominant hand.

When exporting the mesh, the file will be saved in your Documents directory, in the folder "VRTerrainMeshes".
 - Meshes are named TerrainMesh, followed by the lowest available number (TerrainMesh1, TerrainMesh2, TerrainMesh3, etc.).


Unity Source files function on version 2019.1.0a13 Personal edition. Other Versions may function but are untested.
