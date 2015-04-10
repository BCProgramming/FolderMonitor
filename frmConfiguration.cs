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
using System.Windows.Shell;


namespace FolderMonitor
{
    public partial class frmConfiguration : Form
    {
        NotifyIcon ni = null;
        MonitorConfiguration config = null;
        MonitorManager mm = null;
        ContextMenuStrip NotificationMenu = null;
        private void RestartMonitor()
        {
            if(mm!=null)
                mm.Dispose();

            mm = new MonitorManager(config);
            mm.FileChange += mm_FileChange;
            int NumFolders = config.GetItems().Count((f) => f.Active);
            ni.ShowBalloonTip(5000,"BASeCamp Folder Monitor Started","Monitoring " + NumFolders + " Folders.",ToolTipIcon.Info);
        }
        static String mRenameMessage = "File Rename:\n Old:{0}\nNew:{1}";
        static String mCreatedMessage = "File Created:{0}";
        static String mDeletedMessage = "File Deleted:{0}";
        static Dictionary<FolderMonitor.FileChangeEventArgs.FileChangeType, String> ChangeTypeFmtMapping = new Dictionary<FolderMonitor.FileChangeEventArgs.FileChangeType, string>()
        {
            {FolderMonitor.FileChangeEventArgs.FileChangeType.Renamed, mRenameMessage},
            {FolderMonitor.FileChangeEventArgs.FileChangeType.Created, mCreatedMessage},
            {FolderMonitor.FileChangeEventArgs.FileChangeType.Deleted, mDeletedMessage}
        };
        void mm_FileChange(object sender, FolderMonitor.FileChangeEventArgs e)
        {
            FolderMonitor fmSource = sender as FolderMonitor;
            String TypeStr = e.ChangeType.ToString();
            String useFormat = ChangeTypeFmtMapping[e.ChangeType];
            String useMessage;
            if(e.ChangeType==FolderMonitor.FileChangeEventArgs.FileChangeType.Renamed)
            {
                RenamedEventArgs args = (RenamedEventArgs)e.SourceArgs;
                useMessage = String.Format(useFormat, args.OldFullPath, args.FullPath);
            }
            else
            {
                useMessage = String.Format(useFormat, e.FilePath);
            }
            ni.ShowBalloonTip(5000,TypeStr + " Detected.", useMessage,ToolTipIcon.Info);
        }
        public frmConfiguration()
        {
            InitializeComponent();
        }
        private bool Loading = true;
        private void frmConfiguration_Load(object sender, EventArgs e)
        {
            NotificationMenu = new ContextMenuStrip();
            var configureitem = NotificationMenu.Items.Add("Configure...");
            var exitItem = NotificationMenu.Items.Add("Exit");
            configureitem.Click += configureitem_Click;
            exitItem.Click += exitItem_Click;
            ni = new NotifyIcon();
            ni.Site = this.Site;
            ni.Icon = this.Icon;
            ni.Text = "BASeCamp Folder Monitor";
            ni.Visible = true;
            ni.ContextMenuStrip = NotificationMenu;
            
            config = MonitorConfiguration.Static;
            RestartMonitor();
            RepopulateList();
            Hide();

        }

        void exitItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        void configureitem_Click(object sender, EventArgs e)
        {
            Loading = false;
            Show();
        }
        private void RepopulateList()
        {
            lvwMonitorItems.Items.Clear();
            lvwMonitorItems.Columns.Clear();
            lvwMonitorItems.CheckBoxes = true;
            lvwMonitorItems.Columns.Add("Name",128);
            lvwMonitorItems.Columns.Add("Path",256);
            lvwMonitorItems.Columns.Add("Filter",48);
            foreach(var additem in config.GetItems())
            {
                ListViewItem built = BuildItem(additem);
                lvwMonitorItems.Items.Add(built);
            }

        }
        private ListViewItem BuildItem(MonitorConfigurationItem mci)
        {
            ListViewItem result = new ListViewItem(new String[] { mci.Title, mci.MonitorPath, mci.Filter });
            result.Checked = mci.Active;
            result.Tag = mci;
            return result;
        }
        void TestMonitor_FileEvent(object sender, FolderMonitor.FileChangeEventArgs e)
        {
            
           
        }

