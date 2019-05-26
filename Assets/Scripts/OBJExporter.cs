using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityExtension;
using UnityEngine;

public class OBJExporter
{

    //Class to simplify the exporting of the mesh, utilising the OBJ-IO library

    private string outputPath;

    private string constantPath;

    public OBJExporter()
    {
        //Get path to documents and a directory of VRTerrainMeshes
        outputPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/VRTerrainMeshes";
        //If a folder doesn't exist, create one
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        //Add a name to the path
        outputPath += "/TerrainMesh";
        //Set the constant path to Documents/VRTerrainMeshes/TerrainMesh, so it can be reverted to when adding a new number
        constantPath = outputPath;
    }

    public void Export(GameObject mesh)
    {
        //Add .OBJ to the path so it exports as an OBJ
        outputPath += ".OBJ";
        int i = 1;
        //If the file of this name exists, add i to the path and try again, increasing the value of i until there is an available name
        while (File.Exists(outputPath))
        {
            outputPath = constantPath;
            outputPath += i.ToString();
            outputPath += ".OBJ";
            i++;
        }
        //Createa and open a file stream
        FileStream lStream = new FileStream(outputPath, FileMode.Create);
        //Get the mesh data to be converted to the OBJ
        OBJData lOBJData = mesh.GetComponent<MeshFilter>().mesh.EncodeOBJ();
        //Export it using the OBJ-IO function
        OBJLoader.ExportOBJ(lOBJData, lStream);
        //Close the file stream
        lStream.Close();
    }
}
