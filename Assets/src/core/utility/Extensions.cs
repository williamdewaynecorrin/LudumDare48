using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Extensions 
{
    // -- string
    public static bool IsEmailAddress(this string s)
    {
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(s);

        return match.Success;
    }

    // -- material
    public static void SetBool(this Material m, string property, bool value)
    {
        int ivalue = value ? 1 : 0;
        m.SetInt(property, ivalue);
    }

    // -- vector 3
    public static float Product(this Vector3 v)
    {
        return v.x * v.y * v.z;
    }

    public static float Sum(this Vector3 v)
    {
        return v.x + v.y + v.z;
    }

    public static Vector3 Inverse(this Vector3 v)
    {
        return new Vector3(1f / v.x, 1f / v.y, 1f / v.z);
    }

    public static Vector3 ClampComponents(this Vector3 v, float compmax)
    {
        if (v.x < -compmax)
            v.x = -compmax;
        else if (v.x > compmax)
            v.x = compmax;

        if (v.y < -compmax)
            v.y = -compmax;
        else if (v.y > compmax)
            v.y = compmax;

        if (v.z < -compmax)
            v.z = -compmax;
        else if (v.z > compmax)
            v.z = compmax;

        return v;
    }

    public static Vector3 NoY(this Vector3 v)
    {
        return new Vector3(v.x, 0f, v.z);
    }

    public static Vector3 SetX(this Vector3 v, float x)
    {
        return new Vector3(x, v.y, v.z);
    }

    public static Vector3 SetY(this Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 SetZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }

    public static Vector3 XOnly(this Vector3 v)
    {
        return new Vector3(v.x, 0f, 0f);
    }

    public static Vector3 YOnly(this Vector3 v)
    {
        return new Vector3(0f, v.y, 0f);
    }

    public static Vector3 ZOnly(this Vector3 v)
    {
        return new Vector3(0f, 0f, v.z);
    }

    public static Vector3Int ToVector3IntRound(this Vector3 v)
    {
        return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
    }

    public static Vector3Int ToVector3IntRound(this Vector3 v, RoundType yround)
    {
        int vy = Mathf.RoundToInt(v.y);
        if (yround == RoundType.Ceil)
            vy = Mathf.CeilToInt(v.y);
        else if (yround == RoundType.Floor)
            vy = Mathf.FloorToInt(v.y);

        return new Vector3Int(Mathf.RoundToInt(v.x), vy, Mathf.RoundToInt(v.z));
    }

    public static Vector3Int ToVector3IntRound(this Vector3 v, RoundType xround, RoundType yround, RoundType zround)
    {
        // -- x
        int vx = Mathf.RoundToInt(v.x);
        if (xround == RoundType.Ceil)
            vx = Mathf.CeilToInt(v.x);
        else if (xround == RoundType.Floor)
            vx = Mathf.FloorToInt(v.x);

        // -- y
        int vy = Mathf.RoundToInt(v.y);
        if (yround == RoundType.Ceil)
            vy = Mathf.CeilToInt(v.y);
        else if (yround == RoundType.Floor)
            vy = Mathf.FloorToInt(v.y);

        // -- z
        int vz = Mathf.RoundToInt(v.z);
        if (zround == RoundType.Ceil)
            vz = Mathf.CeilToInt(v.z);
        else if (zround == RoundType.Floor)
            vz = Mathf.FloorToInt(v.z);

        return new Vector3Int(vx, vy, vz);
    }

    // -- vector4
    public static Vector3 ToVector3(this Vector4 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

    // -- vector3int
    public static int Sum(this Vector3Int v)
    {
        return v.x + v.y + v.z;
    }

    public static int AbsSum(this Vector3Int v)
    {
        return Mathf.Abs(v.x) + Mathf.Abs(v.y) + Mathf.Abs(v.z);
    }

    public static int Product(this Vector3Int v)
    {
        return v.x * v.y * v.z;
    }

    public static Vector3 ToVector3(this Vector3Int v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

    public static Vector3 UniformScaleF(this Vector3Int v, float scale)
    {
        Vector3 v3 = new Vector3(v.x, v.y, v.z);
        return v3 * scale;
    }

    public static Vector3Int Divide(this Vector3Int v, Vector3Int other)
    {
        return new Vector3Int(v.x / other.x, v.y / other.y, v.z / other.z);
    }

    public static Vector3Int DivideF(this Vector3Int v, Vector3Int other)
    {
        return new Vector3Int(Mathf.CeilToInt(v.x / (float)other.x),
                              Mathf.CeilToInt(v.y / (float)other.y),
                              Mathf.CeilToInt(v.z / (float)other.z));
    }

    public static int[] Arr(this Vector3Int v)
    {
        return new int[] { (int)v.x, (int)v.y, (int)v.z };
    }

    public static float[] ArrF(this Vector3Int v)
    {
        return new float[] { (float)v.x, (float)v.y, (float)v.z };
    }

    public static Quaternion ToQuaternion(this Vector4 v)
    {
        return new Quaternion(v.x, v.y, v.z, v.w);
    }

    // -- quat
    public static Quaternion ClampRotation(this Quaternion q, Vector3 bounds)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, -bounds.x, bounds.x);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
        angleY = Mathf.Clamp(angleY, -bounds.y, bounds.y);
        q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

        float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
        angleZ = Mathf.Clamp(angleZ, -bounds.z, bounds.z);
        q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

        return q;
    }

    public static float QuaternionAngleX(this Quaternion q)
    {
        q.x /= q.w;
        return 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
    }

    public static Quaternion Inverse(this Quaternion q)
    {
        return Quaternion.Inverse(q);
    }

    public static Vector4 ToVector4(this Quaternion q)
    {
        return new Vector4(q.x, q.y, q.z, q.w);
    }

    // -- transform
    public static Vector3 GetScaledDirections(this Transform t, Vector3 v)
    {
        return t.right * v.x + t.up * v.y + t.forward * v.z;
    }

    public static void MultiOp_ReOrientTransform(this StoredTransform[] tarr, TransformOrientation orient)
    {
        for(int i = 0; i < tarr.Length; ++i)
        {
            tarr[i].ReOrientTransform(orient);
        }
    }

    // -- color
    public static Color RGBScale(this Color c, float scale)
    {
        return new Color(c.r * scale, c.g * scale, c.b * scale, c.a);
    }

    public static Color SetAlpha(this Color c, float alpha)
    {
        c.a = alpha;
        return c;
    }

    // -- capsule
    private static float ccradiusadjust = 0.0025f;
    public static Vector3 BottomPoint(this CapsuleCollider cc, Vector3 tpos)
    {
        return tpos + cc.center - (Vector3.up * cc.height * 0.5f) + (Vector3.up * (cc.radius + ccradiusadjust));
    }

    public static Vector3 TopPoint(this CapsuleCollider cc, Vector3 tpos)
    {
        return tpos + cc.center + (Vector3.up * cc.height * 0.5f) - (Vector3.up * (cc.radius + ccradiusadjust));
    }

    public static float AdjustedRadius(this CapsuleCollider cc)
    {
        return cc.radius + ccradiusadjust;
    }

    public static float AdjustedRadiusFactor(this CapsuleCollider cc)
    {
        return 1.0f + ccradiusadjust;
    }

    public static Rect Indent(this Rect r, float xindent)
    {
        return new Rect(r.x + xindent, r.y, r.width, r.height);
    }

    public static void LogPos(this float[] arr, int sizex, int sizey)
    {
        int hd = sizex * sizey;
        for(int i = 0; i < arr.Length; ++i)
        {
            int x = i / hd;
            int y = (i / sizex) - ((i / hd) * sizey);
            int z = i % sizex;

            Vector3Int index = new Vector3Int(x, y, z);
            Debug.Log("Pos: " + index + ", Val:" + arr[i]);
        }
    }

    public static int IndexFrom3D(Vector3Int coord, int sizex, int sizey)
    {
        int a = sizex * sizey;
        int b = sizex;
        int c = 1;
        int d = 0;

        return a * coord.x + b * coord.y + c * coord.z + d;
    }
}

