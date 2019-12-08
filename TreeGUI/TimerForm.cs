using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win32_API;

namespace TreeGUI
{
    public partial class TimerForm : Form
    {
        private TreeController.Node _node;
        private TreeController.Node _lastSelectedNode;
        private bool _lockNode;
        private bool _unlockNodeOnProjectReturn;
        private Form1 _mainForm;
        private TreeController.Node.TimePeriod _period;
        private DateTime _lastSerialized;

        Guid sysGUID = new Guid("FE9743B2-D88E-4B7C-A458-0D17D7A99949");
        Guid extrusionsGUID = new Guid("D3BF89D2-71F3-4F43-9133-03A3D016F7C0");

        public TimerForm(Form1 mainForm)
        {
            InitializeComponent();
            _mainForm=mainForm;
            Visible = true;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (!timer1.Enabled)
            {
                timer1.Enabled = true;
                _period = new TreeController.Node.TimePeriod(Node, DateTime.Now);
                RefreshGUI(true);
            }
            else
            {
                timer1.Enabled = false;
                RefreshGUI(true);
            }
        }


        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (CheckIdle())
                return;
            var periodChanged= _period==null;
            _period =_period??new TreeController.Node.TimePeriod(Node, DateTime.Now);
            _period.Till = DateTime.Now;// += tmpNow - LastMoment;
            RefreshGUI(periodChanged);
            if ((DateTime.Now - _lastSerialized).Minutes > 0)
            {
                Node.ParentTree.SerializeIfNeeded();
                _lastSerialized = DateTime.Now;
            }
        }

        private bool CheckIdle()
        {
            if (Win32.GetTickCount() - Win32.GetLastInputTime() > 60000)
            {
                _period = null;
                return true;
            }

            return false;
        }

        private void RefreshGUI(bool periodChanged)
        {
            lbNodeLabel.Text = Node?.label;
            lbTimeSpent.Text = TreeController.TimeSpanFmt(
                Node?.NearestParentProject.TimeSpentWithChilds(PastDayHour(5)));
            Text = $"{lbTimeSpent.Text} {Node?.label}";
            btStart.Text = timer1.Enabled ? "Pause" : "Start";
            chkLockNode.Checked = LockNode;
            lbStatistics.Text =
                "Since 5 am:\n"
                + $"Total with childs:\n {TreeController.TimeSpanFmt(Node?.TimeSpentWithChilds(PastDayHour(5)))}\n"
                + $"Parent project:\n {Node?.ParentProjectInfo(PastDayHour(5))}\n"
                + "Last 24hrs:\n"
                + $"Total with childs:\n {TreeController.TimeSpanFmt(Node?.TimeSpentWithChilds(DateTime.Now.AddDays(-1)))}\n"
                + $"Parent project:\n {Node?.ParentProjectInfo(DateTime.Now.AddDays(-1))}\n"
                + "All time:\n"
                + $"Total with childs:\n {TreeController.TimeSpanFmt(Node?.TimeSpentWithChilds())}\n"
                + $"Parent project:\n {Node?.ParentProjectInfo()}";

            btExpand.Text = IsExpanded?"<<":">>";
            chkIsProject.Checked = Node?.IsProject??false;

            lstPeriods.BeginUpdate();
            if (periodChanged)
            {
                lstPeriods.Items.Clear();
                foreach (var nodeTimePeriod in Enumerable.Reverse(Node.TimePeriods))
                {
                    lstPeriods.Items.Add(nodeTimePeriod.ToString());
                }

                RefreshExtrusions();
            }
            lstPeriods.EndUpdate();
        }

        private static DateTime PastDayHour(int hour)
        {
            return DateTime.Today.AddHours(hour)<=DateTime.Now?
                DateTime.Today.AddHours(hour) : DateTime.Today.AddHours(hour).AddDays(-1);
        }

        private void RefreshExtrusions()
        {
            lstExtrusion.BeginUpdate();

            lstExtrusion.Items.Clear();
            var extrusions=this.Node.ParentTree.NodeByGuid(extrusionsGUID);
            foreach (var extrusion in extrusions.Childs)
            {
                lstExtrusion.Items.Add(
                    extrusion.Extends.TryNode?.NewViewItem(
                        true, extrusion.GetPropertyValueDateTime(PropertyNames.moment))??
                    extrusion.NewViewItem(
                        true, extrusion.GetPropertyValueDateTime(PropertyNames.moment))
                    );
            }

            lstExtrusion.EndUpdate();
        }

