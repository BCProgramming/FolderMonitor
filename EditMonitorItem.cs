using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Shell;


namespace FolderMonitor
{
    public partial class EditMonitorItem : Form
    {
        private MonitorConfigurationItem mci = null;
        public EditMonitorItem(MonitorConfigurationItem itemedit)
        {
            mci = itemedit;
            
            InitializeComponent();
        }

        private void cmdBrowsePath_Click(object sender, EventArgs e)
        {
            BrowseFolderDialogEx fbdx = new BrowseFolderDialogEx();
            
            fbdx.BrowseFlags = BrowseFlags.EditBox | BrowseFlags.ReturnOnlyFSDirs | BrowseFlags.NewDialogStyle;
            if(fbdx.ShowDialog(this) == DialogResult.OK)
            {
                txtPath.Text = fbdx.FolderPath;
            }

            
        }

        private void EditMonitorItem_Load(object sender, EventArgs e)
        {
            txtPath.Text = mci.MonitorPath;
            txtFilter.Text = mci.Filter;
            txtTitle.Text = mci.Title;
            chkIncludeSubdirs.Checked = mci.Subdirectories;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            mci.MonitorPath = txtPath.Text;
            mci.Filter = txtFilter.Text;
            mci.Title = txtTitle.Text;
            mci.Subdirectories = chkIncludeSubdirs.Checked;
            Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        public static bool EditItem(IWin32Window owner,MonitorConfigurationItem mciitem)
        {
            EditMonitorItem dlgUse = new EditMonitorItem(mciitem);
            return dlgUse.ShowDialog(owner) == System.Windows.Forms.DialogResult.OK;
        }
    }
}
