using UnityEngine;
using UnityEngine.SceneManagement;

public class PorteScene : MonoBehaviour
{
    [SerializeField] private string nomScene = "molkaScene";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(nomScene);
        }
    }
}