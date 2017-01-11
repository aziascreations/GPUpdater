using System;
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

            if (IsBootEnabled)
                toggleButtonBoot.IsChecked = !toggleButtonBoot.IsChecked;
        }

        //Boot switch functions
        private void LabelBootClick(object sender, MouseButtonEventArgs e) {
            bool IsLogoffEnabled = toggleButtonLogoff.IsChecked.HasValue ? toggleButtonLogoff.IsChecked.Value : false;

            if (IsLogoffEnabled)
                toggleButtonLogoff.IsChecked = !toggleButtonLogoff.IsChecked;

            toggleButtonBoot.IsChecked = !toggleButtonBoot.IsChecked;
        }

        private void ToggleBootChecked(object sender, RoutedEventArgs e) {
            bool IsLogoffEnabled = toggleButtonLogoff.IsChecked.HasValue ? toggleButtonLogoff.IsChecked.Value : false;

            if (IsLogoffEnabled)
                toggleButtonLogoff.IsChecked = !toggleButtonLogoff.IsChecked;
        }

        private async void UpdateGroupPolicies(object sender, RoutedEventArgs e) {
            buttonUpdate.IsEnabled = false;
            buttonLogs.IsEnabled = false;
            progressBar.Visibility = Visibility.Visible;

            //await Task.Delay(1000);

            //await ExecuteGPUpdate();

            bool IsForceEnabled = toggleButtonOptionForce.IsChecked.HasValue ? toggleButtonOptionForce.IsChecked.Value : false;
            bool IsLogoffEnabled = toggleButtonLogoff.IsChecked.HasValue ? toggleButtonLogoff.IsChecked.Value : false;
            bool IsBootEnabled = toggleButtonBoot.IsChecked.HasValue ? toggleButtonBoot.IsChecked.Value : false;
            bool IsSyncEnabled = toggleButtonSync.IsChecked.HasValue ? toggleButtonSync.IsChecked.Value : false;

            await Task.Run(() => ExecuteGPUpdate(IsForceEnabled, IsLogoffEnabled, IsBootEnabled, IsSyncEnabled));

            buttonUpdate.IsEnabled = true;
            //buttonLogs.IsEnabled = true;
            progressBar.Visibility = Visibility.Hidden;
        }

        private void ShowLogs(object sender, RoutedEventArgs e) {

        }

        private void ExecuteGPUpdate(Boolean IsForceEnabled, Boolean IsLogoffEnabled, Boolean IsBootEnabled, Boolean IsSyncEnabled) {
            ProcessStartInfo procStartInfo = new ProcessStartInfo() {
                //RedirectStandardError = true,
                //FileName = "runas.exe",
                //Arguments = "/user:Administrator \"cmd /K GPUpdate.exe"
                FileName = @"C:\Windows\System32\gpupdate.exe",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            
            procStartInfo.Arguments = (IsForceEnabled ? "/force " : "");
            procStartInfo.Arguments += (IsLogoffEnabled ? "/logoff " : "");
            procStartInfo.Arguments += (IsBootEnabled ? "/boot " : "");
            procStartInfo.Arguments += (IsSyncEnabled ? "/sync " : "");

            Console.Out.WriteLine("This command will be used: " + procStartInfo.FileName + " " + procStartInfo.Arguments);

            using (Process proc = new Process()) {
                proc.StartInfo = procStartInfo;
                proc.Start();

                string output = "";

                while (!proc.StandardOutput.EndOfStream) {
                    if(!string.IsNullOrWhiteSpace(proc.StandardOutput.ReadLine()))
                        output += proc.StandardOutput.ReadLine() + "\n";
                }
                Console.Out.WriteLine(output);
            }
        }

        private void MainWindowKeyDown(object sender, KeyEventArgs e) {
            if (e.Key.ToString() == "F1") {
                Window w = new WindowAbout();
                w.Show();
            }
        }
    }
}
