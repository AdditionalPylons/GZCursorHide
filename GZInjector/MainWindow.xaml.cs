using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GZInjector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread backgroundThread;

        public MainWindow()
        {
            InitializeComponent();
            backgroundThread = new Thread(new ThreadStart(ThreadEntry));
            backgroundThread.Start();
        }

        void ThreadEntry()
        {
            bool res = false;

            do
            {
                Thread.Sleep(500);
                try
                {
                    res = HookInject.Hook();
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(delegate()
                    {
                        statusText.Content = "Error: " + ex.Message;
                    }
                    );
                }
                if (res)
                {
                    Dispatcher.Invoke(delegate()
                    {
                        statusText.Content = "Injected successfully";
                    });
                }
            }
            while (res == false);
        }
    }
}
