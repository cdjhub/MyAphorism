using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphorism.Tools;

public class CDJRandom
{
    static Random rd = new Random();
    static string RandomStr = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    /// <summary>
    ///  随机获得一个int，注意可能为负数
    /// </summary>
    /// <returns></returns>
    public static int GetRandomInt()
    {
        int x = DateTime.Now.Millisecond * rd.Next();
        x ^= x << 13;
        x ^= x >> 17;
        x ^= x << 5;
        rd = new Random(x);
        return x;
    }

    /// <summary>
    /// 获得0到maxInt-1的一个随机数
    /// </summary>
    /// <param name="maxInt">不包含maxInt，开区间</param>
    /// <returns></returns>
    public static int GetRandomInt(int maxInt)
    {
        int t = GetRandomInt();
        if (t < 0)
            t = -t;
        return t % maxInt;
    }

    /// <summary>
    /// 获得l到r的一个随机数，闭区间
    /// </summary>
    /// <param name="l">左端点</param>
    /// <param name="r">右端点</param>
    /// <returns></returns>
    public static int GetRandomInt(int l, int r)
    {
        // l确定左端点，r-l+1确定长度
        return l + GetRandomInt(r - l + 1);
    }


    public static string GetRandomString(int length)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            sb.Append(RandomStr[GetRandomInt(62)]);
        }
        return sb.ToString();
    }
}