using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;


namespace ConfigTool
{
    //edit's any cloneable object. It needs to be cloneable, so that changes can be "cancelled"
    //I might make changes so it can accept serializable objects as well...



    public partial class ObjectPropertyEditor : Form
    {



        private Object mObjectEdit;
        private DialogResult mUseResult;
        public static void EditObject(IWin32Window parentobject, ref ICloneable objedit)
        {
            EditObject(parentobject, ref objedit, "Edit " + objedit.GetType().Name);

        }
        public static void EditObject(IWin32Window parentobject, ref ICloneable objedit, String Caption)
        {

            Object Editthis = objedit.Clone();
            ObjectPropertyEditor useeditor = new ObjectPropertyEditor(Editthis);
            useeditor.Text = Caption;
            System.Windows.Forms.DialogResult gotresult = useeditor.DoShow(parentobject);
            if (gotresult == System.Windows.Forms.DialogResult.OK)
            {
                objedit = (ICloneable)Editthis;

            }
            else if (gotresult == System.Windows.Forms.DialogResult.Cancel)
            {
                //unchanged

            }



        }
        public DialogResult DoShow(IWin32Window parent)
        {
            ShowDialog(parent);
            return mUseResult;



        }

        public ObjectPropertyEditor()
        {
            InitializeComponent();
        }
        public ObjectPropertyEditor(object objtoEdit)
            : this()
        {

            mObjectEdit = objtoEdit;
            GridEditor.SelectedObject = mObjectEdit;

        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            mUseResult = System.Windows.Forms.DialogResult.OK;
            Hide();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            mUseResult = System.Windows.Forms.DialogResult.Cancel;
            Hide();
        }

        private void ObjectPropertyEditor_Load(object sender, EventArgs e)
        {
            Size = Size;
        }


    }

    public class ObjectTypeEditor : UITypeEditor
    {

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            Debug.Print("In GetEditStyle for ObjectTypeEditor");

            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Debug.Print("ObjectTypeEditor.EditValue");
            IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                provider.GetService(typeof(IWindowsFormsEditorService));


            if (svc != null)
            {
                //svc.ShowDialog(new frmCreatorPropertiesEditor((CreatorProperties)value));
                svc.ShowDialog(new ObjectPropertyEditor(value));
                // update etc
            }
            return value;
        }


    }



}
