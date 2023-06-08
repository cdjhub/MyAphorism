using Aphorism.Service;
using NetDimension.NanUI;
using NetDimension.NanUI.Resource.Data;

namespace Aphorism;

internal static class Program
{
    public static MainWindow MainForm;
    
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        MainWindow mainWindow = new MainWindow();
        // 创建主窗口
        WinFormium.CreateRuntimeBuilder( env => {
            env.CustomCefSettings(settings => { 
                // 指定CEF相关参数
            });

            env.CustomCefCommandLineArguments(commandLine => {
                // 指定CEF命令行参数
            });
        }, app => {
            MainForm = mainWindow;
            // 指定启动窗体
            app.UseMainWindow(content => MainForm);
            // 将本地网页映射成网页
            app.UseLocalFileResource("http", "static.app.local", System.AppDomain.CurrentDomain.BaseDirectory + @"\Web");
            //app.UseEmbeddedFileResource("http", "static.app.local", "Web");
            // 注册Service
            app.UseDataServiceResource("http", "api.app.local");
        })
        .Build()
        .Run();

    }
}
