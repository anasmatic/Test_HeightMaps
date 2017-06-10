﻿using System;
using System.Collections;
using System.Collections.Generic;
using PixelsForGlory.ComputationalSystem;
using UnityEngine;

public class TerrainGeneratorUsingPerlin : MonoBehaviour {

	public GameObject testCube;
	public GameObject testSubCube;
	public float elevation = 50;
	public int width = 128;
	public int height = 128;

	[Range(0,50)]
	public float NoiseScale = 20f;
	[Range(1,15)]
	public int octaves = 4;
	[Range(0,5)]
	public float persistance = 0.5f;
	[Range(0,5)]
	public float lacunarity = 2.0f;
	public int seed = 21;
	public Vector2 offset;

	public AnimationCurve heightEvaluation;
	public Transform ingredientsParent;
	private Transform[] ingredients =  {};
	public bool autoUpdate;

	Terrain terrain;
	int printCount =0;
	private List<Vector3> ingredientsPoints =  new List<Vector3>();
    private List<Vector3> ingredientsCenters =  new List<Vector3>();
    private List<Vector3> ingredientsCorners;
	float[,] betaHeights;

    // Use this for initialization
    void Start () {
		
		
	}

	public void RefreshTerrain(){
		float t = Time.time;
		var lnth = ingredientsParent.transform.childCount;
		ingredients = new Transform[lnth];
		for (int i = 0; i < lnth; i++)
		{
			ingredients[i] =  ingredientsParent.GetChild(i);
		}
		//ingredients = ingredientsParent.transform.GetChild

		if(NoiseScale <= 0) NoiseScale = 0.0001f;
		terrain = GetComponent<Terrain>();
		betaHeights = new float[,]{};
		printCount =0;
		terrain.terrainData = GenerateSmoothTerrain(terrain.terrainData);
		float finalTime = Time.time-t;
		print("Time : "+finalTime.ToString());
	}
	public void SharpAvoidIngredients(){
		//save ingreadiants points
		SaveIngreadiantsPoints();
		AddIngredientPointsToHeightMap(betaHeights);
		terrain.terrainData.SetHeights(0,0,betaHeights);
	}

    private TerrainData GenerateSmoothTerrain(TerrainData terrainData)
    {
		//terrainData.heightmapResolution = width+1;
        terrainData.size = new Vector3(width,elevation,height);
				
		//float[,] betaHeights = GenerateSmoothHeights();Anas.GenerateSmoothHeights(int width,int height,int elevation, int octaves, float NoiseScale, float persistance,float lacunarity, GameObject[] ingredients)
		betaHeights = Noise.GenerateNoiseMap(width, height, seed, NoiseScale, octaves, persistance, lacunarity, offset);
		for(int y=0; y<height; y++){
			for(int x=0; x<width; x++){
				betaHeights[x,y] = heightEvaluation.Evaluate(betaHeights[x,y]);
			}
		}
		

		terrainData.SetHeights(0,0,betaHeights);
		return terrainData;
    }

    private void AddIngredientPointsToHeightMap(float[,] betaHeights)
    {
		for (int i = 0; i < ingredientsPoints.Count; i++)
		{
			betaHeights[(int)ingredientsPoints[i].z,(int)ingredientsPoints[i].x] = ingredientsPoints[i].y/elevation;
		}
    }

