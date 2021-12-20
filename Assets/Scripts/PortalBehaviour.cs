using UnityEngine;

public class PortalBehaviour : MonoBehaviour {
    [SerializeField] private PortalType _portalType;
    [SerializeField] private Collider _platformCollider;
    [SerializeField] private MeshRenderer _platformMeshRenderer;

    public bool ToggleGravity = true;

    private ParticleSystem _particleSystem;
    private Collider _portalCollider;

    private void Awake() {
        _portalCollider = GetComponent<Collider>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        setParticleColor();
        _particleSystem.Play();
    }

    private void Start() {
        // Disable platform renderer once we go into the game,
        // the renderer is only relevant in the editor
        _platformMeshRenderer.enabled = false;
        // Setup which colliders should be ignored
        setupColliders();
    }

    private void setupColliders() {
        // Get the collider for the specific player
        Collider ibbCollider = Game.GetPlayerByType(PlayerBehaviour.PlayerType.Ibb).GetComponent<Collider>();
        Collider obbCollider = Game.GetPlayerByType(PlayerBehaviour.PlayerType.Obb).GetComponent<Collider>();
        // Update the colliders based on portal type and player
        switch (_portalType) {
            case PortalType.Ibb:
                // Ignore platform collision for IBB and portal collision for OBB
                Physics.IgnoreCollision(_platformCollider, ibbCollider, true);
                Physics.IgnoreCollision(_portalCollider, obbCollider, true);
                break;
            case PortalType.Obb:
                // Ignore platform collision for OBB and portal collision for IBB
                Physics.IgnoreCollision(_platformCollider, obbCollider, true);
                Physics.IgnoreCollision(_portalCollider, ibbCollider, true);
                break;
            case PortalType.Both:
            default:
                // Ignore platform collision for both IBB and OBB
                Physics.IgnoreCollision(_platformCollider, obbCollider, true);
                Physics.IgnoreCollision(_platformCollider, ibbCollider, true);
                break;
        }
    }

    private void setParticleColor() {
        ParticleSystem.MainModule main = _particleSystem.main;
        switch (_portalType) {
            case PortalType.Ibb:
                main.startColor = Color.green;
                break;
            case PortalType.Obb:
                main.startColor = Color.red;
                break;
            case PortalType.Both:
            default:
                main.startColor = Color.white;
                break;
        }
    }

    private enum PortalType {
        Both,
        Ibb,
        Obb
    }
}
