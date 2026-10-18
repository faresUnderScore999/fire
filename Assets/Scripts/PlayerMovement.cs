using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float vitesse = 3f;
    public float sensibilite = 0.15f;

    CharacterController controller;
    Animator anim;
    Transform modele;
    float vy;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        if (anim != null) modele = anim.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        Mouse mouse = Mouse.current;
        if (kb == null) return;

        // Souris : tourner (Echap libere, clic reprend)
        if (kb.escapeKey.wasPressedThisFrame) Cursor.lockState = CursorLockMode.None;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame) Cursor.lockState = CursorLockMode.Locked;
        if (mouse != null && Cursor.lockState == CursorLockMode.Locked)
            transform.Rotate(0, mouse.delta.ReadValue().x * sensibilite, 0);

        // Touches ZQSD + fleches
        Vector2 input = Vector2.zero;
        if (kb.zKey.isPressed || kb.wKey.isPressed || kb.upArrowKey.isPressed) input.y += 1;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) input.y -= 1;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) input.x += 1;
        if (kb.qKey.isPressed || kb.aKey.isPressed || kb.leftArrowKey.isPressed) input.x -= 1;

        Vector3 dir = transform.right * input.x + transform.forward * input.y;
        if (dir.magnitude > 1f) dir.Normalize();
        bool bouge = dir.magnitude > 0.1f;

        // Gravite
        if (controller.isGrounded && vy < 0) vy = -2f;
        vy += -9.81f * Time.deltaTime;
        controller.Move((dir * vitesse + Vector3.up * vy) * Time.deltaTime);

        // Animation
        if (anim != null) anim.SetBool("marche", bouge);

        // Le robot se tourne vers sa direction
        if (bouge && modele != null)
        {
            Vector3 local = transform.InverseTransformDirection(dir);
            modele.localRotation = Quaternion.Slerp(modele.localRotation, Quaternion.LookRotation(local), 10f * Time.deltaTime);
        }
    }
}