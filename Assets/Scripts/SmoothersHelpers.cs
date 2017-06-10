using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothersHelpers : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	internal static Vector3 chnageTerrainElevationInPointAccordingToAngle(float angle, Vector3 ommak2, Vector3 lastNewPoint)
    {
		var newDirection = ommak2;
		var deffranceInAngle = angle - 90;
		//print("		deffranceInAngle:"+deffranceInAngle+", Abs:"+(Mathf.Abs(deffranceInAngle) > 30));
		//if deffranceInAngle is positive value , then the point to be reached is high
		//else if deffranceInAngle is nigatve , then the point is low
        if(Mathf.Abs(deffranceInAngle) > 30){
			var rot = Quaternion.AngleAxis(30,Vector3.forward);
			
			newDirection = (rot * Vector3.forward)*.8f;
			print("			rot:"+rot+" >> "+newDirection);
			newDirection.x = ommak2.x;
			newDirection.y = ommak2.y;
			if(deffranceInAngle <= 0)
				newDirection.z = lastNewPoint.z + (ommak2.z)*UnityEngine.Random.Range(0.20f,.25f);
			else
				newDirection.z = lastNewPoint.z - (ommak2.z)*UnityEngine.Random.Range(0.75f,.80f);

			print("			new z:"+newDirection.z+"vs old z:"+ommak2.z);
		}
		return newDirection;
    }


	internal static Vector3 SwichZandY(Vector3 vector3){
		float z = vector3.y;
		vector3.y = vector3.z;
		vector3.z = z;
		return vector3;
	}
	internal static Vector3 SwichXandY(Vector3 vector3){
		float x = vector3.x;
		vector3.x = vector3.y;
		vector3.y = x;
		return vector3;
	}
}
