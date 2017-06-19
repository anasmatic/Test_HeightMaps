using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IngredientsScripts))]
public class FindIngredients : Editor
{
    public override void OnInspectorGUI()
    {
        IngredientsScripts myScript = (IngredientsScripts)target;
        /*
        if(DrawDefaultInspector())
        {
            if(myScript.autoUpdate)
                myScript.findIngredient();
        }
        */
        if(GUILayout.Button("Find Ingredient"))
        {
            //myScript.FindIngredients();
        }
    }
}
