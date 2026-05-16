using Unity.Cinemachine;
using UnityEngine;

public class SeguirJugadorCamara : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;

    private void Start()
    {
        SeguirPlayer();
    }

    private void SeguirPlayer()
    {
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();

        if (player == null)
        {
            Debug.LogWarning("No se encontro el jugador");

            return;
        }

        Transform playerTransform = player.transform;

        cinemachineCamera.Follow = playerTransform;
    }
}