using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientToGround : MonoBehaviour
{
    [SerializeField]
    private OrientType orienttype = OrientType.Normal;
    [SerializeField]
    private LayerMask ground;

    // Start is called before the first frame update
    void Awake()
    {
        if (Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit, 2.5f, ground, QueryTriggerInteraction.Ignore))
        {
            Vector3 axis = hit.normal;
            switch (orienttype)
            {
                case OrientType.Normal:
                    break;
                case OrientType.CrossRight:
                    axis = Vector3.Cross(this.transform.right, hit.normal);
                    axis = Vector3.Cross(hit.normal, axis);
                    break;
                case OrientType.CrossForward:
                    axis = Vector3.Cross(this.transform.forward, hit.normal);
                    axis = Vector3.Cross(hit.normal, axis);
                    break;
            }

            this.transform.rotation = Quaternion.LookRotation(axis);
            this.transform.position = hit.point;
        }
    }
}

public enum OrientType
{
    Normal,
    CrossRight,
    CrossForward
}