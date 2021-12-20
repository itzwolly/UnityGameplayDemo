using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    [SerializeField] private PlayerBehaviour _playerOne;
    [SerializeField] private PlayerBehaviour _playerTwo;
    [SerializeField] private GameObject _spawnPointPlayerOne;
    [SerializeField] private GameObject _spawnPointPlayerTwo;
    [SerializeField] private GameObject _dropPrefab;

    private GameObject _currentSavePoint = null;
    private readonly List<GameObject> _enemiesToRespawn = new List<GameObject>();

    // Start is called before the first frame update
    private void Start() {
        _playerOne.OnDied += onPlayerDied;
        _playerOne.OnKilledEnemy += onPlayerKilledEnemy;
        _playerOne.OnSavePointEntered += onSavePointEntered;
        _playerOne.OnDropPickup += onDropPickup;

        _playerTwo.OnDied += onPlayerDied;
        _playerTwo.OnKilledEnemy += onPlayerKilledEnemy;
        _playerTwo.OnSavePointEntered += onSavePointEntered;
        _playerTwo.OnDropPickup += onDropPickup;
    }

    private void resetLevel() {
        // Set the position of players to the last spawn point position
        _playerOne.transform.position = _spawnPointPlayerOne.transform.position;
        _playerOne.transform.rotation = _spawnPointPlayerOne.transform.rotation;

        _playerTwo.transform.position = _spawnPointPlayerTwo.transform.position;
        _playerTwo.transform.rotation = _spawnPointPlayerTwo.transform.rotation;

        // Reset the local player's gravity
        _playerOne.ResetGravity();
        _playerTwo.ResetGravity();

        // Remove all existing drops in the level
        foreach (GameObject drop in GameObject.FindGameObjectsWithTag("Drop")) {
            Destroy(drop);
        }

        // Respawn enemies that are in the respawn list
        foreach (GameObject enemy in _enemiesToRespawn) {
            enemy.SetActive(true);
        }
    }

    private void spawnDrop(Transform targetTransform) {
        GameObject drop = Instantiate(_dropPrefab, new Vector3(targetTransform.position.x, Mathf.Abs(targetTransform.position.y), targetTransform.position.z), Quaternion.identity);
        Rigidbody rigidbody = drop.GetComponent<Rigidbody>();
        rigidbody.AddForce(new Vector3(0, 4, 0), ForceMode.Impulse);
    }

    private void onPlayerKilledEnemy(object sender, Transform enemyTransform) {
        // Add the killed enemy to the respawn list
        _enemiesToRespawn.Add(enemyTransform.gameObject);
        // Disable the enemy
        enemyTransform.gameObject.SetActive(false);
        // Spawn a drop for the player to pickup
        spawnDrop(enemyTransform);
    }

    private void onPlayerDied(object sender, System.EventArgs e) {
        resetLevel();
    }

    private void onSavePointEntered(object sender, GameObject savePoint) {
        if (_currentSavePoint != savePoint) {
            // Set the new spawn points for both players
            _spawnPointPlayerOne = savePoint.transform.GetChild(0).gameObject;
            _spawnPointPlayerTwo = savePoint.transform.GetChild(1).gameObject;
            // Clear the list of enemies that were killed before we hit the save point
            _enemiesToRespawn.Clear();
            // Apply the current save point, so saving only occurs once for this save point object
            _currentSavePoint = savePoint;
        }
    }

    private void onDropPickup(object sender, GameObject e) {
        // Remove the drop
        Destroy(e);
        // This would also be the place to assign points
    }

    /// <summary>
    /// Gets a player based on the given <see cref="PlayerBehaviour.PlayerType"/>.
    /// </summary>
    /// <param name="playerType">The type of player you want to get.</param>
    /// <returns>A <see cref="PlayerBehaviour"/> script of a given player.</returns>
    public static PlayerBehaviour GetPlayerByType(PlayerBehaviour.PlayerType playerType) {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject gameObject in gameObjects) {
            PlayerBehaviour player = gameObject.GetComponent<PlayerBehaviour>();
            if (player.CurrentPlayerType == playerType) {
                return player;
            }
        }
        return null;
    }
}
