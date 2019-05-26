using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainMesh : MonoBehaviour
{
    //Class for generating the mesh and applying the brushes

    [Range(1.0f, 250.0f)]
    public int gridSizeX, gridSizeY;
    [Range(1.0f, 9.0f)]
    public int vertexDensity;
    public GameObject teleportColliderPrefab;
    public GameObject indicatorObject;

    public List<Texture2D> Brushes;

    private V3I[,][,] grid;
    private Mesh mesh;
    private Vector3[] vertices;
    private List<GameObject> teleportMeshes;
    private float startHeightRight;
    private float incVal;

    void Start()
    {
        //Increase gridsize by 1 to accomodate the additional vertex at the edge of the mesh
        gridSizeX += 1;
        gridSizeY += 1;
        //Initialise the lists and arrays
        teleportMeshes = new List<GameObject>();
        grid = new V3I[gridSizeX, gridSizeY][,];
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                grid[x, y] = new V3I[vertexDensity, vertexDensity];
            }
        }
        vertices = new Vector3[(gridSizeX * gridSizeY) * (vertexDensity * vertexDensity)];
        //Determine the space between each vertice
        incVal = (float)1 / vertexDensity;

        //Initialise the mesh
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "Grid";
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        //Generate the mesh (collider cannot be used due to baking used in teleportation causing large lag spikes)
        GenerateMesh();
        //Generate the colliders as smaller segments of the whole
        GenerateColliders();
    }

    void GenerateMesh()
    {
        //integer for setting value in array
        int vertNumber = 0;
        //for each vertex on the X axis
        for (int x = 0; x < gridSizeX; x++)
        { 
            for (int z = 0; z < vertexDensity; z++)
            {
                //for each vertex on the Y axis (though actually represents Z axis in world space)
                for (int y = 0; y < gridSizeY; y++)
                {
                    for (int w = 0; w < vertexDensity; w++)
                    {
                        //create a new V3I container to store position
                        grid[x, y][z, w] = new V3I(x + incVal * z, 0, y + incVal * w);
                        //add Vector3 data to vertices array using += override
                        vertices[vertNumber] += grid[x, y][z, w];
                        vertNumber++;
                        //if its the last vertex on the Y axis, ensure no extra vertices are added
                        if (y == gridSizeY - 1)
                        {
                            w = vertexDensity;
                        }
                    }
                }
                //if its the last vertex on the X axis, ensure no extra vertices are added
                if (x == gridSizeX - 1)
                {
                    z = vertexDensity;
                }
            }
        }

        //set the mesh's vertices
        mesh.vertices = vertices;

        //Create array for triangles
        int[] triangles = new int[((gridSizeX - 1) * vertexDensity) * ((gridSizeY - 1) * vertexDensity) * 6];
        //Set the triangles based on the vertex data generated previously
        for (int ti = 0, vi = 0, x = 0; x < (gridSizeX - 1) * vertexDensity; x++, vi++)
        {
            for (int y = 0; y < (gridSizeY - 1) * vertexDensity; y++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + ((gridSizeY - 1) * vertexDensity) + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + 1;
                triangles[ti + 5] = vi + (gridSizeY - 1) * vertexDensity + 2;

            }
        }
        //Set the mesh's triangles and recalculate the normals so it looks how it should
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
    void RepopulateVerts(List<V3I> updatedVerts)
    {
        //Since the values in the grid array have been updated, re-apply the values to vertices in the same way they were generated 
        int vertNumber = 0;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < vertexDensity; z++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    for (int w = 0; w < vertexDensity; w++)
                    {
                        vertices[vertNumber] += grid[x, y][z, w];
                        vertNumber++;
                        if (y == gridSizeY - 1)
                        {
                            w = vertexDensity;
                        }
                    }
                }
                if (x == gridSizeX - 1)
                {
                    z = vertexDensity;
                }
            }
        }

        //Set the mesh properties and recalculate
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        //Terrain now visualy updated, but colliders are not



        //Find how many collider 'chunks' there are on each axis
        int xCols = Mathf.CeilToInt((float)(gridSizeX - 1) / 10);
        int yCols = Mathf.CeilToInt((float)(gridSizeY - 1) / 10);

        //Create a list of meshes to be updated (Named mes as mesh is used elsewhere)
        List<int> mes = new List<int>();
        //For each vertex that was modified...
        int countVal = updatedVerts.Count;
        for(int i = 0; i < countVal; i++)
        {
            //If it's not already in the list, add it
            if (!mes.Contains(updatedVerts[i].mesh))
                mes.Add(updatedVerts[i].mesh);
            //If a vertex's Z value is a multiple of 10, but not it's X...
            if (updatedVerts[i].z % 10 == 0 && updatedVerts[i].x % 10 != 0)
            {
                //If it's not the edge of the terrain...
                if (updatedVerts[i].z != 0 && updatedVerts[i].z != gridSizeY-1)
                {
                    //Get the ID of the neighbouring mesh that shares this vertex
                    int otherMeshID;
                    otherMeshID = (int)updatedVerts[i].z / 10 - 1 + (Mathf.FloorToInt(updatedVerts[i].mesh / yCols) * yCols);
                    //If it's not already in the list, add it
                    if (!mes.Contains(otherMeshID))
                    {
                        mes.Add(otherMeshID);
                    }
                    //Create vertex to signal update for it on the neighbouring mesh
                    V3I newVert;
                    int newVecRef;
                    //If its not the edge mesh, work out the vertices array value for this vertex in the neighbouring mesh
                    if (updatedVerts[i].mesh+1 % yCols != 0)
                        newVecRef = updatedVerts[i].vertRef + (10*vertexDensity);
                    //else, do the same but account for the potential for fewer vertices, depending on the grid size (for example if it was a 96x96 grid, this would matter
                    else newVecRef = ((((updatedVerts[i].vertRef / vertexDensity) / ((gridSizeY -1) - ((yCols - 1) * 10)) + 1) * vertexDensity) * 10) - 1;
                    //Initialise the new vertex with the neighbouring mesh data
                    newVert = new V3I(updatedVerts[i].x, updatedVerts[i].y, updatedVerts[i].z, otherMeshID, newVecRef, teleportMeshes[otherMeshID].GetComponent<MeshCollider>().sharedMesh);
                    //Add the new vertex to the list of vertices to be processed
                    updatedVerts.Add(newVert);
                }
            }
            //If a vertex's X value is a multiple of 10, but not it's Z...
            if (updatedVerts[i].x % 10 == 0 && updatedVerts[i].z % 10 != 0)
            {
                //If it's not the edge of the terrain...
                if (updatedVerts[i].x != 0 && updatedVerts[i].x != gridSizeX-1)
                {
                    //Get the ID of the neighbouring mesh that shares this vertex
                    int otherMeshID;
                    otherMeshID = (int)(updatedVerts[i].x / 10 - 1) * yCols + (updatedVerts[i].mesh % yCols);

                    //If it's not already in the list, add it
                    if (!mes.Contains(otherMeshID))
                    {
                        mes.Add(otherMeshID);
                    }
                    //Create vertex to signal update for it on the neighbouring mesh
                    V3I newVert;
                    int newVecRef = 0;
                    //If its not the edge mesh... 
                    if (updatedVerts[i].mesh < (xCols * yCols) - yCols)
                    {
                        //if it's not the edge of the Z axis, work out the vertices array value for this vertex in the neighbouring mesh
                        if (updatedVerts[i].mesh + 1 % yCols != 0)
                        {
                            newVecRef = (vertexDensity * 10 + 1) * (vertexDensity * 10) + updatedVerts[i].vertRef;
                        }
                        //else, do the same but account for the potential for fewer vertices
                        else newVecRef = ((((gridSizeY-1) - ((yCols - 1) * 10)) * vertexDensity) * ((10 * vertexDensity) + 1)) + updatedVerts[i].vertRef;
                    }
                    //Else do the same as above but accounting for potential fewer vertices on both Z and X axis
                    else
                    {
                        if (updatedVerts[i].mesh + 1 % yCols != 0)
                        {
                            newVecRef = (((vertexDensity * 10) + 1) * (vertexDensity * ((gridSizeX -1) - ((xCols - 1) * 10)))) + updatedVerts[i].vertRef;
                        }
                        else newVecRef = ((((gridSizeY-1) - ((yCols - 1) * 10)) * vertexDensity) * ((10 * vertexDensity) + 1)) + updatedVerts[i].vertRef;
                    }
                    //Initialise the new vertex with the neighbouring mesh data
                    newVert = new V3I(updatedVerts[i].x, updatedVerts[i].y, updatedVerts[i].z, otherMeshID, newVecRef, teleportMeshes[otherMeshID].GetComponent<MeshCollider>().sharedMesh);
                    //Add the new vertex to the list of vertices to be processed
                    updatedVerts.Add(newVert);
                }
            }
            //If a vertex's X and Z value are a multiple of 10...
            if(updatedVerts[i].x % 10 == 0 && updatedVerts[i].z % 10 == 0)
            {
                //if it's not the edge of the terrain...
                if (updatedVerts[i].x != 0 && updatedVerts[i].x != gridSizeX - 1)
                {
                    //Get the ID of the southwestern mesh that shares this vertex
                    int otherMeshID;
                    otherMeshID = updatedVerts[i].mesh - yCols - 1;
                    //If it's not already in the list, add it
                    if (!mes.Contains(otherMeshID)) mes.Add(otherMeshID);
                    //Work out the vertices array value for this vertex
                    int newVecRef = ((10 * vertexDensity) + 1) * ((10 * vertexDensity) + 1)-1;
                    //Create and initialise the new vertex with the southwestern mesh data
                    V3I newVert = new V3I(updatedVerts[i].x, updatedVerts[i].y, updatedVerts[i].z, otherMeshID, newVecRef, teleportMeshes[otherMeshID].GetComponent<MeshCollider>().sharedMesh);
                    //Add the new vertex to the list of vertices to be processed
                    updatedVerts.Add(newVert);
                }
            }
        }
        int cVal = 0;
        //For each mesh that had vertices modified...
        foreach(int u in mes)
        {
            //Create new array of vertices
            Vector3[] ver = teleportMeshes[u].GetComponent<MeshCollider>().sharedMesh.vertices;
            //For each vertex that was changed...
            for(int o = 0; o < updatedVerts.Count; o++)
            {
                //If it is from this mesh, update the value.
                if (updatedVerts[o].mesh == mes[cVal])
                {
                    ver[updatedVerts[o].vertRef] += updatedVerts[o];
                }
            }
            //Set the vertices, mesh and then recalculate.
            teleportMeshes[u].GetComponent<MeshCollider>().sharedMesh.vertices = ver;
            teleportMeshes[u].GetComponent<MeshCollider>().sharedMesh = teleportMeshes[u].GetComponent<MeshCollider>().sharedMesh;
            teleportMeshes[u].GetComponent<MeshCollider>().sharedMesh.RecalculateNormals();
            teleportMeshes[u].GetComponent<MeshCollider>().sharedMesh.RecalculateBounds();
            //Next vertex
            cVal++;
        }
    }

    void GenerateColliders()
    {
        //Get the number of colliders that need to be made
        int xCols = Mathf.CeilToInt((float)(gridSizeX - 1) / 10);
        int yCols = Mathf.CeilToInt((float)(gridSizeY - 1) / 10);
        
        //Create integers to store information to keep track of where the loop is to modify behaviour at edges
        int currentX = 0;
        int xCycles = 0;
        int yCycles = 0;
        //For each collider on the X axis...
        for (int x = 0; x < xCols; x++)
        {
            int currentY = 0;
            //For each collider on the Y axis
            for (int y = 0; y < yCols; y++)
            {
                //Create a collider based on the prefab
                GameObject col = Instantiate(teleportColliderPrefab, this.gameObject.transform);
                col.name = "Collider [" + xCycles.ToString() + ", " + yCycles.ToString() + "]";
                //Create and initialise the mesh
                Mesh mesh = new Mesh();
                //Get reference to the collider that is attached to the prefab instance
                MeshCollider meshCol = col.GetComponent<MeshCollider>();
                //Set the collider's mesh to our new mesh
                meshCol.sharedMesh = mesh;
                int toGoX;
                int toGoY;
                //If not an outer edge collider, prepare for 11 vertices (multiplied by the vertex density)
                //11 vertices because an extra is needed to make it reach the full 10 unit distance, though this one doesn't include extra vertices
                if (gridSizeX - currentX > 10)
                {
                    toGoX = 11;
                }
                //Else prepare for fewer (the extra one is included in gridsizeX)
                else toGoX = gridSizeX - currentX;
                //If not an outer edge collider, prepare for 11 vertices (multiplied by the vertex density)
                if (gridSizeY - currentY > 10)
                {
                    toGoY = 11;
                }
                //Else prepare for fewer (the extra one is included in gridsizeX)
                else toGoY = gridSizeY - currentY;

                int total = 0;

                //Create array of vertices to store vertex data
                Vector3[] verts = new Vector3[(toGoX - 1) * (toGoY - 1) * vertexDensity * vertexDensity + ((toGoX - 1) * vertexDensity) + ((toGoY - 1) * vertexDensity) + 1];
                //For each vertex...
                for (int i = currentX; i < toGoX + (x * 10); i++, currentX++)
                {
                    for (int u = 0; u < vertexDensity; u++)
                    {
                        for (int k = currentY; k < toGoY + (y * 10); k++, currentY++)
                        {
                            for (int j = 0; j < vertexDensity; j++)
                            {
                                //Set its position equal to the grid data equivalent
                                verts[total] += grid[i, k][u, j];
                                //Set the mesh data for the grid vertex for use when using brushes
                                grid[i, k][u, j].mesh = teleportMeshes.Count;
                                grid[i, k][u, j].meshRef = meshCol.sharedMesh;
                                grid[i, k][u, j].vertRef = total;

                                total++;
                                //If it's the nothernmost vertex, ensure no more are added
                                if (k == toGoY + (y * 10) - 1) j = vertexDensity;
                            }
                        }
                        currentY -= toGoY;
                        //If it's the easternmost vertex, ensure no more are added
                        if (i == toGoX + (x * 10) - 1) u = vertexDensity;
                    }
                }
                currentY += 10;
                //Add the mesh to the array
                teleportMeshes.Add(col);
                //Set the mesh vertices
                mesh.vertices = verts;


                //Create array for triangles
                int[] triangles = new int[(toGoX - 1) * vertexDensity * (toGoY - 1) * vertexDensity * 6];
                //Set the triangles to the vertices created previously
                for (int ti = 0, vi = 0, g = 0; g < (toGoX - 1) * vertexDensity; g++, vi++)
                {
                    for (int h = 0; h < (toGoY - 1) * vertexDensity; h++, ti += 6, vi++)
                    {
                        triangles[ti] = vi;
                        triangles[ti + 3] = triangles[ti + 2] = vi + ((toGoY - 1) * vertexDensity) + 1;
                        triangles[ti + 4] = triangles[ti + 1] = vi + 1;
                        triangles[ti + 5] = vi + (toGoY - 1) * vertexDensity + 2;

                    }
                }
                //Set the mesh properties and recalculate
                mesh.triangles = triangles;
                mesh.RecalculateNormals();
                meshCol.sharedMesh = mesh;

                //Reset the X value for the next mesh
                currentX -= toGoX;
                yCycles++;
            }
            //Set values for the next mesh to continue correctly
            yCycles = 0;
            xCycles++;
            currentY = 0;
            currentX += 10;
        }
    }

    public void BrushTerrain(float brushSize, Vector3 trans)
    {
        //Create floats to store the integers that will be used to reference the grid mesh
        float x1, z1, xx1, zz1, x2, z2, xx2, zz2;
        //Get westmost vertex array under the influence of the brush
        x1 = Mathf.FloorToInt(trans.x - brushSize);
        //Get eastmost vertex array under the influence of the brush
        x2 = Mathf.FloorToInt(trans.x + brushSize);
        //Get the westmost vertex in the x1 array that is under the influence of the brush
        xx1 = trans.x - (brushSize - Mathf.FloorToInt(brushSize)) - (Mathf.FloorToInt(trans.x - (brushSize - Mathf.FloorToInt(brushSize))));
        //Get the eastmost vertex in the x1 array that is under the influence of the brush
        xx2 = trans.x + (brushSize - Mathf.FloorToInt(brushSize)) - (Mathf.FloorToInt(trans.x + (brushSize - Mathf.FloorToInt(brushSize))));
        //Multiply it by the vertex density to get the corresponding array integer and floor it to ensure it is an integer
        xx1 = Mathf.FloorToInt(xx1 * vertexDensity);
        xx2 = Mathf.FloorToInt(xx2 * vertexDensity);


        //Do the same for the north and southmost vertices
        z1 = Mathf.FloorToInt(trans.z - brushSize);
        z2 = Mathf.FloorToInt(trans.z + brushSize);
        zz1 = trans.z - (brushSize - Mathf.FloorToInt(brushSize)) - (Mathf.FloorToInt(trans.z - (brushSize - Mathf.FloorToInt(brushSize))));
        zz2 = trans.z + (brushSize - Mathf.FloorToInt(brushSize)) - (Mathf.FloorToInt(trans.z + (brushSize - Mathf.FloorToInt(brushSize))));
        zz1 = Mathf.FloorToInt(zz1 * vertexDensity);
        zz2 = Mathf.FloorToInt(zz2 * vertexDensity);

        //Create integers that adjust the starting point on the brush texture based on how much is over an edge
        int minFixX = 0, minFixZ = 0;
        if (x1 < 0) { minFixX = 0 - (int)x1; x1 = 0; xx1 = 0; }
        if (x2 > gridSizeX) x2 = gridSizeX - 1;
        if (z1 < 0){ minFixZ = 0 - (int)z1; z1 = 0; zz1 = 0; }
        if (z2 > gridSizeY) z2 = gridSizeY-1;

        //Store the zz1 variable for later
        int zz1Container = (int)zz1;

        //Store the position of the first vertex to cancel out later
        float startI = (int)x1, startP = (int)z1, startII = (int)xx1, startPP = (int)zz1;

        //Get the value of each unit unit in terms of pixels
        float pixelScale;
        pixelScale = Brushes[UserSettings.brushSelected].width / (brushSize * 2);
        
        //Set the direction to either raise or lower the terrain
        int direction;
        if (UserSettings.brushMode == UserSettings.BrushMode.Lower) direction = -1;
        else direction = 1;

        //Create a list to store all vertices that are affected
        List<V3I> changedVerts = new List<V3I>();
        //For each vertex...
        for (int i = (int)x1; i <= x2; i++)
        {
            for (int p = (int)z1; p <= z2; p++)
            {
                //If it's the last column of vertices, account for potentially reduced vertices
                int xVerts = vertexDensity;
                if (i == x2) xVerts = (int)xx2;
                for (int ii = (int)xx1; ii < xVerts; ii++)
                {
                    //If it's the last row of vertices, account for potentially reduced vertices
                    int zVerts = vertexDensity;
                    if (p == z2) zVerts = (int)zz2;
                    for (int pp = (int)zz1; pp < zVerts; pp++)
                    {
                        //If it exists (shouldn't be needed, but just in case.)
                        if (grid[i, p][ii, pp] != null)
                        {
                            //Find the corresponding pixel position on the brush texture
                            int x, y;
                            x = Mathf.FloorToInt((((i - startI) * pixelScale) + ((ii - startII) * (pixelScale / vertexDensity))) + (minFixX * pixelScale));
                            y = Mathf.FloorToInt((((p - startP) * pixelScale) + ((pp - startPP) * (pixelScale / vertexDensity))) + (minFixZ * pixelScale));
                            //Create a float to store the greyscale colour of the pixel (white is 1, black is 0) to act as height
                            float heightVal = Brushes[UserSettings.brushSelected].GetPixel(x, y).grayscale;
                            //Add the height to the corresponding vertex in grid
                            grid[i, p][ii, pp].y += heightVal * direction;
                            //Add the vertex to the changed verts list
                            changedVerts.Add(grid[i, p][ii, pp]);
                        }
                    }
                }
                //Set zz1 to zero so no vertices are skipped
                zz1 = 0;
            }
            //Reset zz1 for the next line
            zz1 = zz1Container;
            xx1 = 0;
        }
        //Update the vertices on the mesh
        RepopulateVerts(changedVerts);
    }
    
    //Update the indicator circle's position
    public void UpdateIndicator(Vector3 pos)
    {
        indicatorObject.transform.position = pos;
        indicatorObject.transform.localScale = new Vector3(UserSettings.brushSize*2, 0.00025f, UserSettings.brushSize*2);
    }
}

//container to hold vertex position and mesh information
public class V3I {

    public float x = -1, y = -1, z = -1;
    public int mesh = -1, vertRef = -1;
    public Mesh meshRef = null;

    //Constructor to only require position, allowing for mesh information to be added later
    public V3I(float xx, float yy, float zz)
    {
        x = xx;
        y = yy;
        z = zz;
    }

    //Constructor to set all values initially
    public V3I(float xx, float yy, float zz, int m, int vR, Mesh mR)
    {
        x = xx;
        y = yy;
        z = zz;
        mesh = m;
        vertRef = vR;
        meshRef = mR;
    }

    //Override for + to enable += to be used to set a Vector3 to a V3I's values easily
    public static Vector3 operator +(Vector3 a, V3I b)
    {
        return new Vector3(b.x,b.y,b.z);
    }
}
