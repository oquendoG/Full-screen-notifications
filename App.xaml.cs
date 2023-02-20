using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Notificaction;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected string _routeTimeFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\FullScreenTime.txt";
    protected int _time;
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        ReadTimeFile();

        // Crea un temporizador de cierto tiempo
        var timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(_time);
        timer.Tick += Timer_Tick;
        timer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        // Cierra la aplicación
        Current.Shutdown();
    }

    private void ReadTimeFile()
    {
        //leemos el archivo de FullScreenTime.txt
        try
        {
            using StreamReader reader = new(_routeTimeFile);
            _time = int.Parse(reader.ReadLine().Trim());

        }
        catch (Exception e)
        {
            MessageBox.Show("El archivo de tiempo no se pudo leer, compruebe permisos");
            Console.WriteLine(e.Message);
        }
    }
}
