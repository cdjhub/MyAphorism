using NetDimension.NanUI.Browser.ResourceHandler;
using NetDimension.NanUI.Resource.Data;
using System.Diagnostics;

namespace Aphorism.Service;

[DataRoute("/system")]
public class SystemInfoService : DataService
{
    //private static SystemInfoService instance;
    //private static object lockobj = new object();

    //// 双验单例
    //public static SystemInfoService GetInstance
    //{
    //    get
    //    {
    //        if (instance is null)
    //            lock (lockobj)
    //            {
    //                if (instance == null)
    //                    instance = new SystemInfoService();
    //            }
    //        return instance;
    //    }
    //}

    private readonly PerformanceCounter CpuOccupied;

    private readonly PerformanceCounter FreeMem;

    private readonly float TotalMem;

    public SystemInfoService() 
    {
        // 创建CPU占用信息对象
        //CpuOccupied = new PerformanceCounter[System.Environment.ProcessorCount];
        //CpuOccupied[i] = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        // 来源 ： https://www.codenong.com/35288853/
        CpuOccupied = new PerformanceCounter("Processor", "% Idle Time", "_Total");
        CpuOccupied.NextValue();
        
        // 创建内存信息对象
        FreeMem = new PerformanceCounter("Memory", "Available Bytes");
        FreeMem.NextValue();
        TotalMem = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
    }

    /// <summary>
    /// 醉了，返回类型和参数类型必须是这两个，否则路由不到
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [RouteGet("cpu")]
    // All : system/cpu
    public ResourceResponse GetCpuOccupied(ResourceRequest request) 
    {
        return Text(CpuOccupied.NextValue().ToString());
    }

    /// <summary>
    /// 醉了，返回类型和参数类型必须是这两个，否则路由不到
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [RouteGet("mem")]
    // All : system/mem
    public ResourceResponse GetMemOccupied(ResourceRequest request)
    {
        float freeMem = FreeMem.NextValue();
        float res = (1 - freeMem / TotalMem) * 100;
        return Text((res).ToString());
    }
}

