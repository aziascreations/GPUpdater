using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GPHelper {

    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
            progressBar.Visibility = Visibility.Hidden;
        }

        private void LabelForceClick(object sender, MouseButtonEventArgs e) {
            toggleButtonOptionForce.IsChecked = !toggleButtonOptionForce.IsChecked;
        }

        private void LabelSyncClick(object sender, MouseButtonEventArgs e) {
            toggleButtonSync.IsChecked = !toggleButtonSync.IsChecked;
        }

        //Logoff switch functions
        private void LabelLogoffClick(object sender, MouseButtonEventArgs e) {
            bool IsBootEnabled = toggleButtonBoot.IsChecked.HasValue ? toggleButtonBoot.IsChecked.Value : false;

            if(IsBootEnabled)
                toggleButtonBoot.IsChecked = !toggleButtonBoot.IsChecked;

            toggleButtonLogoff.IsChecked = !toggleButtonLogoff.IsChecked;
        }

        private void ToggleLogoffChecked(object sender, RoutedEventArgs e) {
            bool IsBootEnabled = toggleButtonBoot.IsChecked.HasValue ? toggleButtonBoot.IsChecked.Value : false;

            if(IsBootEnabled)
                toggleButtonBoot.IsChecked = !toggleButtonBoot.IsChecked;
        }

        //Boot switch functions
        private void LabelBootClick(object sender, MouseButtonEventArgs e) {
            bool IsLogoffEnabled = toggleButtonLogoff.IsChecked.HasValue ? toggleButtonLogoff.IsChecked.Value : false;

            if(IsLogoffEnabled)
                toggleButtonLogoff.IsChecked = !toggleButtonLogoff.IsChecked;

            toggleButtonBoot.IsChecked = !toggleButtonBoot.IsChecked;
        }

        private void ToggleBootChecked(object sender, RoutedEventArgs e) {
            bool IsLogoffEnabled = toggleButtonLogoff.IsChecked.HasValue ? toggleButtonLogoff.IsChecked.Value : false;

            if(IsLogoffEnabled)
                toggleButtonLogoff.IsChecked = !toggleButtonLogoff.IsChecked;
        }

        private async void UpdateGroupPolicies(object sender, RoutedEventArgs e) {
            buttonUpdate.IsEnabled = false;
            progressBar.Visibility = Visibility.Visible;

            bool IsForceEnabled = toggleButtonOptionForce.IsChecked.HasValue ? toggleButtonOptionForce.IsChecked.Value : false;
            bool IsLogoffEnabled = toggleButtonLogoff.IsChecked.HasValue ? toggleButtonLogoff.IsChecked.Value : false;
            bool IsBootEnabled = toggleButtonBoot.IsChecked.HasValue ? toggleButtonBoot.IsChecked.Value : false;
            bool IsSyncEnabled = toggleButtonSync.IsChecked.HasValue ? toggleButtonSync.IsChecked.Value : false;

            bool targetAll = radioButtonBoth.IsChecked.HasValue ? radioButtonBoth.IsChecked.Value : false;
            bool targetComputer = radioButtonComputer.IsChecked.HasValue ? radioButtonComputer.IsChecked.Value : false;
            bool targetUser = radioButtonUser.IsChecked.HasValue ? radioButtonUser.IsChecked.Value : false;

            int TargetNumber = (targetAll ? 0 : (targetComputer ? 1 : 2));

            int waitTime;

            if(int.TryParse(textBoxWait.Text.Replace(" ", ""), out waitTime)) {
                if(waitTime < -1)
                    waitTime = 600;
            } else {
                waitTime = 600;
            }

            await Task.Run(() => ExecuteGPUpdate(IsForceEnabled, IsLogoffEnabled, IsBootEnabled, IsSyncEnabled, TargetNumber, waitTime));

            buttonUpdate.IsEnabled = true;
            progressBar.Visibility = Visibility.Hidden;
        }

        private void ShowLogs(object sender, RoutedEventArgs e) {
            Window w = new GPUpdater.WindowLogs();
            w.Show();
        }

        private void ExecuteGPUpdate(Boolean IsForceEnabled, Boolean IsLogoffEnabled, Boolean IsBootEnabled, Boolean IsSyncEnabled, int TargetNumber, int waitTime) {
            ProcessStartInfo procStartInfo = new ProcessStartInfo() {
                FileName = @"C:\Windows\System32\gpupdate.exe",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            procStartInfo.Arguments = (IsForceEnabled ? "/force " : "");
            procStartInfo.Arguments += (IsLogoffEnabled ? "/logoff " : "");
            procStartInfo.Arguments += (IsBootEnabled ? "/boot " : "");
            procStartInfo.Arguments += (IsSyncEnabled ? "/sync " : "");

            switch(TargetNumber) {
                case 1:
                    procStartInfo.Arguments += "/target:computer ";
                    break;
                case 2:
                    procStartInfo.Arguments += "/target:user ";
                    break;
                case 0:
                default:
                    break;
            }

            procStartInfo.Arguments += "/wait:" + waitTime;

            Console.Out.WriteLine("This command will be used: " + procStartInfo.FileName + " " + procStartInfo.Arguments);

            Logs.logs.Add("> " + procStartInfo.FileName + " " + procStartInfo.Arguments);

            using(Process proc = new Process()) {
                proc.StartInfo = procStartInfo;
                proc.Start();

                while(!proc.StandardOutput.EndOfStream) {
                    string output = proc.StandardOutput.ReadLine();

                    if(!string.IsNullOrWhiteSpace(output)) {
                        Console.Out.WriteLine(output);
                        Logs.logs.Add(output);
                    }
                }
            }
        }

        private void MainWindowKeyDown(object sender, KeyEventArgs e) {
            if(e.Key.ToString() == "F1") {
                Window w = new WindowAbout();
                w.Show();
            }
        }

        private void Window_Closed(object sender, EventArgs e) {
            Process.GetCurrentProcess().Kill();
        }
    }

    public static class Logs {
        public static ArrayList logs = new ArrayList();
    }
}
