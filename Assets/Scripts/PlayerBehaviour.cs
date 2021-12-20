using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConstantForce))]
[RequireComponent(typeof(Animator))]
public class PlayerBehaviour : MonoBehaviour
{
    public PlayerType CurrentPlayerType;

    [SerializeField] private KeyCode _leftKey;
    [SerializeField] private KeyCode _rightKey;
    [SerializeField] private KeyCode _jumpKey;
    [SerializeField] private float _speed = 100f;
    [SerializeField] private float _jumpForce = 20f;
    [SerializeField] private float _gravityChangeTimeInSeconds = 0.075f;

    private Rigidbody _rigidbody;
    private ConstantForce _constantForce;
    private Animator _animator;
    private bool _isJumping = false;
    private Vector3 _originalGravity;

    public event EventHandler OnDied;
    public event EventHandler<Transform> OnKilledEnemy;
    public event EventHandler<GameObject> OnSavePointEntered;
    public event EventHandler<GameObject> OnDropPickup;

    // Start is called before the first frame update
    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _constantForce = GetComponent<ConstantForce>();
        _animator = GetComponent<Animator>();

        _originalGravity = _constantForce.force;
    }

    // Update is called once per frame
    private void Update() {
        if (Input.GetKeyDown(_jumpKey) && !_isJumping) {
            _rigidbody.AddForce(-_constantForce.force.normalized * _jumpForce, ForceMode.Impulse);
            _isJumping = true;
        }
        _animator.SetBool("Jump", _isJumping);
    }

    private void FixedUpdate() {
        bool leftKey = Input.GetKey(_leftKey);
        bool rightKey = Input.GetKey(_rightKey);

        // We're only interested in X and Y regarding movement
        if (leftKey) {
            _rigidbody.velocity = new Vector2(-_speed * Time.deltaTime, _rigidbody.velocity.y);
        } else if (rightKey) {
            _rigidbody.velocity = new Vector2(_speed * Time.deltaTime, _rigidbody.velocity.y);
        }
        // Reset velocity if we're not pressing the movement keys
        if (!leftKey && !rightKey) {
            _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
        }
        _animator.SetBool("Move", _rigidbody.velocity.x != 0);
    }

    private void OnCollisionEnter(Collision collision) {
        // Allow the user to jump again once we collide with something
        _isJumping = false;

        if (collision.transform.tag == "EnemyKillBox") {
            OnKilledEnemy?.Invoke(this, collision.transform.parent);
        } else if (collision.transform.tag == "Enemy") {
            OnDied?.Invoke(this, EventArgs.Empty);
        } else if (collision.transform.tag == "Drop") {
            OnDropPickup?.Invoke(this, collision.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.transform.tag == "Portal") {
            PortalBehaviour portalBehaviour = other.transform.GetComponent<PortalBehaviour>();
            if (portalBehaviour.ToggleGravity) {
                toggleGravityCoroutine();
            }
        } else if (other.transform.tag == "SavePoint") {
            OnSavePointEntered?.Invoke(this, other.gameObject);
        }
    }

    private void OnAnimatorMove() {
        // Determine which way we're looking based on direction and current gravity
        if (_rigidbody.velocity.x > 0) { // Moving Right
            transform.rotation = Quaternion.LookRotation(
                (FlippedGravity) ? new Vector3(0, 0, -1.0f) : new Vector3(0, 0, 1.0f), -_constantForce.force);
        } else if (_rigidbody.velocity.x < 0) { // Moving Left
            transform.rotation = Quaternion.LookRotation(
                (FlippedGravity) ? new Vector3(0, 0, 1.0f) : new Vector3(0, 0, -1.0f), -_constantForce.force);
        }

        // Determine if we're falling based on our current Y velocity and the current direction of gravity.
        bool isFalling = FlippedGravity ? _rigidbody.velocity.y > 0 : _rigidbody.velocity.y < 0;
        _animator.SetBool("Falling", isFalling);
    }

    /// <summary>
    /// Starts a coroutine which toggles the direction of gravity applied to the player after a certain amount of time.
    /// </summary>
    private void toggleGravityCoroutine() {
        StartCoroutine(toggleGravity());
    }

    /// <summary>
    /// Toggles the gravity by changing the constant force properties of a given player after a given delay.
    /// </summary>
    /// <returns><see cref="IEnumerator"/> used in a coroutine.</returns>
    private IEnumerator toggleGravity() {
        yield return new WaitForSeconds(_gravityChangeTimeInSeconds);
        _constantForce.force = -_constantForce.force;
    }

    public void ResetGravity() {
        _constantForce.force = _originalGravity;
    }

    /// <summary>
    /// Gets whether or not the gravity is flipped for the current player.
    /// </summary>
    public bool FlippedGravity => _constantForce.force.y > 0;

    public enum PlayerType {
        Ibb,
        Obb
    }
}
