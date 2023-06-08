
namespace Aphorism.ScheTask;


/// <summary>
///  定时任务的具体内容
/// </summary>
internal class ScheTaskEntity
{
    public delegate void MyTask();
    private readonly MyTask mytask;
    public DateTime NextRunTime
    {
        get;
        private set;
    }

    public string ShowInfo
    {
        get;
        private set;
    }

    public bool IsRepeat { get; private set; }


    int InitSeconds { get; set; }


    /// <summary>
    /// 初始化获得一个内容实体
    /// </summary>
    /// <param name="seconds">秒数，多少秒后执行</param>
    /// <param name="showInfo">显示的信息</param>
    public ScheTaskEntity(int seconds, string showInfo, bool isRepeat, MyTask task)
    {
        InitSeconds = seconds;
        InitTime();
        ShowInfo = showInfo;
        IsRepeat = isRepeat;
        mytask = task;
    }

    public ScheTaskEntity(int seconds, bool isRepeat, MyTask task)
    {
        InitSeconds = seconds;
        InitTime();
        ShowInfo = null;
        IsRepeat = isRepeat;
        mytask = task;
    }

    /// <summary>
    /// 重置剩余时间，恢复到初始化时的时间
    /// </summary>
    public void InitTime()
    {
        NextRunTime = DateTime.Now.AddSeconds(InitSeconds);
    }

    /// <summary>
    /// 实际任务，为了迎合线程池写法，需要加参数
    /// </summary>
    /// <param name="obj"></param>
    public void DoTask(object obj)
    {
        Console.WriteLine(ShowInfo + $" : 设定时间 {NextRunTime} 当前时间 {DateTime.Now}");
    }

    public void DoTask()
    {
        //Console.WriteLine(ShowInfo + $" : 设定时间 {NextRunTime} 当前时间 {DateTime.Now}");
        mytask();
    }
}

