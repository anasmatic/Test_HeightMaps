using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseAnas : MonoBehaviour {

	private float[,] GenerateSmoothHeights(int width,int height,int elevation, int octaves, float NoiseScale, float persistance,float lacunarity, GameObject[] ingredients)
    {
        float[,] heights = new float[width,height];
		for(int y=0; y<height; y++){
			for(int x=0; x<width; x++){
				//find ingreadiant
				GameObject ingredient = findIngredient(x,y,ingredients);
				if(ingredient){
					//reach for ingreadient
					//PROBLEM: if ingredient is undder ground (like -5) the terrain is not able to reach for it 
					print("FOUND INGREDIENT @"+x+"*"+y+", "+ingredient.transform.position.y+" >>>elev="+elevation);
					heights[y,x] = (int)ingredient.transform.position.y / elevation;//getHeightPerlinNoised((int)ingredient.transform.position.y, (int)ingredient.transform.position.y);
					print(heights[y,x]);
				}
				else//smoth terrain
					heights[x,y] = getHeightPerlinNoised(x,y, octaves, NoiseScale, persistance, lacunarity);
			}
		}

		//normalization
		for(int y=0; y<height; y++){
			for(int x=0; x<width; x++){
				heights[x,y] = Mathf.InverseLerp(minNoiseH, maxNoiseH, heights[x,y]);
			}
		}

		return heights;
    }
	float lastNormalX, lastNormalY;
    float maxNoiseH = float.MinValue;
	float minNoiseH = float.MaxValue;
    private float getHeightPerlinNoised(int x, int y, int octaves, float NoiseScale, float persistance,float lacunarity)
    {

	 	float frequancy = 1;
	 	float amplitude = 1;
		float noiseHeight = 0;
		float xNormlized;
		float yNormlized;
		
		for (int i = 0; i < octaves; i++)
		{
			
			 //xNormlized = lastNormalX = (float) x/width * NoiseScale;
			// yNormlized = lastNormalY =(float) y/height * NoiseScale;
			
			xNormlized = lastNormalX = (float) x/NoiseScale * frequancy;
			yNormlized = lastNormalY =(float) y/NoiseScale * frequancy;
			
			//xNormlized = lastNormalX = (float) x/width * NoiseScale * frequancy;
			//yNormlized = lastNormalY =(float) y/height * NoiseScale * frequancy;
			
			float perlinNoise = Mathf.PerlinNoise(xNormlized,yNormlized)*2-1;
			noiseHeight += perlinNoise * amplitude;

			amplitude *= persistance;
			frequancy *= lacunarity;
		}
		if( noiseHeight > maxNoiseH)
			maxNoiseH = noiseHeight;
		else if(noiseHeight < minNoiseH)
			minNoiseH = noiseHeight;

		return noiseHeight;
    }


    private GameObject findIngredient(int x, int y, GameObject[] ingredients)
    {
        foreach (GameObject ingredient in ingredients){
			if(ingredient.transform.position.x == x &&  ingredient.transform.position.z == y)
				return ingredient;
		}
		return null;
    }
}