public static class Vector3IntXT
{
    public static Vector3Int forward = new Vector3Int(0, 0, 1);
    public static Vector3Int back = new Vector3Int(0, 0, -1);
}

public static class MathXT
{
    public static int Pow2(int pow)
    {
        if (pow == 0)
            return 1;

        int val = 2;
        for(int i = 1; i < pow; ++i)
        {
            val *= 2;
        }

        return val;
    }

    public static Vector3 Abs(Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.x));
    }
}

public static class RandomXT
{
    public static Vector2 RandomVector2(Vector2 min, Vector2 max)
    {
        return new Vector2(Random.Range(min.x, max.x),
                           Random.Range(min.y, max.y));
    }

    public static Vector3 RandomVector3(Vector3 min, Vector3 max)
    {
        return new Vector3(Random.Range(min.x, max.x),
                           Random.Range(min.y, max.y),
                           Random.Range(min.z, max.z));
    }

    public static Vector3 RandomVector3Signed(Vector3 minunsigned, Vector3 maxunsigned)
    {
        Vector3 positive = new Vector3(Random.Range(minunsigned.x, maxunsigned.x),
                                       Random.Range(minunsigned.y, maxunsigned.y),
                                       Random.Range(minunsigned.z, maxunsigned.z));

        Vector3 negative = new Vector3(Random.Range(-maxunsigned.x, -minunsigned.x),
                                       Random.Range(-maxunsigned.y, -minunsigned.y),
                                       Random.Range(-maxunsigned.z, -minunsigned.z));

        return RandomVector3(negative, positive);
    }

