using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroFollower : MonoBehaviour {
	Quaternion _lastRotate;
	bool _canRotate;
	Vector3 _initAngle;

	void Awake(){
		_initAngle = transform.localEulerAngles;
	}

	void OnEnable(){
		StartCoroutine (GyroTrigger(true,2f));
	}

	// Update is called once per frame
	void Update() {
		if (!_canRotate) {
			return;
		}
		Quaternion relative = Quaternion.Inverse(_lastRotate) * Input.gyro.attitude;
		transform.localEulerAngles -= new Vector3 (relative.eulerAngles.x, relative.eulerAngles.y, 0);

		_lastRotate = Input.gyro.attitude;

	}

	public void ResetAngle(){
		transform.localEulerAngles = _initAngle;
	}

	IEnumerator GyroTrigger(bool enableGyro, float delay){
		yield return new WaitForSeconds (delay);
		_canRotate = enableGyro;
	}

	void OnDisable(){
		StopCoroutine ("GyroTrigger");
		ResetAngle ();
		_canRotate = false;
	}
}
