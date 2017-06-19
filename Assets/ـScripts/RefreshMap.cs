using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGeneratorUsingPerlin))]
public class RefreshMap : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainGeneratorUsingPerlin myScript = (TerrainGeneratorUsingPerlin)target;

        if(DrawDefaultInspector())
        {
            if(myScript.autoUpdate)
                myScript.RefreshTerrain();
        }
        
        if(GUILayout.Button("Random Terrain"))
        {
            myScript.RefreshTerrain();
        }

        if(GUILayout.Button("Ingredients"))
        {
            myScript.SharpAvoidIngredients();
        }

        if(GUILayout.Button("smooth"))
        {
            myScript.SmoothAroundIngredients();
        }
    }
}
