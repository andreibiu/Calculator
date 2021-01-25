using System;

namespace Calculator
{
    public class Application : System.Windows.Application
    {
        private static Application application = new Application();
        
        public Application()
        {
            MainWindow = new MainWindow();
            Startup += delegate { MainWindow.Show(); };
        }

        [STAThread]
        public static void Main()
        {
            Current.Run();
        }
    }
}