    private void SaveIngreadiantsPoints()
    {
		ingredientsPoints =  new List<Vector3>();
		ingredientsCenters =  new List<Vector3>();
		ingredientsCorners =  new List<Vector3>();
		float[] mainXPoints;
		float[] mainZPoints; 
        for (int i = 0; i < ingredients.Length; i++)
		{
			Transform ingredient = ingredients[i];
			Renderer ingredRend = ingredient.GetComponent<Renderer>();
			
			Vector3 center = ingredRend.bounds.center;
			ingredientsCenters.Add(center);
			Vector3 point1 = IngredientsScripts.FindPoint(ingredRend, 90)  ;
			Vector3 point2 = IngredientsScripts.FindPoint(ingredRend, -90) ;
			Vector3 point3 = IngredientsScripts.FindPoint(ingredRend, 180) ;
			Vector3 point4 = IngredientsScripts.FindPoint(ingredRend, 0);
	
			point1.x = (int) point1.x+1;point1.z = (int) point1.z;
			point2.x = (int) point2.x+1;point2.z = (int) point2.z+1;
			point3.x = (int) point3.x;  point3.z = (int) point3.z;
			point4.x = (int) point4.x+1;point4.z = (int) point4.z;
	
			ingredientsCorners.Add(point1);	ingredientsCorners.Add(point2);
			ingredientsCorners.Add(point3);	ingredientsCorners.Add(point4);

			mainZPoints = new float[]{point1.z,point3.z,point2.z,point4.z};
			mainXPoints = new float[]{point1.x,point3.x,point2.x,point4.x};
			int leastZ = (int)Mathf.Min(mainZPoints);
			int mostZ = (int)Mathf.Max(mainZPoints);
			int leastX = (int)Mathf.Min(mainXPoints);
			int mostX = (int)Mathf.Max(mainXPoints);
			
			int lenX = (mostX - leastX + 1);
			int lenZ = (mostZ - leastZ + 1);
			
			int len = lenX*lenZ;
			
			for(int z=leastZ; z<=mostZ; z++){
				for(int x=leastX; x<=mostX; x++){
					//TODO: try to exclude exteram oints ex: 3,3 & 7,3 & 3,7 & 7,7
					ingredientsPoints.Add(new Vector3(x,center.y,z));
				}
			}
		}
	}

