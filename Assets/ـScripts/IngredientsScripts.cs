using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientsScripts : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	public static Renderer FindIngredient(GameObject[] ingredients,int x,int y,bool canDrawGizmo = false)
    {

		TerrainGeneratorUsingPerlin terrainGen = new TerrainGeneratorUsingPerlin(); 
		print("~ingredients.Length :"+ ingredients.Length );
		//foreach (GameObject ingredient in terrainGen.ingredients){
		//foreach (GameObject ingredient in gameObject.transform){
		for (int i = 0; i < ingredients.Length; i++)
		{
			Transform ingredient = ingredients[i].transform;
//					print(ingredient.name);
			if(ingredient.position.x == x &&  ingredient.position.z == y)
			{
//						print("x:"+x+", y:"+y);
				Renderer rend = ingredient.GetComponent<Renderer>();
				
				if(canDrawGizmo)
				{
					Vector3 center = rend.bounds.center;
					float radius = rend.bounds.extents.magnitude;
					Gizmos.color = Color.magenta;
					Gizmos.DrawWireSphere(center, radius);
				}
				return rend;
			}
		}

		return null;
    }

    internal static Vector3 FindPoint(Renderer rend, int angle)
    {
		Vector3 center = rend.bounds.center;
		float radius = rend.bounds.extents.magnitude;
        return FindPoint(center, radius, angle);
    }
	public static Vector3 FindPoint(Vector3 c, float r, int i) {
    	return c + Quaternion.AngleAxis(45.0f * i, Vector3.up) * (Vector3.right * r);
 	}

	public static Renderer FindIngredients(bool canDrawGizmo = true)
    {
		Transform me = FindObjectOfType<Transform>();
		TerrainGeneratorUsingPerlin terrainGen = new TerrainGeneratorUsingPerlin(); 

		for(int y=0; y<terrainGen.height; y++)
		{
			for(int x=0; x<terrainGen.width; x++)
			{
				//foreach (GameObject ingredient in terrainGen.ingredients){
				//foreach (GameObject ingredient in gameObject.transform){
				for (int i = 0; i < me.childCount; i++)
				{
					Transform ingredient = me.GetChild(i);
//					print(ingredient.name);
					if(ingredient.position.x == x &&  ingredient.position.z == y)
					{
//						print("x:"+x+", y:"+y);
						Renderer rend = ingredient.GetComponent<Renderer>();
						Vector3 center = rend.bounds.center;
						float radius = rend.bounds.extents.magnitude;
						if(canDrawGizmo)
						{
							Gizmos.color = Color.magenta;
							Gizmos.DrawWireSphere(center, radius);
						}
						return rend;
					}
				}
			}
		}
		return null;
    }

    void OnDrawGizmosSelected() {
        FindIngredients();
    }
}
