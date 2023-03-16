using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Notificaction;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private int _counter = 1;
    private int _maxImageNumber;
    string imageName;
    private string _userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\FullScreenNotificationsImg";
    protected string _routeFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\FullScreenSavings.txt";
    protected string _routeTimeFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\FullScreenTime.txt";
    protected string _routeSurvey = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\FullScreenSurvey.txt";


    public MainWindow()
    {
        InitializeComponent();
    }

    private async void MainImage_Loaded(object sender, RoutedEventArgs e)
    {
        await CreateFolder();
        await CreateFile();
        await CreateTimeFile();
        await CreateSurveyFile();
        await ReadSavingsFile();

        _maxImageNumber = ReadFiles(_userFolder);
        if (_maxImageNumber == 0)
        {
            MessageBox.Show("No hay imágenes que mostrar");
            return;
        }

        await ShowImage();
        await ShowSurvey();
    }

    /// <summary>
    /// Muestra la imagen en pantalla
    /// </summary>
    /// <returns>Una tarea completada</returns>
    private Task ShowImage()
    {
        imageName = string.Format($"{_counter}{DefineExtension(_userFolder)}");
        try
        {
            MainImage.Source = new BitmapImage(new Uri(@$"{_userFolder}\{imageName}", UriKind.RelativeOrAbsolute));
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ruta de imágen incorrecta");
            Console.WriteLine(ex.Message);
        }

        _counter++;

        if (_counter > _maxImageNumber)
            _counter = 1;
        try
        {
            File.WriteAllText(_routeFile, _counter.ToString());
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se puede acceder al sistema de archivos");
            Console.WriteLine(ex.Message);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// leemos todas las fotos del directorio especficado en la ruta
    /// </summary>
    /// <param name="ruta">Define la ruta de las fotos</param>
    /// <returns>La cantidad de archivos en la carpeta</returns>
    private int ReadFiles(string ruta)
    {
        string[] files = Directory.GetFiles(ruta);
        return files.Length;
    }

    /// <summary>
    /// Definimos la extrnsión de la foto
    /// </summary>
    /// <param name="ruta">Recesita la ruta para poder ver que extensión tiene</param>
    /// <returns>la extesión en formato .png o .jpg</returns>
    private string DefineExtension(string ruta)
    {
        List<string> files = (Directory.GetFiles(ruta)).ToList();
        string hasExtension = files[0];
        if (hasExtension.Contains(".jpg"))
        {
            return ".jpg";
        }

        return ".png";
    }

    /// <summary>
    /// Creamos la carpeta sino existe
    /// </summary>
    /// <returns></returns>
    private Task CreateFolder()
    {
        if (!Directory.Exists(_userFolder))
        {
            Directory.CreateDirectory(_userFolder);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Crea el archivo savings.txt
    /// </summary>
    /// <returns></returns>
    private Task CreateFile()
    {
        //Creamos el archivo de savings.txt
        if (!File.Exists(_routeFile))
        {
            using FileStream file = File.Create(_routeFile);
            byte[] miInfo = new UTF8Encoding(true).GetBytes("1");
            file.Write(miInfo, 0, miInfo.Length);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Crea el archivo FullScreenTime.txt
    /// </summary>
    /// <returns></returns>
    private Task CreateTimeFile()
    {
        if (!File.Exists(_routeTimeFile))
        {
            using FileStream file = File.Create(_routeTimeFile);
            byte[] miInfo = new UTF8Encoding(true).GetBytes("60");
            file.Write(miInfo, 0, miInfo.Length);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Crea el archivo FullScreenTime.txt
    /// </summary>
    /// <returns></returns>
    private Task CreateSurveyFile()
    {
        if (!File.Exists(_routeSurvey))
        {
            using FileStream file = File.Create(_routeSurvey);
            byte[] miInfo = new UTF8Encoding(true).GetBytes("www.google.com");
            file.Write(miInfo, 0, miInfo.Length);
        }

        return Task.CompletedTask;
    }

    private async Task ReadSavingsFile()
    {
        //leemos el archivo de Savings.txt
        try
        {
            using StreamReader reader = new(_routeFile);
            _counter = int.Parse((await reader.ReadLineAsync()).Trim());

        }
        catch (Exception e)
        {
            MessageBox.Show("El archivo no se pudo leer compruebe permisos");
            Console.WriteLine(e.Message);
        }

    }

    /// <summary>
    /// Evita que se cierre la app con alt + f4
    /// </summary>
    protected override void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;
        base.OnClosing(e);
    }

    /// <summary>
    /// Abre la encuensta de satisfacción
    /// </summary>
    private void btnEncuesta_Click(object sender, RoutedEventArgs e)
    {
        string url = ReadSurveyFiles();

        var ps = new ProcessStartInfo(url.Trim())
        {
            UseShellExecute = true,
            Verb = "open"
        };
        Process.Start(ps);
    }

    /// <summary>
    /// Lee el archivo que tiene la url de la encuesta
    /// </summary>
    /// <returns>Un string con la url de la encuesta</returns>
    private string ReadSurveyFiles()
    {
        using StreamReader reader = new(_routeSurvey);

        return reader.ReadLine();
    }

    /// <summary>
    /// Muestra el botón de encuensta en la imágen final
    /// </summary>
    private Task ShowSurvey()
    {
        if (imageName.Contains($"{_maxImageNumber}"))
        {
            btnBorder.Visibility = Visibility.Visible;
            btnEncuesta.Visibility = Visibility.Visible;
        }

        return Task.CompletedTask;
    }
}