        private void lvwMonitorItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            tsEditItems.Enabled = tsRemove.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BrowseFolderDialogEx sfb = new BrowseFolderDialogEx();
            sfb.SelChanged += sfb_SelChanged;
            sfb.Title = "Select Folder";
            sfb.BrowseFlags = BrowseFlags.ReturnOnlyFSDirs | BrowseFlags.NewDialogStyle;
            if(sfb.ShowDialog()==DialogResult.OK)
            {
                MessageBox.Show("Selected:" + sfb.FolderPath);
            }
           
           
        }

        void sfb_SelChanged(object sender, BrowseSelChangedEventArgs e)
        {
            Text = e.SelectedPath;
        }

        private void tsNewItem_Click(object sender, EventArgs e)
        {
            MonitorConfigurationItem mci = new MonitorConfigurationItem();
            ListViewItem newitem = BuildItem(mci);
            lvwMonitorItems.Items.Add(newitem);
            if(EditMonitorItem.EditItem(this,mci))
            {
                RefreshItem(newitem);

            }
            else
            {
                lvwMonitorItems.Items.Remove(newitem);
            }

        }
        private void RefreshItem(ListViewItem refreshit)
        {
            MonitorConfigurationItem mci = (MonitorConfigurationItem)refreshit.Tag;
            refreshit.Text = mci.Title;
            refreshit.SubItems[0].Text = mci.MonitorPath;
            refreshit.SubItems[1].Text = mci.Filter;
            refreshit.Checked = mci.Active;

        }

        private void tsEdit_Click(object sender, EventArgs e)
        {
            if (lvwMonitorItems.SelectedItems.Count > 0)
            {
                ListViewItem selecteditem = lvwMonitorItems.SelectedItems[0];
                MonitorConfigurationItem mci = selecteditem.Tag as MonitorConfigurationItem;
                if(EditMonitorItem.EditItem(this,mci))
                    RefreshItem(selecteditem);
            }
        }

        private void tsRemove_Click(object sender, EventArgs e)
        {
            if (lvwMonitorItems.SelectedItems.Count > 0)
            {
                ListViewItem selecteditem = lvwMonitorItems.SelectedItems[0];
                MonitorConfigurationItem mci = selecteditem.Tag as MonitorConfigurationItem;
                if(MessageBox.Show("Are you sure you want to remove the item "  + mci.Title + "?","Confirm Remove",MessageBoxButtons.YesNo)==DialogResult.Yes)
                {
                    lvwMonitorItems.Items.Remove(selecteditem);     
                }
            }
            
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            cmdApply_Click(cmdApply,e);
            Hide();
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            var grabitems = config.GetItems();
            grabitems.Clear();
            foreach(ListViewItem lvi in lvwMonitorItems.Items)
            {
                MonitorConfigurationItem mci = lvi.Tag as MonitorConfigurationItem;
                grabitems.Add(mci);
            }
            config.Save();
            RestartMonitor();
        }

        private void lvwMonitorItems_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            ListViewItem changedItem = lvwMonitorItems.Items[e.Index];
            MonitorConfigurationItem mci = changedItem.Tag as MonitorConfigurationItem;
            bool ischecked = e.NewValue == CheckState.Checked;
            mci.Active = ischecked;
        }

        private void frmConfiguration_Shown(object sender, EventArgs e)
        {
            if (Loading) Hide();
        }

        private void frmConfiguration_VisibleChanged(object sender, EventArgs e)
        {

        }

        private void frmConfiguration_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason==CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                ni.ShowBalloonTip(5000,"BC Folder Monitor Still running","BASeCamp Folder Monitor is still monitoring folder changes.",ToolTipIcon.Info);
            }
        }

       

        

        

       
    }
}
