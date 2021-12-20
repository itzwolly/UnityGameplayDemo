using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveBehaviour : MonoBehaviour
{
    [SerializeField] private Transform[] _points;
    [SerializeField] private float _speed;

    private int _pointIndex;
    private Transform _currentPoint;
    private Vector3 _startingRotation;

    // Start is called before the first frame update
    private void Start() {
        _currentPoint = _points[_pointIndex];
        _startingRotation = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    private void Update() {
        if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector3.down, out RaycastHit hit)) {
            if (hit.transform.tag == "Wall") {
                transform.rotation = Quaternion.Euler(_startingRotation + hit.transform.rotation.eulerAngles);
            }
        }

        // Move our position a step closer to the target point
        float step = _speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, _currentPoint.position, step);
        // Check if the position of the enemy and the current point are approximately equal
        if (Vector3.Distance(transform.position, _currentPoint.position) < 0.001f) {
            // Checks if the point index is equal to or bigger than length-1
            // and if this is the case sets it to negative length-1.
            if (_pointIndex >= _points.Length - 1) {
                _pointIndex = -(_points.Length - 1);
            }
            // We use the absolute value when we do the lookup so
            // it results in a pattern like this: 0, 1, 2, 1, 0, 1, 2, etc.
            _currentPoint = _points[Mathf.Abs(_pointIndex)];
            _pointIndex++;
        }
    }
}
