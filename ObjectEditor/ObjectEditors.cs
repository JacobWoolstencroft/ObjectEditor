using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ObjectEditor.Attributes;

namespace ObjectEditor
{
    public static class ObjectEditors
    {
        public delegate List<T> ImportListDelegate<T>();
        public delegate void ExportListDelegate<T>(List<T> list);

        public delegate Dictionary<string, T> ImportDictionaryDelegate<T>();
        public delegate void ExportDictionaryDelegate<T>(Dictionary<string, T> dict);


        #region public static ShowObjectEditor()
        /// <summary>
        /// Allows the user to directly edit an object's fields that have been tagged with EditableField including fields inside of fields that were tagged with EditableSubField
        /// </summary>
        /// <param name="Title">Title to display on the form</param>
        /// <param name="ob">The object that will be editted.</param>
        /// <param name="Editable">Indicates whether the object is editable or only viewable</param>
        /// <param name="StringLists">The string lists used to populate dropdown boxes.  Keys are defined in the object definition by specifying <see cref="EditableFieldAttribute.DropDownListKey"/></param>
        /// <param name="ObjectLists">The object lists that will be used when populating dropdown boxes when <see cref="StringModes.DropDownObjectList"/> is specified.</param>
        /// <returns></returns>
        public static DialogResult ShowObjectEditor(string Title, object ob, bool Editable, Dictionary<string, List<string>> StringLists = null, Dictionary<string, List<object>> ObjectLists = null)
        {
            ObjectEditorInfo editorInfo = new ObjectEditorInfo();
            editorInfo.Editable = Editable;
            editorInfo.StringLists = StringLists;
            editorInfo.ObjectLists = ObjectLists;

            return ShowObjectEditor(Title, ob, editorInfo);
        }
        /// <summary>
        /// Allows the user to directly edit an object's fields that have been tagged with EditableField including fields inside of fields that were tagged with EditableSubField
        /// </summary>
        /// <param name="Title">Title to display on the form</param>
        /// <param name="ob">The object that will be editted.</param>
        /// <param name="editorInfo">Data used to populate dropdowns and other settings not directly available from the edited object itself.</param>
        /// <returns></returns>
        public static DialogResult ShowObjectEditor(string Title, object ob, ObjectEditorInfo editorInfo)
        {
            if (ob == null)
                return DialogResult.Abort;
            if (editorInfo == null)
                editorInfo = new ObjectEditorInfo();

            EditorField.CreateAutoFieldsForObject(ob, out List<EditorField> Fields, ModesFlags.Object, out List<string> PreferredCategoryOrder, editorInfo);

            DialogResult result = ShowEditor(Title, Fields, ob, editorInfo, PreferredCategoryOrder);
            if (result != DialogResult.OK)
                EditorField.ResetFieldValues(Fields, ob);
            return result;
        }
        internal static DialogResult ShowEditor(string Title, List<EditorField> editorFields, object ObjectBeingEdited, ObjectEditorInfo editorInfo, List<string> PreferredCategoryOrder = null)
        {
            if (editorFields == null)
                return DialogResult.Abort;

            using (frmObjectEditor f = new frmObjectEditor(Title, editorFields, ObjectBeingEdited, PreferredCategoryOrder, editorInfo))
            {
                DialogResult result = f.ShowDialog();
                return result;
            }
        }
        #endregion

        #region public static ShowDictionaryEditor()
        /// <summary>
        /// Displays a list of objects that can be added, edited, and deleted from.  The fields of the objects will be displayed if they are tagged with EditableField.  Sub fields will also be displayed if they're tagged with EditableSubField.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectDict">The list of objects to be edited.  Note: The contents of the list will be changed even if cancel is selected.  It is recommended to pass in a cloned list.</param>
        /// <param name="Editable">If false, the list of objects is displayed in read-only mode.</param>
        /// <param name="importFunction">If passed, an import button will be displayed.  Clicking the button will call the import function.  If the function returns non-null, the list will be replaced with the result from the import function.</param>
        /// <param name="exportFunction">If passed, an export button will be displayed.  Clicking the button will call the export function with the current list as a parameter.</param>
        /// <returns></returns>
        public static DialogResult ShowDictionaryEditor<T>(string Title, Dictionary<string, T> objectDict, bool Editable, Dictionary<string, List<string>> StringLists = null, Dictionary<string, List<object>> ObjectLists = null, ImportDictionaryDelegate<T> importFunction = null, ExportDictionaryDelegate<T> exportFunction = null) where T : ICloneable, new()
        {
            ObjectEditorInfo editorInfo = new ObjectEditorInfo();
            editorInfo.StringLists = StringLists;
            editorInfo.ObjectLists = ObjectLists;
            editorInfo.Editable = Editable;

            using (frmObjectDictionaryEditor<T> f = new frmObjectDictionaryEditor<T>(Title, objectDict, editorInfo, importFunction, exportFunction))
            {
                return f.ShowDialog();
            }
        }

