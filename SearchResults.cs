using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PlexWalk
{
    public partial class SearchResults : Form, FormInterface
    {
        List<Descriptor> searches;
        string query;
        public SearchResults(List<Descriptor> searches, string query)
        {
            InitializeComponent();
            this.searches = searches;
            this.query = query;
            this.Text += " - " + query;
        }

        private void SearchResults_Load(object sender, EventArgs e)
        {
            foreach (Descriptor desc in searches)
            {
                TreeNode n = new TreeNode(desc.serverName) { Tag = desc, Name = "/search?query=" + Uri.EscapeDataString(query) };
                n.Nodes.Add("");
                searchTreeView.Nodes.Add(n);
            }
            foreach (TreeNode n in searchTreeView.Nodes)
                n.Expand();
        }

        delegate void ChangeNodeCallback(object sender, TreeNode src);

        private void ExpandNode(object sender, TreeNode src)
        {
            if (((Control)sender).InvokeRequired)
            {
                ChangeNodeCallback d = new ChangeNodeCallback(ExpandNode);
                this.Invoke(d, new object[] { sender, src });
            }
            else
                PlexUtils.populateSubNodes(src, this);
        }

        delegate void AddNodeCallback(TreeNode Parent, TreeNode Child);

        public void AddNode(TreeNode Parent, TreeNode Child)
        {
            if (searchTreeView.InvokeRequired)
            {
                AddNodeCallback d = new AddNodeCallback(AddNode);
                this.Invoke(d, new object[] { Parent, Child });
            }
            else
                Parent.Nodes.Add(Child);
        }

        private void searchTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(delegate(object state)
            {
                object[] array = state as object[];
                TreeNode src = (TreeNode)array[0];
                if (src.FirstNode.Text == null || src.FirstNode.Text == "")
                {
                    ClearNodes(sender, src);
                    try
                    {
                        PlexUtils.populateSubNodes(src, this);
                    }
                    catch
                    {
                    }
                }
            }, new object[] { e.Node });
        }
        private void ClearNodes(object sender, TreeNode src)
        {
            if (((Control)sender).InvokeRequired)
            {
                ChangeNodeCallback d = new ChangeNodeCallback(ClearNodes);
                this.Invoke(d, new object[] { sender, src });
            }
            else
                src.Nodes.Clear();
        }
    }
}
