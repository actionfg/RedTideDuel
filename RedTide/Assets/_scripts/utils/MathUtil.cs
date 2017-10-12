using System;
using Random = UnityEngine.Random;

public class MathUtil
{
    // 产生一个符合正态分布的随机数
    public static float NextNormalValue()
    {
        float U, u, v, S;

        do
        {
            u = 2.0f * Random.value - 1.0f;
            v = 2.0f * Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0);

        float fac = (float) Math.Sqrt(-2.0 * Math.Log(S) / S);
        return u * fac;
    }

    public static float NextNormalValue(float mean, float stdDeviation)
    {
        return mean + NextNormalValue() * stdDeviation;
    }
}