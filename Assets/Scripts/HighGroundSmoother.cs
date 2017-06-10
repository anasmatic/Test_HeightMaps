using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighGroundSmoother : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	internal static void SmoothLineZHigh(Vector3 startPoint,ref float[,] betaHeights, float elevation){
		//print("--SmoothLine:@"+startPoint);
		
		bool trigger = false;
		List<Vector3> newIngredCenters;
		if(betaHeights.Length > 0){
			newIngredCenters = new List<Vector3>();
			//Color[] colors = new Color[]{Color.black,Color.blue,Color.cyan,Color.gray,Color.green,Color.magenta,Color.red,Color.white,Color.yellow,Color.black};
			//String[] colorsS = new String[]{"black","blue","cyan","gray","green","magenta","red","white","yellow","black"};
			Vector3 ingredientsCenter0 = startPoint;//for testing :ingredientsCenters[0];
			ingredientsCenter0 = SmoothersHelpers.SwichZandY(ingredientsCenter0);
			Vector3 ommak1 = ingredientsCenter0;
			newIngredCenters.Add(ommak1);
			for (int i = 0; i < 10; i++)
			{
				//Gizmos.color = colors[i];

				Vector3 ommak2 = ingredientsCenter0 ;
				ommak2.x++;//next step
				
				//ommak2 = SwichZandY(ommak2);
				//print("-swiched:"+ommak2 +","+ingredientsCenter0);
				ommak2.z = betaHeights[(int)ingredientsCenter0.x+1,(int)ingredientsCenter0.y]*elevation;
				Vector3 targetDir = ommak2 - ommak1;// - ommak2;
				float angle = Vector3.Angle(targetDir, Vector3.forward);
				print("	old point : "+ommak2);
				//Instantiate(testSubCube,SmoothersHelpers.SwichZandY(ommak2),Quaternion.identity);
	
				Vector3 newOmmak = ChnageTerrainElevationInPointAccordingToAngle(angle,ommak2,newIngredCenters[i], false);
				print("	new point : "+newOmmak);
				betaHeights[(int)ingredientsCenter0.x+1,(int)ingredientsCenter0.y] = newOmmak.z/elevation;
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

	internal static void SmoothLineBackwardZHigh(Vector3 startPoint,ref float[,] betaHeights, float elevation)
	{
		//print("--SmoothLine:@"+startPoint);
		
		bool trigger = false;
		List<Vector3> newIngredCenters;
		if(betaHeights.Length > 0){
			newIngredCenters = new List<Vector3>();
			//Color[] colors = new Color[]{Color.black,Color.blue,Color.cyan,Color.gray,Color.green,Color.magenta,Color.red,Color.white,Color.yellow,Color.black};
			//String[] colorsS = new String[]{"black","blue","cyan","gray","green","magenta","red","white","yellow","black"};
			Vector3 ingredientsCenter0 = startPoint;//for testing :ingredientsCenters[0];
			ingredientsCenter0 = SmoothersHelpers.SwichZandY(ingredientsCenter0);
			Vector3 ommak1 = ingredientsCenter0;
			newIngredCenters.Add(ommak1);
			for (int i = 0; i < 10; i++)
			{
				//Gizmos.color = colors[i];

				Vector3 ommak2 = ingredientsCenter0 ;
				ommak2.x--;//next step
				
				//ommak2 = SwichZandY(ommak2);
				//print("-swiched:"+ommak2 +","+ingredientsCenter0);
				ommak2.z = betaHeights[(int)ingredientsCenter0.x-1,(int)ingredientsCenter0.y]*elevation;
				Vector3 targetDir = ommak2 - ommak1;// - ommak2;
				float angle = Vector3.Angle(targetDir, Vector3.forward);
				//print("	old point : "+ommak2);
				//Instantiate(testSubCube,SwichZandY(ommak2),Quaternion.identity);
	
				Vector3 newOmmak = ChnageTerrainElevationInPointAccordingToAngle(angle,ommak2,newIngredCenters[i],true);
				//print("	new point : "+newOmmak);
				betaHeights[(int)ingredientsCenter0.x-1,(int)ingredientsCenter0.y] = newOmmak.z/elevation;
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

	static Vector3 ChnageTerrainElevationInPointAccordingToAngle(float angle, Vector3 ommak2, Vector3 lastNewPoint, bool backward)
    {
		var newDirection = ommak2;
		var deffranceInAngle = angle - 90;
		//print("		deffranceInAngle:"+deffranceInAngle+", Abs:"+(Mathf.Abs(deffranceInAngle) > 30));
		//if deffranceInAngle is positive value , then the point to be reached is high
		//else if deffranceInAngle is nigatve , then the point is low
        if(Mathf.Abs(deffranceInAngle) > 30){
			var rot = Quaternion.AngleAxis(30,Vector3.forward);
			
			newDirection = (rot * Vector3.forward)*.8f;
			print("			rot:"+rot+" >> "+newDirection+" , defAngl:"+deffranceInAngle);
			newDirection.x = ommak2.x;
			newDirection.y = ommak2.y;
			if(backward)
				newDirection.z = lastNewPoint.z - (ommak2.z)*UnityEngine.Random.Range(0.15f,.25f);
			else
				newDirection.z = lastNewPoint.z - (ommak2.z)*UnityEngine.Random.Range(0.80f,.75f);

			print("			new z:"+newDirection.z+"vs old z:"+ommak2.z);
		}
		return newDirection;
    }
}
