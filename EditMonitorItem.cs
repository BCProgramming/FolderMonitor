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
using Microsoft.WindowsAPICodePack.Dialogs;


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

        private String SelectFolder(String sDefaultDir = null)
        {

            if(CommonOpenFileDialog.IsPlatformSupported)
            {
                CommonOpenFileDialog sfd = new CommonOpenFileDialog();
                sfd.IsFolderPicker = true;
                sfd.AllowNonFileSystemItems=  false;

                if (sDefaultDir != null) sfd.DefaultDirectory = sDefaultDir;
                if(sfd.ShowDialog(this.Handle)==CommonFileDialogResult.Ok)
                {
                    return sfd.FileName;
                }
            }
            else
            {
                BrowseFolderDialogEx fbdx = new BrowseFolderDialogEx();

                fbdx.BrowseFlags = BrowseFlags.EditBox | BrowseFlags.ReturnOnlyFSDirs | BrowseFlags.NewDialogStyle;
                if (!String.IsNullOrEmpty(sDefaultDir))
                {
                    fbdx.Initialized += (ob, eargs) =>
                    {
                        fbdx.SetSelection(sDefaultDir);
                    };
                }
                if (fbdx.ShowDialog(this) == DialogResult.OK)
                {
                    txtPath.Text = fbdx.FolderPath;
                }
            }
            return null;
        }


        private void cmdBrowsePath_Click(object sender, EventArgs e)
        {
           

            
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
