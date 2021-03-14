using UnityEngine;
using System.Collections;

public class DestroyByTime : MonoBehaviour {

	public float lifetime;

	private float startTime;

	void Start () {
		startTime = Time.time;
	}

	private void OnEnable()
	{
		startTime = Time.time;
	}

	void Update () {

		if (Time.time > (lifetime + startTime)) {
			gameObject.SetActive (false);
		}
	}
}
