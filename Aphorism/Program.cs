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
        // ����������
        WinFormium.CreateRuntimeBuilder( env => {
            env.CustomCefSettings(settings => { 
                // ָ��CEF��ز���
            });

            env.CustomCefCommandLineArguments(commandLine => {
                // ָ��CEF�����в���
            });
        }, app => {
            MainForm = mainWindow;
            // ָ����������
            app.UseMainWindow(content => MainForm);
            // ��������ҳӳ�����ҳ
            app.UseLocalFileResource("http", "static.app.local", System.AppDomain.CurrentDomain.BaseDirectory + @"\Web");
            //app.UseEmbeddedFileResource("http", "static.app.local", "Web");
            // ע��Service
            app.UseDataServiceResource("http", "api.app.local");
        })
        .Build()
        .Run();

    }
}
