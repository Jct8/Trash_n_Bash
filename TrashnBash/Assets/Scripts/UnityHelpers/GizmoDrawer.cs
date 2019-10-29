using UnityEngine;

public class GizmoDrawer : MonoBehaviour
{
    public enum EGizmoShape
    {
        WireSphere = 0,
        Sphere,
        Cube,
        WireCube,
    }

    public enum EGizmoColor
    {
        Blue,
        Cyan,
        Green,
        Magenta,
        Red,
        White,
        Yellow,
    }

    // Defined in Inspector
    public bool showGizmo = false;
    public EGizmoShape gizmoShape = EGizmoShape.Cube;
    public EGizmoColor gizmoColour = EGizmoColor.Blue;
    public float gizmoRadius = 0.0f;
    public Vector3 gizmoScale = Vector3.zero;

    private void OnDrawGizmos()
    {
        if (!showGizmo) return;
        switch (gizmoColour)
        {
            case EGizmoColor.Blue:
                Gizmos.color = Color.blue;
                break;
            case EGizmoColor.Cyan:
                Gizmos.color = Color.cyan;
                break;
            case EGizmoColor.Green:
                Gizmos.color = Color.green;
                break;
            case EGizmoColor.Magenta:
                Gizmos.color = Color.magenta;
                break;
            case EGizmoColor.Red:
                Gizmos.color = Color.red;
                break;
            case EGizmoColor.White:
                Gizmos.color = Color.white;
                break;
            case EGizmoColor.Yellow:
                Gizmos.color = Color.yellow;
                break;
        }


        switch (gizmoShape)
        {
            case EGizmoShape.Cube:
                Gizmos.DrawCube(transform.position, gizmoScale);
                break;
            case EGizmoShape.Sphere:
                Gizmos.DrawSphere(transform.position, gizmoRadius);
                break;
            case EGizmoShape.WireCube:
                Gizmos.DrawWireCube(transform.position, gizmoScale);
                break;
            case EGizmoShape.WireSphere:
                Gizmos.DrawWireSphere(transform.position, gizmoRadius);
                break;
        }
    }
}