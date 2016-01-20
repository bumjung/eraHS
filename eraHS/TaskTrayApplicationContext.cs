using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using eraHS.LogReader;
using eraHS.Utility;

namespace eraHS
{
    public class TaskTrayApplicationContext : ApplicationContext
    {
        NotifyIcon notifyIcon = new NotifyIcon();
        Configuration configWindow = new Configuration();

        public TaskTrayApplicationContext()
        {
            Console.WriteLine("Starting app");
            this.init();
            LogManager logManager = new LogManager();
            ConfigManager configManager = new ConfigManager();

            var logThread = new Thread(() => {
                configManager.init();
                logManager.start();
            });
            logThread.Start();

            var testThread = new Thread(() =>
            {
                Random rnd = new Random();

                string[] testLines = System.IO.File.ReadAllLines(Config.userFilePath + @"\testing.txt");

                int i = 0;
                int random = rnd.Next(20, 101);
                while (i < testLines.Length)
                {
                    if (random-- < 0)
                    {
                        random = rnd.Next(100, 1001);
                        Thread.Sleep(1000);
                    }
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(Config.userFilePath + @"\Power_1.log", true))
                    {
                        file.WriteLine(testLines[i]);

                    }
                    i++;
                }
            });
            testThread.Start();

            var lsTestThread = new Thread(() =>
            {
                Random rnd = new Random();

                string[] testLines = System.IO.File.ReadAllLines(Config.userFilePath + @"\lsTest.txt");

                int i = 0;
                int random = rnd.Next(1, 5);
                while (i < testLines.Length)
                {
                    if (random-- < 0)
                    {
                        random = rnd.Next(1, 5);
                        Thread.Sleep(1000);
                    }
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(Config.userFilePath + @"\LoadingScreen.log", true))
                    {
                        file.WriteLine(testLines[i]);
                    }
                    i++;
                }
            });
            lsTestThread.Start();
        }

        void init()
        {
            MenuItem configMenuItem = new MenuItem("Configuration", new EventHandler(ShowConfig));
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));

            notifyIcon.Icon = eraHS.Properties.Resources.AppIcon;
            notifyIcon.DoubleClick += new EventHandler(ShowMessage);
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { configMenuItem, exitMenuItem });
            notifyIcon.Visible = true;
        }

        void ShowMessage(object sender, EventArgs e)
        {
            // Only show the message if the settings say we can.
            if (eraHS.Properties.Settings.Default.ShowMessage)
                MessageBox.Show("Hello World");
        }

        void ShowConfig(object sender, EventArgs e)
        {
            // If we are already showing the window meerly focus it.
            if (configWindow.Visible)
                configWindow.Focus();
            else
                configWindow.ShowDialog();
        }

        void Exit(object sender, EventArgs e)
        {
            // We must manually tidy up and remove the icon before we exit.
            // Otherwise it will be left behind until the user mouses over.
            notifyIcon.Visible = false;

            Application.Exit();
        }
    }
}
