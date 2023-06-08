using Aphorism.Aphorism;
using Aphorism.ScheTask;
using NetDimension.NanUI.Browser;
using NetDimension.NanUI.Browser.ResourceHandler;
using NetDimension.NanUI.Resource.Data;
namespace Aphorism.Service;

public class TaskInfo 
{
    public string Type { get; set; }

    public int Time { get; set; }

    public int Loop { get; set; }

    public string Text { get; set; }
}

public class ResultInfo
{
    public string result { get; set; }
}

[DataRoute("/task")]
internal class TaskService : DataService
{
    private readonly ScheTaskHelper _ScheTaskHelper;

    // 显示信息
    ScheTaskEntity.MyTask showInfo;

    public TaskService() 
    {
        _ScheTaskHelper = ScheTaskHelper.Instance;
    }

    [RoutePost("addtask")]
    public ResourceResponse AddTask(ResourceRequest request)
    {
        TaskInfo taskInfo = request.DeserializeObjectFromJson<TaskInfo>();

        if(taskInfo.Time < 0)
        {
            return Json(new ResultInfo { result="时间不正确，请重新设置时间！"});
        }
        // 确定委托
        showInfo = () =>
        {
            string res = $"mainpage.show_taskInfo(\"{taskInfo.Text}\");";
            _ScheTaskHelper._JSFunc(res);
        };
        // 添加任务
        _ScheTaskHelper.AddTask(new ScheTaskEntity(taskInfo.Time, taskInfo.Loop == 1, showInfo));
        return Json(new ResultInfo { result = "success"});
    }
}
