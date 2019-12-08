using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static TreeGUI.Extensions;

// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace TreeGUI
{
    public partial class Form1 : Form
    {
        public TreeController contr = TreeController.contr;
        private bool keyHandled;
        private int _inSync;
        private bool _firstActivateDone;
        private bool _searchResultsVisible;

        public Form1()
        {
            InitializeComponent();
            contr.NodeAdded += NodeAdded;

            TreeGUI.TreeController.dbgSupressAssertion = true;
            //TreeController.dbgManualSerialization = true;
            timerForm=new TimerForm(this);
        }

        public TimerForm timerForm { get; set; }

        private void NodeAdded(TreeController.Node node, bool byUser)
        {
            if (node.Parent != null)
            { 
                var viewParent = TreeNodeByNode(node.Parent);
                var newNode = new TreeNode("");
                newNode.Tag = node;
                viewParent.Nodes.Insert(node.Index, newNode);
                SyncNode(newNode, false);
                SyncNode(viewParent, false); //IsExpanded workaround after treeView1.Node.Clear;
                if(byUser)
                    treeView1.SelectedNode = newNode; 
                return;
            }
            else
            {
                var newNode = treeView1.Nodes.Add("");
                newNode.Tag = node; 
                SyncNode(newNode, false);
                treeView1.SelectedNode = newNode;
                return;
            }

            //Debug.Assert(false);
        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Click on the link below to continue learning how to build a desktop app using WinForms!
            System.Diagnostics.Process.Start("http://aka.ms/dotnet-get-started-desktop");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //contr.AddNode(treeView1.SelectedNode.Tag as TreeController.Node);
            treeView1.Nodes.Clear();
            TreeController.contr.Serialize(false);
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if(treeView1.SelectedNode!=null)
                SyncNode(treeView1.SelectedNode, true);
            contr.SerializeIfNeeded();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //panel1.Visible = TreeController.isDbg;
            SearchResultsVisible = false;
        }

        public bool SearchResultsVisible
        {
            get
            {
                return _searchResultsVisible;} set
            {
                if (value&&_searchResultsVisible)
                    return;
                _searchResultsVisible = value;
                splitContainer2.SplitterDistance= value?ClientRectangle.Height/3:splitContainer2.Panel1MinSize;

            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
        }
            
        private void TreeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label.SafeString() == "")
                e.CancelEdit = true;
            else
                e.Node.Text = e.Label;
            SyncNode(e.Node, true);
        }

        private void TreeView1_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = keyHandled;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = keyHandled;
            switch (e.KeyData)
            {
                case Keys.Alt | Keys.D1:
                    if (treeView1.Focused)
                        rtNotes.Focus();
                    else
                        treeView1.Focus();
                    break;

                case Keys.L | Keys.Control:
                    Clipboard.SetData(DataFormats.Text, (Object) $"{CurrentNode.PathWithProjects()}\n{TreeController.mlrLinkPrefixNew}{CurrentNode.Guid}}}");
                    break;
                case Keys.Control | Keys.F:
                    textBox1.Focus();
                    break;
                case Keys.Escape:
                    SearchResultsVisible = false;
                    break;
            }
        }

        public TreeController.Node CurrentNode
        {
            get
            {
                return treeView1.SelectedNode.Tag as TreeController.Node;
            } //; set;
            set { treeView1.SelectedNode=TreeNodeByNode(value)?? treeView1.SelectedNode; }
        }

        private TreeNode TreeNodeByNode(TreeController.Node node)
        {
            foreach (TreeNode each in treeView1.Nodes.Find("", true))
                if (each.Tag == node)
                    return each;
        
            return null;
        }

        private void TreeView1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Debug.Print(e.KeyData.ToString());
            var node = treeView1.SelectedNode;
            if(node==null)return;
            
            var parent = node.Parent;
            keyHandled = true;
            
            switch (e.KeyData)
            {
                case Keys.F2:
                    treeView1.SelectedNode.BeginEdit();
                    break;
                case Keys.Insert:
                    contr.AddNode(treeView1.SelectedNode.Tag as TreeController.Node, true);
                    treeView1.SelectedNode.BeginEdit();
                    break;
                case Keys.Delete:
                    //TODO
                    break;
                case Keys.Left | Keys.Alt:
                    if (treeView1.SelectedNode?.Parent.Parent != null)
                    { //TODO: ?To contr
                        _inSync++;
                        try
                        {
                            node.Remove();
                            parent.Parent.Nodes.Insert(parent.Index + 1, node);
                        }finally{ _inSync--; }
                        SyncNode(node, true);
                        treeView1.SelectedNode=node;
                    }
                    break;
                case Keys.Right | Keys.Alt:
                    if (treeView1.SelectedNode?.PrevNode != null)
                    {
                        //TODO: ?To contr
                        var prev = node.PrevNode;
                        node.Remove();
                        prev.Nodes.Add(node);
                        treeView1.SelectedNode = node;
                        SyncNode(node, true);
                    }

                    break;

                case Keys.Up | Keys.Alt:
                case Keys.Down | Keys.Alt:
                    var Idx = node.Index + (e.KeyValue == (int)Keys.Up ? -1 : 1);
                    
                    SetNodeIndex(node, Idx);
                    SyncNode(node, true);
                    treeView1.SelectedNode = node;
                    break;
                        default:
                            keyHandled = false;
                    break;
            }

        }


        private void SyncNode(TreeNode node, bool update)
        {
            if (_inSync>0) return;
            _inSync++;
            try
            {
                var contrNode = node.Tag as TreeController.Node;

                if (update)
                {
                    contrNode.label = node.Text;
                    contrNode.Parent = node.Parent?.Tag as TreeController.Node;
                    contrNode.Index = node.Index;
                    contrNode.IsExpanded = node.IsExpanded;
                    contrNode.DebugPrint($"SyncNode contrNode.Expanded={contrNode.IsExpanded}");
                    contrNode.Checked = node.Checked;
                    if (treeView1.SelectedNode == node)
                    {
                        contrNode.Notes = rtNotes.Text;
                    }

                    if(treeView1.SelectedNode?.Tag!=null)
                        contr.currentNode = treeView1.SelectedNode.Tag as TreeController.Node;
                    
                    contrNode.DebugPrint($"SyncNode->");
                }
                else
                {
                    contrNode.DebugPrint($"SyncNode<-");
                    
                    node.Text = contrNode.label;
                    //node.Parent = TODO
                    if (node.Parent != null)
                        SetNodeIndex(node, contrNode.Index);
                    
                    node.Checked = contrNode.Checked;

                    if (contrNode.IsExpanded)
                    {
                        node.Expand();
                        contrNode.DebugPrint($"Expanded viewNode.IsExpanded == {node.IsExpanded} ;subCnt=={node.Nodes.Count}");
                    }
                    else
                    {
                        node.Collapse();
                        contrNode.DebugPrint("Collapsed");
                    }
                    
                    if (treeView1.SelectedNode == node)
                    {
                        rtNotes.Text = contrNode.Notes;
                    }

                }
            }finally{ _inSync--; }
        }

        private void SetNodeIndex(TreeNode node, int Idx)
        {
            _inSync++;
            try
            {
                node.SetNodeIndex(Idx);
            }
            finally
            {
                _inSync--;
            }
        }

        private void TreeView1_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = keyHandled;
        }


        private void Button2_Click(object sender, EventArgs e)
        {
            contr.Serialize(true);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            contr.AddNode(null, true);
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if(treeView1.SelectedNode.IsExpanded)
            treeView1.SelectedNode.Collapse();
            else
            {
                treeView1.SelectedNode.Expand();
            }
        }

        private void TreeView1_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            SyncNode(e.Node, true);
        }

        private void TreeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            (e.Node.Tag as TreeController.Node).DebugPrint("TreeView1_AfterExpand");
            SyncNode(e.Node, true);
        }

        private void TreeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            SyncNode(e.Node, true);
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
           SyncNode(e.Node, false);
           TreeController.contr.currentNode = e.Node.Tag as TreeController.Node;
           timerForm.NodeSelected(e.Node.Tag as TreeController.Node);
        }

        private void TreeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if(treeView1.SelectedNode!=null)
                SyncNode(treeView1.SelectedNode, true);
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (_firstActivateDone)
                return;

            treeView1.BeginUpdate();
            contr.LoadFile();
            treeView1.EndUpdate();

            _firstActivateDone = true;
        }

        private void Test1()
        {
            if (!contr.dbgRunTest)
                return;
            Debug.Assert(treeView1.Nodes[0].IsExpanded, "treeView1.Nodes[0].IsExpanded");
            treeView1.Nodes.Clear(); 
            /* 
            for (int index = treeView1.Nodes.Count - 1; index > -1; index--)

            {

                treeView1.Nodes.RemoveAt(index);

            }
            
            treeView1.Nodes.Add("asdf");
            treeView1.Nodes[0].Expand();
            treeView1.Nodes[0].Nodes.Add("fda");
            //treeView1.Nodes[0].Expand();


            */
            contr.Serialize(false);
            Debug.Assert(treeView1.Nodes[0].IsExpanded, "treeView1.Nodes[0].IsExpanded");
        }

        private void RtNotes_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            var path = e.LinkText;
            Guid guid;
            if (ParseMlrLink(path, out guid))
            {
                CurrentNode = contr.NodeByGuid(guid) ?? CurrentNode;
                return;
            }


            ParseMloLink(ref path);
                
            Shell32.Shell shell = new Shell32.Shell();
            shell.ShellExecute(path);
        }

        private bool ParseMlrLink(string path, out Guid guid)
        {
            guid = Guid.Empty;
            if (!path.StartsWith(TreeController.mlrLinkPrefixNew)&& !path.StartsWith(TreeController.mlrLinkPrefixOld))
                return false;

            Debug.Assert(TreeController.mlrLinkPrefixNew.Length == TreeController.mlrLinkPrefixOld.Length);

            return Guid.TryParse(path.Remove(0, TreeController.mlrLinkPrefixNew.Length), out guid);
        }

        private bool ParseMloLink(ref string eLinkText)
        {
            const string ProtoPrefix = "\\\\mlo:{";
            const string newPrefix = "mlo://x:\\mlo-VsevGlobal\\vsevGlobal.ml?{";
            if (!eLinkText.StartsWith(ProtoPrefix))
                return false;
            
            eLinkText = newPrefix + eLinkText.Remove(0, ProtoPrefix.Length); 
            return true;
        }

        private void TextBox1_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            SearchResultsVisible = SearchResultsVisible;
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            //if (e.KeyData == Keys.Enter)
            //{
                SearchResultsVisible = true;
                //keyHandled = true;
                SearchNodes(textBox1.Text);
            //}
            */

        }

        private void SearchNodes(string pattern)
        {
            lstSearchResults.Items.Clear();
            var results=contr.root.Childs.SelectRecursive(c => c.Childs).Where(
                n => n.Notes?.IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase) >= 0 
                     || n.label?.IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase)>=0
                     );

            foreach (var node in results)
            {
                lstSearchResults.Items.Add(node.NewViewItem(true, new DateTime()));
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(keyHandled)
                e.Handled = true;
            keyHandled = false;
        }

        private void BtHideResults_Click(object sender, EventArgs e)
        {
            SearchResultsVisible = false;
        }

        private void LstSearchResults_SelectedValueChanged(object sender, EventArgs e)
        {
            treeView1.SelectedNode = 
                TreeNodeByNode((lstSearchResults.SelectedItem as TreeController.Node.ViewItem).Node);
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            SearchResultsVisible = true;
            SearchNodes(textBox1.Text);
        }
    }
}
