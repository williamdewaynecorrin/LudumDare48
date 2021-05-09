using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomValue<T>
{
    [SerializeField]
    protected T min;
    [SerializeField]
    protected T max;
    [HideInInspector]
    public T lastpicked;

    public RandomValue(T min, T max)
    {
        this.min = min;
        this.max = max;
    }

    public virtual T PickRandomValue()
    {
        return min;
    }
}

[System.Serializable]
public class RandomFloat : RandomValue<float>
{
    public RandomFloat(float min, float max) : base(min, max)
    {
        this.min = min;
        this.max = max;
    }

    public override float PickRandomValue()
    {
        return lastpicked = Random.Range(min, max);
    }
}

[System.Serializable]
public class RandomVector2 : RandomValue<Vector2>
{
    public RandomVector2(Vector2 min, Vector2 max) : base(min, max)
    {
        this.min = min;
        this.max = max;
    }

    public override Vector2 PickRandomValue()
    {
        return lastpicked = RandomXT.RandomVector2(min, max);
    }
}

[System.Serializable]
public class RandomInt : RandomValue<int>
{
    public RandomInt(int min, int max) : base(min, max)
    {
        this.min = min;
        this.max = max;
    }

    public override int PickRandomValue()
    {
        return lastpicked = Random.Range(min, max + 1);
    }
}

[System.Serializable]
public class RandomColor : RandomValue<Color>
{
    public RandomColor(Color min, Color max) : base(min, max)
    {
        this.min = min;
        this.max = max;
    }

    public override Color PickRandomValue()
    {
        float r = Random.Range(min.r, max.r);
        float g = Random.Range(min.g, max.g);
        float b = Random.Range(min.b, max.b);
        float a = Random.Range(min.a, max.a);

        return lastpicked = new Color(r, g, b, a);
    }
}