	public void SmoothAroundIngredients(){
		float t = Time.time;
		OtherSideSmoothing(ingredientsCorners[0],ingredientsCorners[1]);//x direction
		SideSmoothing(ingredientsCorners[2],ingredientsCorners[3]);//z direction
		
		terrain.terrainData.SetHeights(0,0,betaHeights);

		float finalTime = Time.time-t;
		print("Time : "+finalTime.ToString());
	}
	void OtherSideSmoothing(Vector3 pointA, Vector3 pointZ){
		print("-from "+pointA+"to"+pointZ);
		
		Vector3 point = pointA;
		pointA.z--; pointZ.z++;
		for (int i = (int)pointA.z; i <= (int)/*pointA.x*/pointZ.z; i++)
		{
			print("i:"+i);
			point.z = i;
			//Instantiate(testCube,point,Quaternion.identity);
			SmoothLineX(point);//vertical to this point
			SmoothLineBackwardX(point);
		}
	}
	void SideSmoothing(Vector3 pointA, Vector3 pointZ){
		//print("-from "+pointA+"to"+pointZ);
		
		Vector3 point = pointA;
		pointA.x--; pointZ.x++;
		for (int i = (int)pointA.x; i <= (int)/*pointA.x*/pointZ.x; i++)
		{
			//print("i:"+i);
			point.x = i;
			//Instantiate(testCube,point,Quaternion.identity);
			SmoothLineY(point);//vertical to this point
			SmoothLineBackwardY(point);
		}
	}
	void SmoothLineX(Vector3 startPoint){
		//print("--SmoothLine:@"+startPoint);
		
		bool trigger = false;
		List<Vector3> newIngredCenters;
		if(ingredientsCenters.Count > 0 && betaHeights.Length > 0){
			newIngredCenters = new List<Vector3>();
			//Color[] colors = new Color[]{Color.black,Color.blue,Color.cyan,Color.gray,Color.green,Color.magenta,Color.red,Color.white,Color.yellow,Color.black};
			//String[] colorsS = new String[]{"black","blue","cyan","gray","green","magenta","red","white","yellow","black"};
			Vector3 ingredientsCenter0 = startPoint;//for testing :ingredientsCenters[0];
			ingredientsCenter0 = SwichZandY(ingredientsCenter0);
			Vector3 ommak1 = ingredientsCenter0;
			newIngredCenters.Add(ommak1);
			for (int i = 0; i < 10; i++)
			{
				//Gizmos.color = colors[i];

				Vector3 ommak2 = ingredientsCenter0 ;
				ommak2.x++;//next step
				
				//ommak2 = SwichZandY(ommak2);
				//print("-swiched:"+ommak2 +","+ingredientsCenter0);
				ommak2.z = betaHeights[(int)ingredientsCenter0.y+1,(int)ingredientsCenter0.x]*elevation;
				Vector3 targetDir = ommak2 - ommak1;// - ommak2;
				float angle = Vector3.Angle(targetDir, Vector3.forward);
				//print("	old point : "+ommak2);
				//Instantiate(testSubCube,SwichZandY(ommak2),Quaternion.identity);
	
				Vector3 newOmmak = chnageTerrainElevationInPointAccordingToAngle(angle,ommak2,newIngredCenters[i]);
				//print("	new point : "+newOmmak);
				betaHeights[(int)ingredientsCenter0.y+1,(int)ingredientsCenter0.x] = newOmmak.z/elevation;
				//print("		angle:"+angle+" ,color:"+colorsS[i]);
				newIngredCenters.Add(newOmmak);

				if(trigger && ommak2 == newOmmak)
				{
					trigger = false;	
					break;
				}else if(ommak2 != newOmmak)
				{
					trigger = true;
				}
				ingredientsCenter0.x++;
				ommak1 = newOmmak;//ommak2;
			}
		}
	}
	void SmoothLineBackwardX(Vector3 startPoint){
		//print("--SmoothLine:@"+startPoint);
		
		bool trigger = false;
		List<Vector3> newIngredCenters;
		if(ingredientsCenters.Count > 0 && betaHeights.Length > 0){
			newIngredCenters = new List<Vector3>();
			//Color[] colors = new Color[]{Color.black,Color.blue,Color.cyan,Color.gray,Color.green,Color.magenta,Color.red,Color.white,Color.yellow,Color.black};
			//String[] colorsS = new String[]{"black","blue","cyan","gray","green","magenta","red","white","yellow","black"};
			Vector3 ingredientsCenter0 = startPoint;//for testing :ingredientsCenters[0];
			ingredientsCenter0 = SwichZandY(ingredientsCenter0);
			Vector3 ommak1 = ingredientsCenter0;
			newIngredCenters.Add(ommak1);
			for (int i = 0; i < 10; i++)
			{
				//Gizmos.color = colors[i];

				Vector3 ommak2 = ingredientsCenter0 ;
				ommak2.x--;//next step
				
				//ommak2 = SwichZandY(ommak2);
				//print("-swiched:"+ommak2 +","+ingredientsCenter0);
				ommak2.z = betaHeights[(int)ingredientsCenter0.y-1,(int)ingredientsCenter0.x]*elevation;
				Vector3 targetDir = ommak2 - ommak1;// - ommak2;
				float angle = Vector3.Angle(targetDir, Vector3.forward);
				//print("	old point : "+ommak2);
				//Instantiate(testSubCube,SwichZandY(ommak2),Quaternion.identity);
	
				Vector3 newOmmak = chnageTerrainElevationInPointAccordingToAngle(angle,ommak2,newIngredCenters[i]);
				//print("	new point : "+newOmmak);
				betaHeights[(int)ingredientsCenter0.y-1,(int)ingredientsCenter0.x] = newOmmak.z/elevation;
				//print("		angle:"+angle+" ,color:"+colorsS[i]);
				newIngredCenters.Add(newOmmak);

				if(trigger && ommak2 == newOmmak)
				{
					trigger = false;	
					break;
				}else if(ommak2 != newOmmak)
				{
					trigger = true;
				}
				ingredientsCenter0.x--;
				ommak1 = newOmmak;//ommak2;
			}
		}
	}
	void SmoothLineY(Vector3 startPoint){
		//print("--SmoothLine:@"+startPoint);
		
		bool trigger = false;
		List<Vector3> newIngredCenters;
		if(ingredientsCenters.Count > 0 && betaHeights.Length > 0){
			newIngredCenters = new List<Vector3>();
			//Color[] colors = new Color[]{Color.black,Color.blue,Color.cyan,Color.gray,Color.green,Color.magenta,Color.red,Color.white,Color.yellow,Color.black};
			//String[] colorsS = new String[]{"black","blue","cyan","gray","green","magenta","red","white","yellow","black"};
			Vector3 ingredientsCenter0 = startPoint;//for testing :ingredientsCenters[0];
			ingredientsCenter0 = SwichZandY(ingredientsCenter0);
			Vector3 ommak1 = ingredientsCenter0;
			newIngredCenters.Add(ommak1);
			for (int i = 0; i < 10; i++)
			{
				//Gizmos.color = colors[i];

				Vector3 ommak2 = ingredientsCenter0 ;
				ommak2.y++;//next step
				
				//ommak2 = SwichZandY(ommak2);
				//print("-swiched:"+ommak2 +","+ingredientsCenter0);
				ommak2.z = betaHeights[(int)ingredientsCenter0.y+1,(int)ingredientsCenter0.x]*elevation;
				Vector3 targetDir = ommak2 - ommak1;// - ommak2;
				float angle = Vector3.Angle(targetDir, Vector3.forward);
				//print("	old point : "+ommak2);
				//Instantiate(testSubCube,SwichZandY(ommak2),Quaternion.identity);
	
				Vector3 newOmmak = chnageTerrainElevationInPointAccordingToAngle(angle,ommak2,newIngredCenters[i]);
				//print("	new point : "+newOmmak);
				betaHeights[(int)ingredientsCenter0.y+1,(int)ingredientsCenter0.x] = newOmmak.z/elevation;
				//print("		angle:"+angle+" ,color:"+colorsS[i]);
				newIngredCenters.Add(newOmmak);

				if(trigger && ommak2 == newOmmak)
				{
					trigger = false;	
					break;
				}else if(ommak2 != newOmmak)
				{
					trigger = true;
				}
				ingredientsCenter0.y++;
				ommak1 = newOmmak;//ommak2;
			}
		}
	}
	void SmoothLineBackwardY(Vector3 startPoint){
		//print("--SmoothLine:@"+startPoint);
		
		bool trigger = false;
		List<Vector3> newIngredCenters;
		if(ingredientsCenters.Count > 0 && betaHeights.Length > 0){
			newIngredCenters = new List<Vector3>();
			//Color[] colors = new Color[]{Color.black,Color.blue,Color.cyan,Color.gray,Color.green,Color.magenta,Color.red,Color.white,Color.yellow,Color.black};
			//String[] colorsS = new String[]{"black","blue","cyan","gray","green","magenta","red","white","yellow","black"};
			Vector3 ingredientsCenter0 = startPoint;//for testing :ingredientsCenters[0];
			ingredientsCenter0 = SwichZandY(ingredientsCenter0);
			Vector3 ommak1 = ingredientsCenter0;
			newIngredCenters.Add(ommak1);
			for (int i = 0; i < 10; i++)
			{
				//Gizmos.color = colors[i];

				Vector3 ommak2 = ingredientsCenter0 ;
				ommak2.y--;//next step
				
				//ommak2 = SwichZandY(ommak2);
				//print("-swiched:"+ommak2 +","+ingredientsCenter0);
				ommak2.z = betaHeights[(int)ingredientsCenter0.y-1,(int)ingredientsCenter0.x]*elevation;
				Vector3 targetDir = ommak2 - ommak1;// - ommak2;
				float angle = Vector3.Angle(targetDir, Vector3.forward);
				//print("	old point : "+ommak2);
				//Instantiate(testSubCube,SwichZandY(ommak2),Quaternion.identity);
	
				Vector3 newOmmak = chnageTerrainElevationInPointAccordingToAngle(angle,ommak2,newIngredCenters[i]);
				//print("	new point : "+newOmmak);
				betaHeights[(int)ingredientsCenter0.y-1,(int)ingredientsCenter0.x] = newOmmak.z/elevation;
				//print("		angle:"+angle+" ,color:"+colorsS[i]);
				newIngredCenters.Add(newOmmak);

				if(trigger && ommak2 == newOmmak)
				{
					trigger = false;	
					break;
				}else if(ommak2 != newOmmak)
				{
					trigger = true;
				}
				ingredientsCenter0.y--;
				ommak1 = newOmmak;//ommak2;
			}
		}
	}