    public static Vector3 RandomUnitVector3()
    {
        return new Vector3(Random.Range(-1f, 1f),
                           Random.Range(-1f, 1f),
                           Random.Range(-1f, 1f)).normalized;
    }

    public static void SetMin(this Vector3 v, float negcompmin, float poscompmin)
    {
        if (v.x < poscompmin && v.x >= 0f)
            v.x = poscompmin;
        else if (v.x > negcompmin && v.x < 0f)
            v.x = negcompmin;

        if (v.y < poscompmin && v.y >= 0f)
            v.y = poscompmin;
        else if (v.y > negcompmin && v.y < 0f)
            v.y = negcompmin;

        if (v.z < poscompmin && v.z >= 0f)
            v.z = poscompmin;
        else if (v.z > negcompmin && v.z < 0f)
            v.z = negcompmin;
    }


    public static bool RandomBool()
    {
        return Random.Range(0f, 1.0f) > 0.5f;
    }

    public static Color RandomColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1.0f);
    }

    public static Color RandomColor(Color min, Color max)
    {
        Color c = new Color();
        c.r = Random.Range(min.r, max.r);
        c.g = Random.Range(min.g, max.g);
        c.b = Random.Range(min.b, max.b);
        c.a = Random.Range(min.a, max.a);

        return c;
    }

    public static Gradient RandomGradient(Color[] colors)
    {
        GradientAlphaKey[] alphakeys = new GradientAlphaKey[colors.Length];
        GradientColorKey[] colorkeys = new GradientColorKey[colors.Length];

        float time = 0f;
        float increment = 1f / (float)colors.Length;
        for (int i = 0; i < colors.Length; ++i)
        {
            alphakeys[i] = new GradientAlphaKey(colors[i].a, time);
            colorkeys[i] = new GradientColorKey(colors[i], time);
            time += increment;
        }

        //float biomeindex = 0;
        //.......//
        //biomeindex *= (1 - weight);
        //biomeindex += i * weight;

        Gradient gradient = new Gradient();
        gradient.alphaKeys = alphakeys;
        gradient.colorKeys = colorkeys;

        return gradient;
    }
}

