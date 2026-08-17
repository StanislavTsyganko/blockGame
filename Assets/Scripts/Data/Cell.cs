using UnityEngine;
using UnityEngine.UIElements;

public class Cell : MonoBehaviour
{
    public Vector3 position { get; set; }
    public bool destroyed { get; set; }
    string type { get; set; }

    public void Initialize(Vector3 position, string type)
    {
        this.position = position;
        this.type = type;
        destroyed = false;
    }
}
