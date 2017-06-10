using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAngle : MonoBehaviour {

	public Transform[] cubes;

	// Use this for initialization
	void Start () {
		StartCoroutine("Routine");
	}

    private IEnumerator Routine()
    {
		for (int i = 0; i < cubes.Length; i++)
		{
			var rot = Quaternion.AngleAxis(45*i,Vector3.forward);
			var newDirection = rot * Vector3.down;
			cubes[i].transform.position = newDirection;
			print(i+","+45*i+","+newDirection+","+rot);
       		yield return new WaitForSeconds(3);
		}
    }

    private Vector3 chnageTerrainElevationInPointAccordingToAngle(float angle, Vector3 ommak2)
    {
		var newDirection = ommak2;
		var deffranceInAngle = angle - 90;
		print("		deffranceInAngle:"+deffranceInAngle+", Abs:"+(Mathf.Abs(deffranceInAngle) > 20));
		//if deffranceInAngle is positive value , then the point to be reached is high
		//else if deffranceInAngle is nigatve , then the point is low
        if(Mathf.Abs(deffranceInAngle) > 45){
			var rot = Quaternion.AngleAxis(0,Vector3.forward);
			print("				rot:"+rot);
			newDirection = rot * Vector3.forward;
			newDirection.x = ommak2.x;
			newDirection.y = ommak2.y;
		}
		return newDirection;
    }
}