        /// <summary>
        /// Displays a list of objects that can be added, edited, and deleted from.  The fields of the objects will be displayed if they are tagged with EditableField.  Sub fields will also be displayed if they're tagged with EditableSubField.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectDict">The list of objects to be edited.  Note: The contents of the list will be changed even if cancel is selected.  It is recommended to pass in a cloned list.</param>
        /// <param name="importFunction">If passed, an import button will be displayed.  Clicking the button will call the import function.  If the function returns non-null, the list will be replaced with the result from the import function.</param>
        /// <param name="exportFunction">If passed, an export button will be displayed.  Clicking the button will call the export function with the current list as a parameter.</param>
        /// <returns></returns>
        public static DialogResult ShowDictionaryEditor<T>(string Title, Dictionary<string, T> objectDict, ObjectEditorInfo editorInfo, ImportDictionaryDelegate<T> importFunction = null, ExportDictionaryDelegate<T> exportFunction = null) where T : ICloneable, new()
        {
            if (editorInfo == null)
                editorInfo = new ObjectEditorInfo();
            using (frmObjectDictionaryEditor<T> f = new frmObjectDictionaryEditor<T>(Title, objectDict, editorInfo, importFunction, exportFunction))
            {
                return f.ShowDialog();
            }
        }
        #endregion

        #region public static ShowListEditor()
        /// <summary>
        /// Displays a list of objects that can be added, edited, and deleted from.  The fields of the objects will be displayed if they are tagged with EditableField.  Sub fields will also be displayed if they're tagged with EditableSubField.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectList">The list of objects to be edited.  Note: The contents of the list will be changed even if cancel is selected.  It is recommended to pass in a cloned list.</param>
        /// <param name="importFunction">If passed, an import button will be displayed.  Clicking the button will call the import function.  If the function returns non-null, the list will be replaced with the result from the import function.</param>
        /// <param name="exportFunction">If passed, an export button will be displayed.  Clicking the button will call the export function with the current list as a parameter.</param>
        /// <returns></returns>
        public static DialogResult ShowListEditor<T>(string Title, List<T> objectList, bool Editable, Dictionary<string, List<string>> StringLists = null, Dictionary<string, List<object>> ObjectLists = null, ImportListDelegate<T> importFunction = null, ExportListDelegate<T> exportFunction = null) where T : ICloneable, new()
        {
            ObjectEditorInfo editorInfo = new ObjectEditorInfo();
            editorInfo.StringLists = StringLists;
            editorInfo.ObjectLists = ObjectLists;
            editorInfo.Editable = Editable;

            using (frmObjectListEditor<T> f = new frmObjectListEditor<T>(Title, objectList, editorInfo, importFunction, exportFunction))
            {
                return f.ShowDialog();
            }
        }

        /// <summary>
        /// Displays a list of objects that can be added, edited, and deleted from.  The fields of the objects will be displayed if they are tagged with EditableField.  Sub fields will also be displayed if they're tagged with EditableSubField.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectList">The list of objects to be edited.  Note: The contents of the list will be changed even if cancel is selected.  It is recommended to pass in a cloned list.</param>
        /// <param name="importFunction">If passed, an import button will be displayed.  Clicking the button will call the import function.  If the function returns non-null, the list will be replaced with the result from the import function.</param>
        /// <param name="exportFunction">If passed, an export button will be displayed.  Clicking the button will call the export function with the current list as a parameter.</param>
        /// <returns></returns>
        public static DialogResult ShowListEditor<T>(string Title, List<T> objectList, ObjectEditorInfo editorInfo, ImportListDelegate<T> importFunction = null, ExportListDelegate<T> exportFunction = null) where T : ICloneable, new()
        {
            if (editorInfo == null)
                editorInfo = new ObjectEditorInfo();
            using (frmObjectListEditor<T> f = new frmObjectListEditor<T>(Title, objectList, editorInfo, importFunction, exportFunction))
            {
                return f.ShowDialog();
            }
        }
        #endregion
    }
}
