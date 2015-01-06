using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiAlarm
{
    public partial class MainForm : Form
    {
        private List<Alarm> alarmList = new List<Alarm>();

        public MainForm()
        {
            InitializeComponent();
            this.MinimumSize = new Size(500, 300);
            CreateTabs(2,new int[]{2, 3});

            
        }



        public void CreateTabs(int groups, int[] alarmNumbers)
        {
            int rowHeight = 99;
            TabControl tabControl = new TabControl();
            this.Controls.Add(tabControl);
            tabControl.Dock = DockStyle.Fill;
            
            
            
            for (int g=0;g<groups;g++)
            {
                TabPage tabPage = new TabPage("Group " + (g+1));
                tabControl.Controls.Add(tabPage);
                tabPage.AutoScroll = true;
                

                TableLayoutPanel panel = new TableLayoutPanel();                
                panel.ColumnCount = 1;
                panel.RowCount = 5;
                
                panel.Dock = DockStyle.Top;
                tabPage.Controls.Add(panel);
                panel.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
                panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                panel.AutoSize = true;
                panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;


                for (int n=0;n<alarmNumbers[g];n++)
                {
                    Alarm alarm = new Alarm(g, n,new TimeSpan(1,0,5));
                    alarmList.Add(alarm);
                    Panel alarmPanel = alarm.CreateAlarmPanel(rowHeight);
                    panel.Controls.Add(alarmPanel);
                    alarmPanel.Dock = DockStyle.Fill;                    
                }
            }
        }
        


    }
}
