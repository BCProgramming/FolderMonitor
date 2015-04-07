using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FolderMonitor
{
    public class MonitorConfigurationItem
    {
        private String _Title;
        private String _MonitorPath = "";
        private bool _Subdirectories = false;
        private String _Filter = "*";
        private bool _Active = true;
        public String Title { get { return _Title; } set { _Title = value; } }
        public String MonitorPath { get { return _MonitorPath; } set { _MonitorPath = value; } }
        public bool Subdirectories { get { return _Subdirectories; } set { _Subdirectories = value; } }

        public bool Active { get { return _Active; } set { _Active = value; } }
        public String Filter { get { return _Filter; } set { _Filter = value; } }

        public MonitorConfigurationItem()
        {

        }
        public MonitorConfigurationItem(XElement Source):this()
        {
            foreach(XAttribute attrib in Source.Attributes())
            {
                if (attrib.Name.ToString().Equals("Path", StringComparison.OrdinalIgnoreCase))
                    _MonitorPath = attrib.Value;
                else if (attrib.Name.ToString().Equals("Subdirs", StringComparison.OrdinalIgnoreCase))
                {
                    bool.TryParse(attrib.Value, out _Subdirectories);
                }
                else if (attrib.Name.ToString().Equals("Filter", StringComparison.OrdinalIgnoreCase))
                {
                    _Filter = attrib.Value;
                }

            }
        }
        public XElement Save()
        {
            XElement buildelement = new XElement("MonitorItem", new XAttribute("Path", _MonitorPath), new XAttribute("Subdirs", _Subdirectories), new XAttribute("Filter", _Filter), new XAttribute("Active", _Active));
            return buildelement;
        }
    }
    public class MonitorConfiguration
    {
        public static String DefaultDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "BASeCamp", "BCFolderMonitor");
        public static String DefaultDataFile = Path.Combine(DefaultDataPath, "Monitor.config");
        private List<MonitorConfigurationItem> ConfigurationElements = new List<MonitorConfigurationItem>();
        public static MonitorConfiguration Static = new MonitorConfiguration(DefaultDataFile);
        private String sSource;
        public MonitorConfiguration()
        {

        }
        public IList<MonitorConfigurationItem> GetItems()
        {
            return ConfigurationElements;
        }
        public MonitorConfiguration(String sFilePath):this()
        {
            if(File.Exists(sFilePath))
            {
                var acquiredoc = XDocument.Load(sFilePath);
                LoadItems(acquiredoc);
            }
            sSource = sFilePath;
        }
        private void LoadItems(XDocument docSource)
        {
            ConfigurationElements = new List<MonitorConfigurationItem>();
            XElement RootNode = docSource.Root;
            if (RootNode == null) return;
           

            foreach (XElement MonitorItem in RootNode.Descendants("MonitorItem"))
            {
                MonitorConfigurationItem mci = new MonitorConfigurationItem(MonitorItem);
                ConfigurationElements.Add(mci);
            }
        }
        public MonitorConfiguration(XDocument docSource)
        {
            LoadItems(docSource);
        }
        public void Save()
        {
            Save(sSource);
        }
        public XDocument SaveNode()
        {
            XElement Root = new XElement("Monitor");
            foreach (MonitorConfigurationItem mci in ConfigurationElements)
            {
                Root.Add(mci.Save());
            }
            return new XDocument(Root);
        }
        public void Save(String sFilePath)
        {
            XDocument xsaved = SaveNode();
            if (!Directory.Exists(Path.GetDirectoryName(sFilePath))) Directory.CreateDirectory(Path.GetDirectoryName(sFilePath));
            xsaved.Save(sFilePath);
        }

    }
}
