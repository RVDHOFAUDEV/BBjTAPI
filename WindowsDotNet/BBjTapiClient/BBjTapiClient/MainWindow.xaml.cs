using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Timers;
using System.Threading;
using JulMar.Atapi;
using System.Reflection;

namespace BBjTapiClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private System.Timers.Timer timer;
        private bool virgin = false;
        /* tray icon stuff */
        private bool cancelShutDown = true;
        public NotifyIcon notifyIcon = null;
        private System.Windows.Forms.ContextMenuStrip contextMenu1Strip;
        private ToolStripItem menuItem1Strip;
        private ToolStripItem menuItem2Strip;

        public MainWindow()
        {
            InitializeComponent();
            hideApplication();
            App.mainWin = this;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Application_ThreadException);
            initializeAsTrayIconApplication();
            setMainWindowTitle();
            App.displayPage("binding"); // main settings
            App.network.initialize(); // method code continues before call is completed - is okay here
            App.isPreparationPhase = false; //Preparation done - user interaction allowed
            if (App.Setup.Extension == "" || App.Setup.Line=="")
            {
                showApplication(); // only display the app on the desktop if the setup is incomplete
                virgin = true;
            }
            startTimer();
        }

        #region trayIconApplicationHandling
        /* initialize the tray icon application style */
        private void initializeAsTrayIconApplication()
        {
            /** init context menu and tray icon */
            this.contextMenu1Strip = new System.Windows.Forms.ContextMenuStrip();

            menuItem1Strip = contextMenu1Strip.Items.Add("show");
            menuItem1Strip.Click += MenuItem1Strip_Click;
            menuItem1Strip.Image = Properties.Resources.show;

            menuItem2Strip = contextMenu1Strip.Items.Add("exit");
            menuItem2Strip.Click += MenuItem2Strip_Click;
            menuItem2Strip.Image = Properties.Resources.exit;

            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            notifyIcon.Icon = Properties.Resources.BBjTAPIClient;
            notifyIcon.Visible = true;

            notifyIcon.ContextMenuStrip = contextMenu1Strip;
        }


        /* show application by doubleclick */
        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            showApplication();
        }


        /** exit context clicked */
        private void MenuItem2Strip_Click(object sender, EventArgs e)
        {
            cancelShutDown = false;
            closeApplication();
        }


        /** show context clicked */
        private void MenuItem1Strip_Click(object sender, EventArgs e)
        {
            showApplication();
        }


        /* hide this */
        private void hideApplication()
        {
            this.ShowInTaskbar = false;
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        /* show this app window on the desktop */
        private void showApplication()
        {
            this.ShowInTaskbar = true;
            this.Visibility = System.Windows.Visibility.Visible;
        }


        /* sure to close this wonderful application? */
        private void mainWin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (cancelShutDown)
            {
                hideApplication();
                e.Cancel = cancelShutDown;
            }
        }
        

        /* triggered by close() - final cleanup activities */
        private void mainWin_Closed(object sender, EventArgs e)
        {
            App.isShuttingDown = true;
            stopTimer();
            App.tapi.Dispose();
            App.network.disconnect();
            /* tray icon */
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            /* free again and open for a restart */
            if (App.createdNewMutex)
            {
                try
                {
                    App.mutex.ReleaseMutex();
                }
                catch { }
            }
        }


        /* say good bye */
        public void closeApplication()
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                App.log(String.Format("Unable to close the application. {0}", ex.Message));
            }
        }
        #endregion

        #region miscellaneous 
        /* tray icon balloon tip */
        public void showBalloonTip(string text)
        {
            notifyIcon.BalloonTipText = text;
            notifyIcon.BalloonTipTitle = "BBjTAPIClient.Net";
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.ShowBalloonTip(1000);
        }


        /* catch TAPI exceptions */
        static void Application_ThreadException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            if (e is TapiException)
                System.Windows.Forms.MessageBox.Show(e.Message);
        }


        /* set the main window caption */
        private void setMainWindowTitle()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string version = assembly.GetName().Version.ToString();
            string title = App.aim + (IntPtr.Size == 4 ? " - x86" : " - x64") + " - V." + version;
            mainWin.Title = title;
        }


        private void mainWin_Loaded(object sender, RoutedEventArgs e)
        {
            App.tapi.init(); // get lines -- display lines
        }

        #endregion 

        #region timerManaging
        /* program heart beat timer configuration */
        private void startTimer()
        {
            /* arrange timer in order to connect unattended to the bbjtapi central server*/
            timer = new System.Timers.Timer(2000);
            timer.Elapsed += raiseTimer;
            timer.AutoReset = true;
            timer.Start();
        }


        private void stopTimer()
        {
            if (timer != null)
            {
                timer.Enabled = false;
                timer.Stop();
                timer.Close();
                timer.Dispose();
            }
        }


        /* check if BBjTapi was started in the time being / if the connection is available now */
        private void raiseTimer(object sender, ElapsedEventArgs e)
        {
            if (App.isPreparationPhase == false)
            {
                if (App.Setup.IsNetworkConnectionEstablished == false)
                {
                    App.network.disconnect();
                    App.network.initialize(); // async embedded - continues before initialize call is completed - is okay here 
                }
                if (App.isRefreshingTapiSession)
                {
                    App.tapi.stopSession();
                    App.tapi.startSession();
                    App.isRefreshingTapiSession = false;
                }
                /* this program may only one run once using this EXTENSION - avoid parallel processing of the same Extension */
                if (App.mutex == null && App.Setup.Extension != "")
                {
                    App.mutex = new System.Threading.Mutex(true, "BBjTAPIClient.Net.Extension" + App.Setup.Extension, out App.createdNewMutex);
                    if (!App.createdNewMutex)
                        Close();
                }
                /* only once */
                if (virgin)
                {
                    /* signalize fully connectivity */
                    if (App.mainWin.Visibility==Visibility.Visible && App.Setup.IsExtensionRegistered && App.Setup.IsTapiSessionConnected)
                    {
                        showBalloonTip("connected");
                        virgin = false;
                    }
                }
            }
        }
        #endregion

        #region page_management
        /* on 'Extras' page click */
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            btnBinding.Style = (Style)mainWin.FindResource("normalButton");
            btnExtras.Style = (Style)mainWin.FindResource("activeButton");
            App.displayPage("extras");
        }

        /* on 'Binding' page click */
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            btnBinding.Style = (Style)mainWin.FindResource("activeButton");
            btnExtras.Style = (Style)mainWin.FindResource("normalButton");
            App.displayPage("binding");
        }
        #endregion
    }
}
