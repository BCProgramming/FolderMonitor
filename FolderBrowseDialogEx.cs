using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace System.Windows.Shell
{
    /// <summary>
    ///     Flags for the SHBrowseFolder function call.
    /// </summary>
    [Flags]
    public enum BrowseFlags
    {
        /// <summary>
        ///     BIF_RETURNONLYFSDIRS
        /// </summary>
        ReturnOnlyFSDirs = 0x0001,

        /// <summary>
        ///     BIF_DONTGOBELOWDOMAIN
        /// </summary>
        DontGoBelowDomain = 0x0002,

        /// <summary>
        ///     BIF_STATUSTEXT
        /// </summary>
        ShowStatusText = 0x0004,

        /// <summary>
        ///     BIF_RETURNFSANCESTORS
        /// </summary>
        ReturnFSancestors = 0x0008,

        /// <summary>
        ///     BIF_EDITBOX
        /// </summary>
        EditBox = 0x0010,

        /// <summary>
        ///     BIF_VALIDATE
        /// </summary>
        Validate = 0x0020,

        /// <summary>
        ///     BIF_NEWDIALOGSTYLE
        /// </summary>
        NewDialogStyle = 0x0040,

        /// <summary>
        ///     BIF_BROWSEINCLUDEURLS
        /// </summary>
        BrowseIncludeURLs = 0x0080,

        /// <summary>
        ///     BIF_UAHINT
        /// </summary>
        AddUsageHint = 0x0100,

        /// <summary>
        ///     BIF_NONEWFOLDERBUTTON
        /// </summary>
        NoNewFolderButton = 0x0200,

        /// <summary>
        ///     BIF_BROWSEFORCOMPUTER
        /// </summary>
        BrowseForComputer = 0x1000,

        /// <summary>
        ///     BIF_BROWSEFORPRINTER
        /// </summary>
        BrowseForPrinter = 0x2000,

        /// <summary>
        ///     BIF_BROWSEINCLUDEFILES
        /// </summary>
        IncludeFiles = 0x4000,

        /// <summary>
        ///     BIF_SHAREABLE
        /// </summary>
        ShowShareable = 0x8000
    }


    /// <summary>
    ///     Event information when the item selection changes.
    /// </summary>
    public class BrowseSelChangedEventArgs : EventArgs, IDisposable
    {
        internal BrowseSelChangedEventArgs(IntPtr selectedItemPidl)
        {
            SelectedItemPidl = selectedItemPidl;
        }

        /// <summary>
        ///     Return ITEMIDLIST for the currently selected folder
        /// </summary>
        public IntPtr SelectedItemPidl { get; private set; }

        /// <summary>
        ///     uses the PIDL and retrieves the string representation for the given path.
        /// </summary>
        public string SelectedPath
        {
            get
            {
                var path = new StringBuilder(260);
                NativeMethods.SHGetPathFromIDList(SelectedItemPidl, path);

                return path.ToString();
            }
        }

        public void Dispose()
        {
            NativeMethods.SHMemFree(SelectedItemPidl);
        }
    };

    /// <summary>
    ///     represents the IUnknown Message event.
    /// </summary>
    public class IUnknownObtainedEventArgs : EventArgs
    {
        internal IUnknownObtainedEventArgs(object siteUnknown)
        {
            SiteUnknown = siteUnknown;
        }
        public object SiteUnknown { get; private set; }
    }

    public class ValidationFailedEventArgs : EventArgs
    {
        internal ValidationFailedEventArgs(string invalidText)
        {
            Dismiss = false;
            InvalidText = invalidText;
        }

        /// <summary>
        ///     The text which called validation to fail
        /// </summary>
        public string InvalidText { get; private set; }

        /// <summary>
        ///     Sets whether the dialog needs to be dismissed or not
        /// </summary>
        public bool Dismiss { get; set; }
    }



    /// <summary>
    ///     Encapsulates the shell folder browse dialog shown by SHBrowseForFolder
    /// </summary>
    public class BrowseFolderDialogEx : Component
    {
        private const int WM_USER = 0x0400;
        private const int BFFM_SETSTATUSTEXTA = (WM_USER + 100);
        private const int BFFM_SETSTATUSTEXTW = (WM_USER + 104);
        private const int BFFM_ENABLEOK = (WM_USER + 101);
        private const int BFFM_SETSELECTIONA = (WM_USER + 102);
        private const int BFFM_SETSELECTIONW = (WM_USER + 103);
        private const int BFFM_SETOKTEXT = (WM_USER + 105);
        private const int BFFM_SETEXPANDED = (WM_USER + 106);
        private const int BFFM_INITIALIZED = 1;
        private const int BFFM_SELCHANGED = 2;
        private const int BFFM_VALIDATEFAILEDA = 3;
        private const int BFFM_VALIDATEFAILEDW = 4;
        private const int BFFM_IUNKNOWN = 5;
        private IntPtr handle;
        private IntPtr pidlReturned = IntPtr.Zero;
        private string title;

        /// <summary>
        ///     String displayed above the Dialog's treeview, intended for instructions. Can only be modified before displaying the
        ///     dialog.
        /// </summary>
        [Description("String displayed above the Dialog's treeview, intended for instructions. Can only be modified before displaying the dialog.")]
        public string Title
        {
            get { return title; }
            set
            {
                if (handle != IntPtr.Zero)
                    throw new InvalidOperationException();

                title = value;
            }
        }

        /// <summary>
        ///     retrieve display name of selected folder.
        /// </summary>
        [Description("The display name of the folder selected by the user")]
        public string FolderDisplayName { get; private set; }

        /// <summary>
        ///     retrieves The folder path that was selected
        /// </summary>
        public string FolderPath
        {
            get
            {
                if (pidlReturned == IntPtr.Zero)
                    return string.Empty;

                var pathReturned = new StringBuilder(260);

                NativeMethods.SHGetPathFromIDList(pidlReturned, pathReturned);
                return pathReturned.ToString();
            }
        }

        /// <summary>
        ///     Gets/Sets BIF behaviour flags.
        /// </summary>
        public BrowseFlags BrowseFlags { get; set; }

        private DialogResult ShowDialogInternal(ref BrowseInfo bi)
        {
            bi.title = title;
            bi.displayname = new string('\0', 260);
            bi.callback = BrowseCallback;
            bi.flags = (int)BrowseFlags;

            //Free any old pidls
            if (pidlReturned != IntPtr.Zero)
                NativeMethods.SHMemFree(pidlReturned);

            var ret = (pidlReturned = NativeMethods.SHBrowseForFolder(ref bi)) != IntPtr.Zero;

            if (ret)
            {
                FolderDisplayName = bi.displayname;
            }

            //Reset the handle
            handle = IntPtr.Zero;

            return ret ? DialogResult.OK : DialogResult.Cancel;
        }
        public static String DoBrowse(String pTitle, BrowseFlags pFlags, Action<Object, EventArgs> pInitialize = null, Action<Object, BrowseSelChangedEventArgs> pSelChanged = null,
            Action<Object, ValidationFailedEventArgs> pValidationFailure = null)
        {
            return DoBrowse(null, pTitle, pFlags, pInitialize, pSelChanged, pValidationFailure);
        }
        public static String DoBrowse(IWin32Window pOwner, String pTitle, BrowseFlags pFlags, Action<Object, EventArgs> pInitialize = null, Action<Object, BrowseSelChangedEventArgs> pSelChanged = null,
            Action<Object, ValidationFailedEventArgs> pValidationFailure = null)
        {
            BrowseFolderDialogEx bfd = new BrowseFolderDialogEx();
            bfd.Title = pTitle;
            bfd.BrowseFlags = pFlags;
            if (pInitialize != null) bfd.Initialized += (EventHandler)((ob, e) => { pInitialize(ob, e); });
            if (pSelChanged != null) bfd.SelChanged += (EventHandler<BrowseSelChangedEventArgs>)((ob, e) => { pSelChanged(ob, e); });
            if (pValidationFailure != null) bfd.ValidateFailed += (EventHandler<ValidationFailedEventArgs>)((ob, e) => { pValidationFailure(ob, e); });
            DialogResult result;
            if (pOwner != null)
                result = bfd.ShowDialog(pOwner);
            else
                result = bfd.ShowDialog();

            if (result == DialogResult.OK)
                return bfd.FolderPath;
            else
                return null;

        }
        /// <summary>
        ///     Shows the dialog
        /// </summary>
        /// <param name="owner">The window to use as the owner</param>
        /// <returns></returns>
        public DialogResult ShowDialog(IWin32Window owner)
        {
            if (handle != IntPtr.Zero)
                throw new InvalidOperationException();

            var bi = new BrowseInfo();

            if (owner != null)
                bi.hwndOwner = owner.Handle;

            return ShowDialogInternal(ref bi);
        }

        /// <summary>
        ///     Shows the dialog using active window as the owner
        /// </summary>
        public DialogResult ShowDialog()
        {
            return ShowDialog(Form.ActiveForm);
        }

        /// <summary>
        ///     Sets the text of the status area of the folder dialog
        /// </summary>
        /// <param name="text">Text to set</param>
        public void SetStatusText(string text)
        {
            if (handle == IntPtr.Zero)
                throw new InvalidOperationException();

            var msg = (Environment.OSVersion.Platform == PlatformID.Win32NT) ? BFFM_SETSTATUSTEXTW : BFFM_SETSTATUSTEXTA;
            var strptr = Marshal.StringToHGlobalAuto(text);

            NativeMethods.SendMessage(handle, msg, IntPtr.Zero, strptr);

            Marshal.FreeHGlobal(strptr);
        }

        /// <summary>
        ///     Enables or disables the ok button
        /// </summary>
        /// <param name="bEnable">true to enable false to diasble the OK button</param>
        public void EnableOkButton(bool bEnable)
        {
            if (handle == IntPtr.Zero)
                throw new InvalidOperationException();

            var lp = bEnable ? new IntPtr(1) : IntPtr.Zero;

            NativeMethods.SendMessage(handle, BFFM_ENABLEOK, IntPtr.Zero, lp);
        }

        /// <summary>
        ///     Sets the selection the text specified
        /// </summary>
        /// <param name="newsel">The path of the folder which is to be selected</param>
        public void SetSelection(string newsel)
        {
            if (handle == IntPtr.Zero)
                throw new InvalidOperationException();

            var msg = (Environment.OSVersion.Platform == PlatformID.Win32NT) ? BFFM_SETSELECTIONA : BFFM_SETSELECTIONW;

            var strptr = Marshal.StringToHGlobalAuto(newsel);

            NativeMethods.SendMessage(handle, msg, new IntPtr(1), strptr);

            Marshal.FreeHGlobal(strptr);
        }

        /// <summary>
        ///     Sets the text of the OK button in the dialog
        /// </summary>
        /// <param name="text">New text of the OK button</param>
        public void SetOkButtonText(string text)
        {
            if (handle == IntPtr.Zero)
                throw new InvalidOperationException();

            var strptr = Marshal.StringToHGlobalUni(text);

            NativeMethods.SendMessage(handle, BFFM_SETOKTEXT, new IntPtr(1), strptr);

            Marshal.FreeHGlobal(strptr);
        }

        /// <summary>
        ///     Expand a path in the folder
        /// </summary>
        /// <param name="path">The path to expand</param>
        public void SetExpanded(string path)
        {
            var strptr = Marshal.StringToHGlobalUni(path);

            NativeMethods.SendMessage(handle, BFFM_SETEXPANDED, new IntPtr(1), strptr);

            Marshal.FreeHGlobal(strptr);
        }

        /// <summary>
        ///     Fired when the dialog is initialized
        /// </summary>
        public event EventHandler Initialized;

        /// <summary>
        ///     Fired when selection changes
        /// </summary>
        public event EventHandler<BrowseSelChangedEventArgs> SelChanged;

        /// <summary>
        ///     Shell provides an IUnknown through this event. (For details see documentation of SHBrowseForFolder)
        /// </summary>
        public event EventHandler<IUnknownObtainedEventArgs> IUnknownObtained;

        /// <summary>
        ///     Fired when validation of text typed by user into the edit box fails.
        /// </summary>
        public event EventHandler<ValidationFailedEventArgs> ValidateFailed;

        private void FireInitialized(object sender, EventArgs e)
        {
            var copied = Initialized;
            if (copied != null) copied(sender, e);
        }

        private void FireSelChanged(Object sender, BrowseSelChangedEventArgs e)
        {
            var copied = SelChanged;
            if (copied != null) copied(sender, e);
        }

        private void FireIUnknownObtained(Object sender, IUnknownObtainedEventArgs e)
        {
            var copied = IUnknownObtained;
            if (copied != null) copied(sender, e);
        }

        private void FireValidateFailed(Object sender, ValidationFailedEventArgs e)
        {
            var copied = ValidateFailed;
            if (copied != null) copied(sender, e);
        }

        private int BrowseCallback(IntPtr hwnd, int msg, IntPtr lp, IntPtr lpData)
        {
            var ret = 0;

            switch (msg)
            {
            case BFFM_INITIALIZED:
                handle = hwnd;
                FireInitialized(this, null);

                break;
            case BFFM_IUNKNOWN:
                if (IUnknownObtained != null)
                {
                    if (lp != IntPtr.Zero)
                    {
                        var obtained = Marshal.GetObjectForIUnknown(lp);
                        if (obtained != null)
                            FireIUnknownObtained(this, new IUnknownObtainedEventArgs(obtained));
                    }
                }
                break;
            case BFFM_SELCHANGED:
                if (SelChanged != null)
                {
                    var e = new BrowseSelChangedEventArgs(lp);
                    FireSelChanged(this, e);
                }
                break;
            case BFFM_VALIDATEFAILEDA:
                if (ValidateFailed != null)
                {
                    var e = new ValidationFailedEventArgs(Marshal.PtrToStringAnsi(lpData));
                    FireValidateFailed(this, e);
                    ret = (e.Dismiss) ? 0 : 1;
                }
                break;
            case BFFM_VALIDATEFAILEDW:
                if (ValidateFailed != null)
                {
                    var e = new ValidationFailedEventArgs(Marshal.PtrToStringUni(lpData));
                    FireValidateFailed(this, e);
                    ret = (e.Dismiss) ? 0 : 1;
                }
                break;
            }

            return ret;
        }

        protected override void Dispose(bool disposing)
        {
            if (pidlReturned != IntPtr.Zero)
            {
                NativeMethods.SHMemFree(pidlReturned);
                pidlReturned = IntPtr.Zero;
            }
        }
    }

    internal delegate int BrowseCallBackProc(IntPtr hwnd, int msg, IntPtr lp, IntPtr wp);

    [StructLayout(LayoutKind.Sequential)]
    internal struct BrowseInfo
    {
        public IntPtr hwndOwner;
        public IntPtr pidlRoot;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string displayname;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string title;
        public int flags;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public BrowseCallBackProc callback;
        public IntPtr lparam;
    }

    [ComImport]
    [Guid("00000002-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMalloc
    {
        [PreserveSig]
        IntPtr Alloc(IntPtr cb);

        [PreserveSig]
        IntPtr Realloc(IntPtr pv, IntPtr cb);

        [PreserveSig]
        void Free(IntPtr pv);

        [PreserveSig]
        IntPtr GetSize(IntPtr pv);

        [PreserveSig]
        int DidAlloc(IntPtr pv);

        [PreserveSig]
        void HeapMinimize();
    }

    /// <summary>
    ///     A class that defines all the native/unmanaged methods used
    /// </summary>
    internal class NativeMethods
    {
        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SHBrowseForFolder(ref BrowseInfo bi);

        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SHGetPathFromIDList(IntPtr pidl, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszPath);

        [DllImport("User32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SendMessage(IntPtr hwnd, int msg, IntPtr wp, IntPtr lp);

        [DllImport("Shell32.dll")]
        internal static extern int SHGetMalloc([MarshalAs(UnmanagedType.IUnknown)] out object shmalloc);

        //Helper routine
        internal static void SHMemFree(IntPtr ptr)
        {
            object shmalloc = null;

            if (SHGetMalloc(out shmalloc) == 0)
            {
                var malloc = (IMalloc)shmalloc;

                (malloc).Free(ptr);
            }
        }
    }
}