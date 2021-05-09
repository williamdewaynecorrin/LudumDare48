using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhysicsManager
{
    private static float gravityforce = 0.5f;
    private static Vector3 gravitydir = Vector3.down;

    public static float killheight = -250f;

    public static Vector3 Gravity()
    {
        return gravitydir * gravityforce;
    }

    public static Vector3 GravityDir()
    {
        return gravitydir.normalized;
    }

    public static Vector3 NormalDir()
    {
        return -gravitydir.normalized;
    }

    public static void FlipGravity(Vector3 newdir, float newforce)
    {
        if(newdir != gravitydir)
            gravitydir = newdir;
        if (!Mathf.Approximately(newforce, gravityforce))
            gravityforce = newforce;
    }

    public static Vector3 DefaultGravityDir()
    {
        return Vector3.down;
    }
}
