using System;
using System.Collections;
using System.Windows;

namespace GPUpdater {

    public partial class WindowLogs : Window {

        private ArrayList lines = new ArrayList();
        private int linesStartingIndex = 0;
        private int lastLinesAmount = 0;

        public WindowLogs() {
            InitializeComponent();

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            this.lines = GPHelper.Logs.logs;
            lastLinesAmount = this.lines.Count;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e) {
            if(lastLinesAmount < GPHelper.Logs.logs.Count) {
                this.lines = GPHelper.Logs.logs;
                textBlockLogs.Text = "";
                lastLinesAmount = 0;

                for(int i = this.linesStartingIndex; i < this.lines.Count; i++) {
                    textBlockLogs.Text += this.lines[i];

                    if(i != this.lines.Count)
                        textBlockLogs.Text += "\n";

                    lastLinesAmount++;
                }
            }
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e) {
            textBlockLogs.Text = "";
            this.linesStartingIndex = this.lines.Count;
        }
    }
}
