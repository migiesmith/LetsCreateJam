using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	private float _moveSpeed = 0.25f, _scaleSpeed = 0.1f;
	private Coroutine _moveCoroutine;

	void Start () {
		Vector3 localScale = transform.localScale;
		transform.localScale = new Vector3(0,0,0);
		StartCoroutine(scaleUp(localScale));
	}
	
	
	void Update () {
		
	}

	public void moveTo(Vector3 newPos)
	{
		if(_moveCoroutine != null)
			StopCoroutine(_moveCoroutine);
		_moveCoroutine = StartCoroutine(lerpMoveTo(newPos));
	}

	private IEnumerator lerpMoveTo(Vector3 newPos)
	{
		while(Vector3.Distance(transform.position, newPos) > 0.01f)
		{
			transform.position = Vector3.Lerp(transform.position, newPos, _moveSpeed);
			yield return null;
		}
		transform.position = newPos;
	}

	private IEnumerator scaleUp(Vector3 newScale)
	{
		while(Mathf.Abs(transform.localScale.magnitude - newScale.magnitude) > 0.01f)
		{
			transform.localScale = Vector3.Lerp(transform.localScale, newScale, _scaleSpeed);
			yield return null;
		}
		transform.localScale = newScale;
	}
}
