using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundComponent : MonoBehaviour
{
    float scrollSpeed = 0.1f;

    Vector2 offset;
    Renderer backgroundRenderer;
    // Start is called before the first frame update
    void Start()
    {
        backgroundRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Dot(Physics.gravity.normalized, Vector3.up) == 1.0f)
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        else if (Vector3.Dot(Physics.gravity.normalized, Vector3.right) == 1.0f)
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
        }
        else if (Vector3.Dot(Physics.gravity.normalized, Vector3.down) == 1.0f)
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
        }
        else if (Vector3.Dot(Physics.gravity.normalized, Vector3.left) == 1.0f)
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
        }

        offset = new Vector2(offset.x, offset.y - scrollSpeed*Time.unscaledDeltaTime);
        backgroundRenderer.material.mainTextureOffset = offset;
    }
}
