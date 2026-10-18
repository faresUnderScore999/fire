using UnityEngine;
using UnityEngine.InputSystem;

public class JoueurSimple : MonoBehaviour
{
    public float vitesse = 3f;
    public float sensibilite = 0.15f;
    private float inclinaison = 0f;

    void Update()
    {
        var clavier = Keyboard.current;
        var souris = Mouse.current;
        if (clavier == null || souris == null) return;

        // Clic droit maintenu + souris = regarder autour
        if (souris.rightButton.isPressed)
        {
            Vector2 d = souris.delta.ReadValue() * sensibilite;
            inclinaison = Mathf.Clamp(inclinaison - d.y, -80f, 80f);
            float y = transform.eulerAngles.y + d.x;
            transform.rotation = Quaternion.Euler(inclinaison, y, 0f);
        }

        // Z Q S D (ou fleches) = se deplacer
        Vector3 avant = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 droite = new Vector3(transform.right.x, 0, transform.right.z).normalized;
        Vector3 mouvement = Vector3.zero;

        if (clavier.wKey.isPressed || clavier.upArrowKey.isPressed) mouvement += avant;
        if (clavier.sKey.isPressed || clavier.downArrowKey.isPressed) mouvement -= avant;
        if (clavier.dKey.isPressed || clavier.rightArrowKey.isPressed) mouvement += droite;
        if (clavier.aKey.isPressed || clavier.leftArrowKey.isPressed) mouvement -= droite;

        transform.position += mouvement.normalized * vitesse * Time.deltaTime;
    }
}