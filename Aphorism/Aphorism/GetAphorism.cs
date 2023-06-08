using Aphorism.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vanara.Extensions;

namespace Aphorism.Aphorism;
internal class GetAphorism
{
    // 单例
    private static GetAphorism instance;
    private static object sLockObj = new object();

    public static GetAphorism GetInstance
    {
        get
        {
            if (instance == null)
            {
                lock (sLockObj)
                {
                    if (instance == null)
                    {
                        instance = new GetAphorism();
                    }
                }
            }
            return instance;
        }
    }

    private object LockObj = new object();
    private IList<string> aphorisms;

    private GetAphorism() 
    {
        this.aphorisms = GetAphorims();
    }

    public string GetNextAphorism()
    {
        if(this.aphorisms.Count == 0 || this.aphorisms is null) 
        {
            this.aphorisms = GetAphorims();
        }
        string res = this.aphorisms[CDJRandom.GetRandomInt(0, this.aphorisms.Count - 1)];
        this.aphorisms.Remove(res);
        return res;
    }

    public IList<string> GetAphorims()
    {
        lock (LockObj)
        {
            return GetAphorismsFileStream();
        }
    }

    /// <summary>
    /// 使用FileStream实现的名言读取
    /// </summary>
    /// <returns></returns>
    private IList<string> GetAphorismsFileStream()
    {
        List<string> res = new List<string>();

        StringBuilder sb = new StringBuilder();

        // 先获得所有的名言内容
        using (FileStream fs = File.OpenRead(System.AppDomain.CurrentDomain.BaseDirectory + @"\Web\aphorisms.txt"))
        {
            byte[] bytes = new byte[2048];
            UTF8Encoding encode = new UTF8Encoding();
            while (fs.Read(bytes, 0, bytes.Length) > 0)
            {
                sb.Append(encode.GetString(bytes));
            }
        }

        string alpo = sb.ToString();
        sb.Clear();

        // 变成List
        foreach (char x in alpo)
        {
            if (x == '\n')
            {
                res.Add(sb.ToString());
                sb.Clear();
                continue;
            }
            sb.Append(x);
        }
        return res;
    }
}

