using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphorism.ScheTask;


/// <summary>
/// 定时任务的优先队列，保证时间最小的排在最前面
/// 最小堆实现
/// </summary>
internal class ScheTaskQueue
{
    private List<ScheTaskEntity> Entities { get; set; }

    /// <summary>
    /// N 是指最后一个位置，并且0位是不存的
    /// </summary>
    private int N
    {
        get { return Entities.Count - 1;}
    }

    public ScheTaskQueue()
    {
        Entities = new List<ScheTaskEntity>(); 
        Entities.Add(new ScheTaskEntity(0, false, () => { }));
    }

    public ScheTaskEntity Top()
    { 
        lock(Entities)
        {
            return TopUnLock();
        }
    }

    private ScheTaskEntity TopUnLock()
    {
        if (N <= 0)
        {
            return null;
        }
        return Entities[1];
    }


    public ScheTaskEntity Pop()
    {
        lock(Entities )
        {
            return PopUnLock();
        }
    }

    private ScheTaskEntity PopUnLock()
    {
        if (N <= 0) 
        {
            return null;
        }
        ScheTaskEntity res = Entities[1];
        Remove(1);
        return res;
    }

    public void Add(ScheTaskEntity ent)
    {
        lock(Entities )
        {
            AddUnLock(ent);
        }
    }

    private void AddUnLock(ScheTaskEntity ent)
    {
        Entities.Add(ent);
        Up(N);
    }


    /// <summary>
    /// 从儿子找父亲
    /// </summary>
    /// <param name="idx">儿子索引</param>
    private void Up(int idx)
    {
        // 如果超出索引范围，就不行
        if(idx > N)
        {
            return;
        }

        // 比较是左边儿子，右边是父亲
        while ((idx >> 1) > 0 && Entities[idx].NextRunTime < Entities[idx >> 1].NextRunTime)
        {
            Swap(idx, idx >> 1);
            idx >>= 1;
        }
    }

    /// <summary>
    /// 从父亲找儿子
    /// </summary>
    /// <param name="idx">父亲索引</param>
    private void Down(int idx)
    {
        // t记录最小的索引
        int t = idx;

        // 检查左右两边儿子是不是都比父亲小
        // 比较是左边儿子，右边是父亲
        if ((idx << 1) <= N && Entities[idx << 1].NextRunTime < Entities[t].NextRunTime)
        {
            t = (idx << 1);
        }
        if((idx << 1 | 1) <= N && Entities[idx << 1 | 1].NextRunTime < Entities[t].NextRunTime)
        {
            t = (idx << 1 | 1);
        }
        if(t != idx)
        {
            Swap(t, idx);
            Down(t);
        }
    }


    /// <summary>
    /// 删除某个元素
    /// </summary>
    /// <param name="idx"></param>
    private void Remove(int idx)
    {
        if(idx > N || idx == 0)
        {
            return;
        }

        Swap(idx, N);
        Entities.RemoveAt(N);
        Down(idx);
    }


    /// <summary>
    /// 如果N == 0 说明只有0位，认为是空队列
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        lock(Entities) 
        {
            return N <= 0;
        }
    }

    private void Swap(int idxa, int idxb) 
    { 
        ScheTaskEntity t = Entities[idxa];
        Entities[idxa] = Entities[idxb];
        Entities[idxb] = t; 
    }
}

