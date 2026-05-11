using UnityEngine;

public class MovementGoodEllf : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private float velocidadMovimiento;
    [SerializeField]private float entradaHorizontal;

    private void Update()
    {
        entradaHorizontal = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        rb2D.linearVelocity = new Vector2(entradaHorizontal * velocidadMovimiento, rb2D.linearVelocity.y);

        if(entradaHorizontal > 0 && !MirandoALaDerecha()){
        
        }
    }

    private bool MirandoALaDerecha()
    {
        return transform.localScale.x == 1;
    }
}
