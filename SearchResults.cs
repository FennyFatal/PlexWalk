using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PlexWalk
{
    public partial class SearchResults : Form
    {
        TreeNodeCollection searches;
        public SearchResults(TreeNodeCollection searches)
        {
            InitializeComponent();
            this.searches = searches;
        }

        private void SearchResults_Load(object sender, EventArgs e)
        {
            foreach (TreeNode tn in searches)
            {
                
            }
        }
    }
}
