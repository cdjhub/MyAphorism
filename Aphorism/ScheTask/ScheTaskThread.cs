using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphorism.ScheTask;
internal class ScheTaskThread
{
    ScheTaskQueue Queue { get; set; }

    private readonly object lockobj = new object();
    TimeSpan TimeSpan1S = new TimeSpan(0, 0, 1);
    TimeSpan TimeSpan10S = new TimeSpan(0, 0, 10);
    TimeSpan TimeSpan30S = new TimeSpan(0, 0, 30);

    private bool Cycle = true;
 

    public ScheTaskThread(ScheTaskQueue queue)
    {
        Queue = queue;
    }

    public void Start()
    {
        // 创建AutoResetEvent对象和下次需要执行的时间
        AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        // 循环执行任务
        while (Cycle)
        {
            // 设置下一次需要执行的时间
            DateTime nextRunTime = Queue.Top().NextRunTime;

            // 计算下一次需要执行的时间间隔
            TimeSpan timeToWait = nextRunTime - DateTime.Now;

            // 如果下一次需要执行的时间没到，则等待指定时间后开始执行任务
            if (timeToWait > TimeSpan.Zero)
            {
                // !!! 如果是这个阻塞途中插入了任务，其结束时间也在这个阻塞时间内的，那么会同时完成,与预期不符合
                //autoResetEvent.WaitOne(timeToWait);

                // 所以需要进行定期检测 就看想要什么精度了，这里设置大概1s，也可以30s，但是1h搞3600次，不过分
                if(timeToWait > TimeSpan1S)
                {
                    // 阻塞线程，定时唤醒，因为中途可能会有新的任务添加进来，导致TimeToWait更改，所需要这么做
                    autoResetEvent.WaitOne(TimeSpan1S);
                    // 终止后续，继续循环
                    continue;
                }

                // 阻塞线程，定时唤醒
                autoResetEvent.WaitOne(timeToWait);
            }

            // 取出当前对象
            ScheTaskEntity CurrentEntity = Queue.Top();

            // 再次检查是否到规定时间
            if (CurrentEntity.NextRunTime > DateTime.Now)
                continue;
            Queue.Pop();
            // 提交任务到线程池中执行，，，不要这么做，会发现设定循环任务的时候，CurrentTime在DoTask前就被InitTIme
            // 主要原因应该是，当前线程在占用CPU片，当前任务结束后才到DoTask的线程
            //ThreadPool.QueueUserWorkItem(CurrentEntity.DoTask);
            CurrentEntity.DoTask();
            // 如果是重复任务对象
            if(CurrentEntity.IsRepeat)
            {
                CurrentEntity.InitTime();
                Queue.Add(CurrentEntity);
            }

            // 如果消费完了，为了维持线程消费继续，需要继续添加产品
            if(Queue.IsEmpty())
            {
                Queue.Add(new ScheTaskEntity(3600, false, () => { }));
            }

        }
    }

    public void stop()
    {
        Cycle = false;
    }
}

