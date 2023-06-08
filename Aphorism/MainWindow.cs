using Aphorism.Aphorism;
using Aphorism.ScheTask;
using Aphorism.Service;
using NetDimension.NanUI;
using NetDimension.NanUI.Browser;
using NetDimension.NanUI.HostWindow;
using NetDimension.NanUI.Resource.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphorism;

internal class MainWindow : Formium
{
    public override string StartUrl => "http://static.app.local/index.html";

    public override HostWindowType WindowType => HostWindowType.Borderless;

    // 执行js
    public delegate void JSFunc(string text);
    public JSFunc jsFunc;
    // 显示信息
    private ScheTaskEntity.MyTask showInfo;
    // 保留一个任务对象以便关闭
    private ScheTaskHelper scheTask;
    Task scheTaskLoop;

    public MainWindow() 
    {
        MinimumSize = new System.Drawing.Size(10, 5);
        // 设置大小
        Size = new System.Drawing.Size(60, 60);
        // 是否修改大小
        Sizable = true;
        // 置顶窗口
        TopMost = true;

        // 关闭启动画面
        EnableSplashScreen = false;

        // 不在任务栏显示
        ShowInTaskBar = false;

        // 关闭前
        BeforeClose += MainWindow_BeforeClose;

        // 按键事件添加
        KeyEvent += MainWindow_KeyEvent;
    }

    private void MainWindow_BeforeClose(object? sender, FormiumCloseEventArgs e)
    {
        // 之前开的线程要记得结束和关闭
        scheTask.Stop();
        Thread.Sleep(1100);
#if DEBUG
        if (!scheTaskLoop.IsCanceled)
            MessageBox.Show("关闭失败");
#endif

    }

    private void MainWindow_KeyEvent(object? sender, NetDimension.NanUI.Browser.KeyEventArgs e)
    {
        switch (e.KeyEvent.WindowsKeyCode)
        {
            // F12 打开调试窗口
            case 123:
                // 必须放这里，在这里ShowDevTools才有实例，否则报空指针
                ShowDevTools();
                break;
            // esc关闭窗口
            case 27:
                this.Close(true);
                break;
            default: break;
        }
    }

    protected override void OnReady()
    {
        // 浏览器相关操作

#if DEBUG
        
#endif
        this.LoadEnd += MainWindow_LoadEnd;
    }

    private void MainWindow_LoadEnd(object? sender, LoadEndEventArgs e)
    {
        // 执行js的函数的委托
        jsFunc = this.ExecuteJavaScript;
        // showinfo函数的委托
        showInfo = () =>
        {
            // 每次都是一个新的名言
            string text = GetAphorism.GetInstance.GetNextAphorism();
            text = text.Replace("\r", "");
            string res = $"mainpage.show_aphorism(\"{text}\");";
            jsFunc(res);
        };

        showInfo();

        // 开启一个新线程执行任务
        scheTaskLoop = Task.Factory.StartNew(() =>
        {
            scheTask = ScheTaskHelper.Instance;
            // 委托传递过去
            scheTask._JSFunc = jsFunc;
            scheTask.AddTask(new ScheTaskEntity(1800, true, showInfo));
            scheTask.Start();
        });
    }
}