        private class PropertyNames
        {
            public const string moment="moment";
        }

        private TreeController.Node Node {
            get
            {
                return _node;
            }
            set
            {
                _node = value;
                RefreshGUI(true);
            }
        }

        public void NodeSelected(TreeController.Node node)
        {
            if (node.NearestParentProject != Node?.NearestParentProject)
            {
                if (!LockNode && Node != null)
                {
                    //Turned out to be inconvenient
                    /*
                    LockNode = AskUser();
                    _unlockNodeOnProjectReturn = LockNode;
                    */
                }
            }

            if (
                (lstExtrusion.Items.Count == 0)
                ||
                ((lstExtrusion.Items[0] as TreeController.Node.ViewItem).Node.NearestParentProject !=
                 node.NearestParentProject)
            )
            {
                if (lstExtrusion.Items.Count > 0 &&
                    (lstExtrusion.Items[0] as TreeController.Node.ViewItem).Node != _lastSelectedNode)
                {
                    lstExtrusion.Items.Insert(0, _lastSelectedNode.NewViewItem(true, DateTime.Now));
                    AppendExtrusion(_lastSelectedNode, DateTime.Now);
                }

                lstExtrusion.Items.Insert(0, node.NewViewItem(true, DateTime.Now));
                AppendExtrusion(node, DateTime.Now);
            }

            if (_unlockNodeOnProjectReturn && node.NearestParentProject == Node.NearestParentProject)
                LockNode = false;

            _lastSelectedNode = node;
            LockNode = LockNode;
        }

        private void AppendExtrusion(TreeController.Node Node, DateTime moment)
        {
            var sys = Node.ParentTree.root.GetOrInsertChild(sysGUID, "_sys");
            var extrusionList = sys.GetOrInsertChild(extrusionsGUID, "_extrusions");
            var extension =new TreeController.Node(extrusionList){Extends=new TreeController.Node.ExternalNode(Node), Index = 0};
            extension.SetPropertyValue(PropertyNames.moment, moment);
        }

        private bool AskUser()
        {
            string message = "Lock timer node?";
            string caption = "Parent project changed";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;

            return MessageBox.Show(message, caption, buttons) == System.Windows.Forms.DialogResult.Yes;
        }


        private void TimerForm_Load(object sender, EventArgs e)
        {
            IsExpanded = false;
            MinimumSize =new Size(Width, Height);
            RefreshGUI(false);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
        }

        public bool LockNode
        {
            get { return _lockNode;}
            set
            {
                _lockNode = value;
                if (!_lockNode)
                {
                    Node = _lastSelectedNode;
                    _period = null;
                }//TODO:else Node=Node instead RefreshGUI call
                RefreshGUI(true);
            }
        }

        private void ChkLockNode_CheckedChanged(object sender, EventArgs e)
        {
            LockNode = chkLockNode.Checked;
            _unlockNodeOnProjectReturn = false;
        }

        private void ChkIsProject_CheckedChanged(object sender, EventArgs e)
        {
            Node.IsProject = chkIsProject.Checked;
            RefreshGUI(false);
        }

        private void BtExpand_Click(object sender, EventArgs e)
        {
            IsExpanded = !IsExpanded;
            RefreshGUI(false);
        }

        public bool IsExpanded
        {
            get { return Width != MinimumSize.Width;}
            set
            {
                if(!value)
                    SetClientSizeCore(panel1.Right, panel1.Bottom);
                else
                    SetClientSizeCore(panel2.Right, panel2.Bottom);

                RefreshGUI(false);
            }
        }

        private void LstExtrusion_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LstExtrusion_DoubleClick(object sender, EventArgs e)
        {
            var item=lstExtrusion.SelectedItem as TreeController.Node.ViewItem;
            _mainForm.CurrentNode = item.Node;
        }

        private void TimerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            WindowState = WindowState == FormWindowState.Minimized?FormWindowState.Normal:FormWindowState.Minimized;

        }

        private void BtReport_Click(object sender, EventArgs e)
        {
            (new ReportGeneratorForm()).Visible = true;
        }
    }

}
