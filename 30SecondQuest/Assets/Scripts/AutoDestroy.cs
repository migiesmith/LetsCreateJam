using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {

	public float timeTillDestroyed = 5.0f;

	void Start () {
		
	}
	
	void Update () {
		timeTillDestroyed -= Time.deltaTime;
		if(timeTillDestroyed <= 0.0f)
			Destroy(this.gameObject);
	}
}
