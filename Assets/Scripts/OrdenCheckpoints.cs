using System.Collections.Generic;
using UnityEngine;

// Obliga a pasar los checkpoints en orden.
// Solo el siguiente checkpoint está activo; si el jugador va en sentido
// contrario, los que encuentra están apagados y no cuentan.
public class OrdenCheckpoints : MonoBehaviour
{
    [Tooltip("Arrastra aquí los checkpoints en el orden en que se deben pasar")]
    [SerializeField] private List<Collider> checkpointsEnOrden = new List<Collider>();

    private int siguiente = 0;

    void Start()
    {
        // Al iniciar, solo el primer checkpoint queda activo
        for (int i = 0; i < checkpointsEnOrden.Count; i++)
        {
            if (checkpointsEnOrden[i] != null)
                checkpointsEnOrden[i].enabled = (i == 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (siguiente >= checkpointsEnOrden.Count) return;
        if (other != checkpointsEnOrden[siguiente]) return;

        // Pasó el checkpoint correcto: se activa el siguiente
        siguiente++;
        if (siguiente < checkpointsEnOrden.Count && checkpointsEnOrden[siguiente] != null)
            checkpointsEnOrden[siguiente].enabled = true;
    }
}