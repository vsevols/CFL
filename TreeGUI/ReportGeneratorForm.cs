using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TreeGUI
{
    public partial class ReportGeneratorForm : Form
    {
        public ReportGeneratorForm()
        {
            InitializeComponent();
        }

        private void ReportGeneratorForm_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Today.AddDays(-1);
            dateTimePicker2.Value = DateTime.Today;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ReportGenerate(dateTimePicker1.Value, dateTimePicker2.Value);
        }

        private void ReportGenerate(DateTime since, DateTime till)
        {
            var node = TreeController.contr.currentNode;
            var timePeriods = node.SelectTimePeriodsRecursive(since, till);

            using (var w = new StreamWriter(Application.ExecutablePath + "report.csv"))
            {
                foreach (var timePeriod in timePeriods)
                {
                    w.WriteLine(ReportLine(timePeriod));
                }
                w.Flush();
            }

        }

        private string ReportLine(TreeController.Node.TimePeriod timePeriod)
        {
            return $"{timePeriod.Since.ToString()};{timePeriod.Till.ToString()};"
            +$"{TreeController.TimeSpanFmt(timePeriod.Till-timePeriod.Since)};{timePeriod.Node.PathWithProjects(false,";")}";
        }
    }
}