	Vector3 SwichZandY(Vector3 vector3){
		float z = vector3.y;
		vector3.y = vector3.z;
		vector3.z = z;
		return vector3;
	}
	Vector3 SwichXandY(Vector3 vector3){
		float x = vector3.x;
		vector3.x = vector3.y;
		vector3.y = x;
		return vector3;
	}
	
	
    void OnDrawGizmos() {
		/*
		bool trigger = false;
		if(ingredientsCenters.Count > 0 && betaHeights.Length > 0){
			List<Vector3> newIngredCenters = new List<Vector3>();
			Color[] colors = new Color[]{Color.black,Color.blue,Color.cyan,Color.gray,Color.green,Color.magenta,Color.red,Color.white,Color.yellow,Color.black};
			String[] colorsS = new String[]{"black","blue","cyan","gray","green","magenta","red","white","yellow","black"};
			Vector3 ingredientsCenter0 = ingredientsCenters[0];
			ingredientsCenter0 = SwichZandY(ingredientsCenter0);
			Vector3 ommak1 = ingredientsCenter0;
			newIngredCenters.Add(ommak1);
			for (int i = 0; i < 10; i++)
			{
				Gizmos.color = colors[i];

				Vector3 ommak2 = ingredientsCenter0 ;
				ommak2.x++;//next step
				
				//ommak2 = SwichZandY(ommak2);
				//print("-swiched:"+ommak2 +","+ingredientsCenter0);
				ommak2.z = betaHeights[(int)ingredientsCenter0.x+1,(int)ingredientsCenter0.y]*elevation;
				Vector3 targetDir = ommak2 - ommak1;// - ommak2;
				float angle = Vector3.Angle(targetDir, Vector3.forward);
				print("old point : "+ommak2);
				Vector3 newOmmak = chnageTerrainElevationInPointAccordingToAngle(angle,ommak2,newIngredCenters[i]);
				print("new point : "+newOmmak);
				print("		angle:"+angle+" ,color:"+colorsS[i]);
				newIngredCenters.Add(newOmmak);
				var drawOmmak2 = SwichZandY(SwichXandY(ommak2));
				var drawNewOmmak2 = SwichZandY(SwichXandY(newOmmak));
				var drawIngred = SwichZandY(SwichXandY(ingredientsCenter0));
				//ommak2.y+=5;//+=2
				Gizmos.DrawWireSphere(drawNewOmmak2, .2f);
				Gizmos.DrawWireCube(drawOmmak2, new Vector3(.2f,.4f,.2f));
				Gizmos.DrawCube(drawIngred, new Vector3(.2f,.2f,.2f));

				if(trigger && ommak2 == newOmmak)
				{
					trigger = false;	
					break;
				}else if(ommak2 != newOmmak)
				{
					trigger = true;
				}
				ingredientsCenter0.x++;
				ommak1 = newOmmak;//ommak2;
			}
		}
		*/
		/*
        for (int i = 0; i < ingredientsPoints.Count; i+=5)
		{
			Vector3 center = ingredientsPoints[i]    ;
			Vector3 point1 = ingredientsPoints[i+1]  ;
			Vector3 point2 = ingredientsPoints[i+2]  ;
			Vector3 point3 = ingredientsPoints[i+3]  ;
			Vector3 point4 = ingredientsPoints[i+4]  ;

			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(center,0.1f);
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(point1,0.1f);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(point2,0.1f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(point3,0.1f);
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(point4,0.1f);
			
		}
		*/
    }

    private Vector3 chnageTerrainElevationInPointAccordingToAngle(float angle, Vector3 ommak2, Vector3 lastNewPoint)
    {
		var newDirection = ommak2;
		var deffranceInAngle = angle - 90;
		//print("		deffranceInAngle:"+deffranceInAngle+", Abs:"+(Mathf.Abs(deffranceInAngle) > 30));
		//if deffranceInAngle is positive value , then the point to be reached is high
		//else if deffranceInAngle is nigatve , then the point is low
        if(Mathf.Abs(deffranceInAngle) > 30){
			var rot = Quaternion.AngleAxis(30,Vector3.forward);
			
			newDirection = (rot * Vector3.forward)*.8f;
			//print("			rot:"+rot+" >> "+newDirection);
			newDirection.x = ommak2.x;
			newDirection.y = ommak2.y;
			newDirection.z = lastNewPoint.z + (ommak2.z)*UnityEngine.Random.Range(0.20f,.25f);
			//print("			new z:"+newDirection.z+"vs old z:"+ommak2.z);
		}
		return newDirection;
    }
}

