using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
namespace ConfigTool
{
    class PropertyGridCustom : PropertyGrid
    {
        public PropertyGridCustom()
        {

            this.ToolStripRenderer = ToolStripManager.Renderer;

        }

    }
    public class CustomCollectionForm : CollectionEditor
    {
        public CustomCollectionForm(Type ptypeof)
            : base(ptypeof)
        {

        }
        
        protected override CollectionForm CreateCollectionForm()
        {
            
            //Get a reference top new collection form
            CollectionEditor.CollectionForm form = base.CreateCollectionForm();

            //Center the form 
            form.StartPosition = FormStartPosition.CenterParent;

            //Get the forms type
            Type formType = form.GetType();

            //Get a reference to the private fieldtype propertyBrowser
            //This is the propertgrid inside the collectionform
            FieldInfo fieldInfo = formType.GetField("propertyBrowser", BindingFlags.NonPublic | BindingFlags.Instance);

            if (fieldInfo != null)
            {

                //get a reference to the propertygrid instance located on the form
                PropertyGrid propertyGrid = (PropertyGrid)fieldInfo.GetValue(form);

                if (propertyGrid != null)
                {

                    //Make the tool bar visible
                    propertyGrid.ToolbarVisible = true;

                    //Make the help/description visible
                    propertyGrid.HelpVisible = true;

                    //Get the property grid's type.
                    //Note that this is a vsPropertyGrid located in System.Windows.Forms.Design
                    Type propertyGridType = propertyGrid.GetType();

                    //Get a reference to the non-public property ToolStripRenderer
                    PropertyInfo propertyInfo = propertyGridType.GetProperty("ToolStripRenderer", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (propertyInfo != null)
                    {

                        propertyInfo.SetValue(propertyGrid, ToolStripManager.Renderer, null);
                    }
                }
            }

            //Return the form
            return form;
        }

    }
    public abstract class BaseMultiEditor : CustomCollectionForm
    {

        public delegate void OnEditCompleteFunc(Object[] completededit);
        public static event OnEditCompleteFunc OnEditComplete;

        protected BaseMultiEditor(Type ptypeof)
            : base(ptypeof)
        {


        }

        public void InvokeEditComplete(Object[] finishwith)
        {
            OnEditCompleteFunc copyto = OnEditComplete;
            if (copyto != null)
            {
                copyto.Invoke(finishwith);



            }
        }

        protected override bool CanSelectMultipleInstances()
        {
            return true;
        }
        protected override object SetItems(object editValue, object[] value)
        {
            if (editValue != null)
            {
                Debug.Print("in TriggerCollectionEditor::SetItems- editvalue type=" + editValue.GetType().FullName +
                            " count of values:" + value.Length.ToString());
                if (value.Length > 3)
                {
                    Debug.Print("greater than 3");

                }
            }
            InvokeEditComplete(value);
            return base.SetItems(editValue, value);

        }

    }
    public class GenericCollectionEditor<T> : BaseMultiEditor
    {
        public GenericCollectionEditor(Type ptypeof)
            : base(ptypeof)
        {


        }
        protected override Type[] CreateNewItemTypes()
        {
            return new Type[] { typeof(T) };
        }
        protected override string GetDisplayText(object value)
        {
            return value.ToString();
        }
    }
}
