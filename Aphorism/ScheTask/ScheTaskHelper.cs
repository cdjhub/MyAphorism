using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphorism.ScheTask;

internal class ScheTaskHelper
{
    // 单例
    private static ScheTaskHelper instance;
    private static object sLockObj = new object();
    public static ScheTaskHelper Instance
    {
        get 
        { 
            if(instance == null)
            {
                lock(sLockObj)
                {
                    if (instance == null)
                    {
                        instance = new ScheTaskHelper();
                    }
                }
            }
            return instance;
        }
    }

    // 记录一下执行JS的委托
    public MainWindow.JSFunc _JSFunc {get; set; }

    ScheTaskQueue queue;
    ScheTaskThread scheTaskThread;

    private ScheTaskHelper()
    {
        queue = new ScheTaskQueue();
        scheTaskThread = new ScheTaskThread(queue);
    }


    public void AddTask(ScheTaskEntity entity)
    {
        queue.Add(entity);
    }

    public void Start()
    {
        scheTaskThread.Start();
    }

    public void Stop()
    {
        scheTaskThread.stop();
    }
}
