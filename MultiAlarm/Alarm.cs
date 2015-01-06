using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Windows;
using System.IO;


namespace MultiAlarm
{
    class Alarm
    {
        private TextBox timeBox;
        private Timer timer = new Timer();
        private Panel alarmPanel;
        private Panel progressBar;
        private Button button;


        public int group
        {
            get; set;
        }
        public int alarmNumber
        {
            get; set;
        }
        public DateTime startTime
        {
            get; private set;
        }
        public DateTime endTime
        {
            get; private set;
        }        
        public TimeSpan duration
        {
            get; private set;
        }

        /// <summary>
        /// Create new alarm
        /// </summary>
        /// <param name="group">Place alarm in group</param>
        /// <param name="alarmNumber">Place alarm as this number in that group</param>
        /// <param name="duration">Duration of alarm</param>
        public Alarm(int group, int alarmNumber, TimeSpan duration)
        {
            this.group = group;
            this.alarmNumber = alarmNumber;
            this.duration = duration;
        }



        /// <summary>
        /// Start alarm
        /// </summary>
        /// <param name="duration">Duration of alarm</param>
        public void Start(TimeSpan duration)
        {            
            this.duration = duration;
            Start();                        
        }

        /// <summary>
        /// Start alarm with previously set duration
        /// </summary>
        public void Start ()
        {
                    
            button.Visible = false;
            alarmPanel.BackColor = Color.LightGreen;
            timeBox.ReadOnly = true;
            
            startTime = DateTime.Now;
            endTime = startTime.Add(duration);
                        
            timer.Interval = 1000;
            timer.Tick += timer_Tick;
            
            timer.Start();//
        }

        void timer_Tick(object sender, EventArgs e)
        {
            UpdateAlarm();
        }
        
        /// <summary>
        /// Creates and returns the alarmpanel to attach in list inside group-tab
        /// </summary>
        /// <param name="height">Height of row to put panel in</param>
        /// <returns></returns>
        public Panel CreateAlarmPanel(int height)
        {
            
            alarmPanel = new Panel();
            alarmPanel.Height = height;
            alarmPanel.BackColor = Color.LightGray;
            alarmPanel.MouseDown += alarmPanel_MouseDown;

            
            progressBar = new Panel();
            alarmPanel.Controls.Add(progressBar);
            progressBar.Dock = DockStyle.Left;
            progressBar.BackColor = Color.Green;
            progressBar.Width = 0;
            progressBar.MouseDown += alarmPanel_MouseDown;
                    
            button = new Button();
            button.Text = "Alarm " + (alarmNumber+1);
            button.Size = new Size(height, height );
            button.Click += new EventHandler(onButtonClick);
            alarmPanel.Controls.Add(button);
                       

            timeBox = new TextBox();
            timeBox.Text = TimeBoxFormat(duration);
            timeBox.Font = new Font(timeBox.Font.FontFamily, 40);
            timeBox.Size = new Size(200,60);
            timeBox.TextAlign = HorizontalAlignment.Right;
            alarmPanel.Controls.Add(timeBox);
            timeBox.Location = new Point(height + 50, height/2 - timeBox.Height / 2);
            timeBox.KeyDown +=timeBox_KeyDown;


            return alarmPanel;
        }





        void alarmPanel_MouseDown(object sender, MouseEventArgs e)
        {
            
            if (e.Button == MouseButtons.Right && timer.Enabled == true)
            {
                ContextMenu rightClickMenu = new ContextMenu();
                MenuItem menuReset = new MenuItem("Reset Alarm");
                menuReset.Click += menuReset_Click;
                rightClickMenu.MenuItems.Add(menuReset);
                rightClickMenu.Show(alarmPanel,e.Location);
            }
        }

        void menuReset_Click(object sender, EventArgs e)
        {
            timer.Stop();
            button.Visible = true;
            alarmPanel.BackColor = Color.LightGray;
            timeBox.ReadOnly = false;
            timeBox.Text = TimeBoxFormat(duration);
            progressBar.Width = 0;
            progressBar.BackColor = Color.Green;

        }

        private void onButtonClick(object sender, EventArgs e)
        {            
            try
            {
                duration = TimeSpanFromString.Check(timeBox.Text);
                Start();
            }
            catch (FormatException exception)
            {
                MessageBox.Show(exception.Message);
            }
            
        }
        void timeBox_KeyDown(object sender, KeyEventArgs e) //check if possible to integrate into
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {

                try
                {
                    duration = TimeSpanFromString.Check(timeBox.Text);
                    Start();
                }
                catch (FormatException exception)
                {
                    MessageBox.Show(exception.Message);
                    
                }
                
                
            }
            
        }



        
        /// <summary>
        /// Updates a running alarms progressbar and time textbox
        /// </summary>
        public void UpdateAlarm()
        {
            DateTime now = DateTime.Now;
            TimeSpan timeLeft = endTime-now;
            if (timeLeft.TotalMilliseconds>0)
            {
                timeBox.Text = TimeBoxFormat(timeLeft);
                progressBar.Width = (int)(alarmPanel.Width *(1- (timeLeft.TotalMilliseconds /duration.TotalMilliseconds)));
                progressBar.SendToBack();
            }
            else
            {
                TriggerAlarm();
            }
            
        }
                
        private void TriggerAlarm()
        {
            timer.Stop();
            progressBar.Dock = DockStyle.Fill;
            progressBar.BackColor = Color.Red;

            string fileName = Directory.GetCurrentDirectory()+ @"\alarm.wav";

            if (System.IO.File.Exists(fileName))
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(fileName);
                player.Play();
            }
            else
            {
                MessageBox.Show("alarm.wav could not be found");
            }



        }

        /// <summary>
        /// Return correct format on string for timeBox
        /// </summary>
        /// <param name="timeSpan">Timespan for formatting</param>
        /// <returns></returns>
        private string TimeBoxFormat(TimeSpan timeSpan)
        {
            string timeString="";

            if (timeSpan.Hours>=10)
            {
                timeString = timeSpan.ToString(@"hh\:mm\:ss");
            }

            if (timeSpan.Hours>=1)
            {
                timeString = timeSpan.ToString(@"h\:mm\:ss");
            }
            else
            {
                timeString = timeSpan.ToString(@"mm\:ss");
            }

            return timeString;
        }


    }
}