public static class PhysicsXT
{
    public static bool CapsuleCast(Transform transform, CapsuleCollider collider, float radius, Vector3 direction, out RaycastHit hit,
                                   float distance, LayerMask mask, QueryTriggerInteraction interaction)
    {
        Vector3 h2 = (Vector3.up * collider.height * 0.5f);
        Vector3 r = (Vector3.up * (radius + 0.0025f));

        Vector3 p1 = collider.BottomPoint(transform.position);
        Vector3 p2 = collider.TopPoint(transform.position);
        return Physics.CapsuleCast(p1, p2, radius, direction.normalized, out hit, distance, mask, interaction);
    }

    public static RaycastHit[] CapsuleCastAll(Transform transform, CapsuleCollider collider, float radius, Vector3 direction,
                                              float distance, LayerMask mask, QueryTriggerInteraction interaction)
    {
        Vector3 h2 = (Vector3.up * collider.height * 0.5f);
        Vector3 r = (Vector3.up * (radius + 0.0025f));

        Vector3 p1 = collider.BottomPoint(transform.position);
        Vector3 p2 = collider.TopPoint(transform.position);

        return Physics.CapsuleCastAll(p1, p2, radius, direction.normalized, distance, mask, interaction);
    }
}

public static class InputXT
{
    public static int NumpadInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
            return 0;
        else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            return 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            return 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            return 3;
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            return 4;
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
            return 5;
        else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
            return 6;
        else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
            return 7;
        else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
            return 8;
        else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
            return 9;

        return -1;
    }
}

public static class DebugXT
{
    // -- logging messages
    public static void LogMessage(string log)
    {
        Debug.Log(log);
    }

    public static void LogMessage(string log, object arg1)
    {
        Debug.Log(string.Format(log, arg1));
    }

    public static void LogMessage(string log, object arg1, object arg2)
    {
        Debug.Log(string.Format(log, arg1, arg2));
    }

    public static void LogMessage(string log, object arg1, object arg2, object arg3)
    {
        Debug.Log(string.Format(log, arg1, arg2, arg3));
    }

    public static void LogMessage(string log, object arg1, object arg2, object arg3, object arg4)
    {
        Debug.Log(string.Format(log, arg1, arg2, arg3, arg4));
    }

    public static void LogMessage(string log, object arg1, object arg2, object arg3, object arg4, object arg5)
    {
        Debug.Log(string.Format(log, arg1, arg2, arg3, arg4, arg5));
    }

    // -- logging warnings
    public static void LogWarning(string log)
    {
        Debug.LogWarning(log);
    }

    public static void LogWarning(string log, object arg1)
    {
        Debug.LogWarning(string.Format(log, arg1));
    }

    public static void LogWarning(string log, object arg1, object arg2)
    {
        Debug.LogWarning(string.Format(log, arg1, arg2));
    }

    public static void LogWarning(string log, object arg1, object arg2, object arg3)
    {
        Debug.LogWarning(string.Format(log, arg1, arg2, arg3));
    }

    public static void LogWarning(string log, object arg1, object arg2, object arg3, object arg4)
    {
        Debug.LogWarning(string.Format(log, arg1, arg2, arg3, arg4));
    }

    public static void LogWarning(string log, object arg1, object arg2, object arg3, object arg4, object arg5)
    {
        Debug.LogWarning(string.Format(log, arg1, arg2, arg3, arg4, arg5));
    }

    // -- logging errors
    public static void LogError(string log)
    {
        Debug.LogError(log);
    }

    public static void LogError(string log, object arg1)
    {
        Debug.LogError(string.Format(log, arg1));
    }

    public static void LogError(string log, object arg1, object arg2)
    {
        Debug.LogError(string.Format(log, arg1, arg2));
    }

    public static void LogError(string log, object arg1, object arg2, object arg3)
    {
        Debug.LogError(string.Format(log, arg1, arg2, arg3));
    }

    public static void LogError(string log, object arg1, object arg2, object arg3, object arg4)
    {
        Debug.LogError(string.Format(log, arg1, arg2, arg3, arg4));
    }

    public static void LogError(string log, object arg1, object arg2, object arg3, object arg4, object arg5)
    {
        Debug.LogError(string.Format(log, arg1, arg2, arg3, arg4, arg5));
    }
}

public enum RoundType
{
    Round,
    Floor,
    Ceil
}