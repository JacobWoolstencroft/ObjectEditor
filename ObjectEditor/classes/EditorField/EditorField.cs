using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ObjectEditor.Attributes;

namespace ObjectEditor
{
    internal abstract class EditorField
    {
        public const char PassChar = '\u25cf';
        public const string DEFAULT_CATEGORY = "General";
        public const string DEFAULT_NULL_VALUE_DESCRIPTOR = "";
        private const BindingFlags MemberBinding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public string Category;
        public string Description;
        public string ListDescription;
        public string ToolTipText;
        internal FieldData VisibilityFlagMember = null;
        internal FieldData EnabledFlagMember = null;
        internal EditorSubField ParentField = null;
        public double SortIndex;

        public bool IsVisible(object ObjectBeingEditted)
        {
            if (ParentField != null && !ParentField.IsVisible(ObjectBeingEditted))
                return false;
            if (VisibilityFlagMember != null)
            {
                return (bool)VisibilityFlagMember.GetValue(ObjectBeingEditted);
            }
            return true;
        }

        public abstract void UpdateCellValue(DataGridViewCell cell, object ObjectBeingEditted);
        protected abstract DataGridViewCell MakeDataGridViewCell(object ObjectBeingEditted);
        public abstract bool VisibleOnList();
        public abstract string ListValue(object ObjectBeingEditted);

        public DataGridViewCell CreateDataGridViewCell(object ObjectBeingEditted)
        {
            DataGridViewCell cell = MakeDataGridViewCell(ObjectBeingEditted);
            UpdateCellValue(cell, ObjectBeingEditted);
            return cell;
        }

        internal class DefaultEditorFieldValues
        {
            public DefaultEditorFieldValues()
            {
            }
            public DefaultEditorFieldValues(DefaultEditorFieldValues copyFrom)
            {
                this.DefaultCategory = copyFrom.DefaultCategory;
                this.NullValueDescriptor = copyFrom.NullValueDescriptor;
                this.EmptyStringMode = copyFrom.EmptyStringMode;
                this.StringMode = copyFrom.StringMode;
            }

            public string DefaultCategory = DEFAULT_CATEGORY;
            public string NullValueDescriptor = DEFAULT_NULL_VALUE_DESCRIPTOR;
            public EmptyStringModes EmptyStringMode = EmptyStringModes.Blank;
            public StringModes StringMode = StringModes.NoChange;
        }
        public static void GetDefaults(Type ParentType, out DefaultEditorFieldValues defaults, out List<string> PreferredCategoryOrder)
        {
            PreferredCategoryOrder = new List<string>();
            defaults = new DefaultEditorFieldValues();

            EditableObjectAttribute editableObjectAttribute = ParentType.GetCustomAttribute<EditableObjectAttribute>();
            if (editableObjectAttribute != null)
            {
                if (editableObjectAttribute.PreferredCategoryOrder != null)
                    PreferredCategoryOrder.AddRange(editableObjectAttribute.PreferredCategoryOrder);
                if (editableObjectAttribute.Category != null)
                    defaults.DefaultCategory = editableObjectAttribute.Category;
                if (editableObjectAttribute.EmptyStringMode != EmptyStringModes.Unspecified)
                    defaults.EmptyStringMode = editableObjectAttribute.EmptyStringMode;
                if (editableObjectAttribute.StringMode != StringModes.Unspecified)
                    defaults.StringMode = editableObjectAttribute.StringMode;
                if (editableObjectAttribute.NullValueDescriptor != null)
                    defaults.NullValueDescriptor = editableObjectAttribute.NullValueDescriptor;
            }
        }
        public static void CreateAutoFieldsForObject(object ob, out List<EditorField> EditorFields, ModesFlags CurrentMode, out List<string> PreferredCategoryOrder, ObjectEditorInfo editorInfo)
        {
            GetDefaults(ob.GetType(), out DefaultEditorFieldValues defaults, out PreferredCategoryOrder);

            CreateFields(ob.GetType(), out EditorFields, defaults, CurrentMode, editorInfo);
            EditorField.SaveFieldValues(EditorFields, ob);
        }

        public static void CreateFields(Type ParentType, out List<EditorField> EditorFields, DefaultEditorFieldValues defaults, ModesFlags CurrentMode, ObjectEditorInfo editorInfo)
        {
            EditorFields = new List<EditorField>();

            List<FieldInfo> ParentFields = new List<FieldInfo>();
            foreach (MemberInfo member in ParentType.GetMembers(MemberBinding))
            {
                if (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Method)
                    CreateFields(EditorFields, CurrentMode, defaults, null, ParentType, new List<MemberInfo>(), member, editorInfo);
            }
            EditorFields = EditorFields.OrderBy(field => field.SortIndex).ToList();
        }
        private static void CreateFields(List<EditorField> EditorFields, ModesFlags CurrentMode, DefaultEditorFieldValues defaults, EditorSubField ParentField, Type ParentType, List<MemberInfo> ParentMembers, MemberInfo member, ObjectEditorInfo editorInfo)
        {
            Type memberType = null;
            if (member is FieldInfo field)
                memberType = field.FieldType;
            else if (member is PropertyInfo property)
                memberType = property.PropertyType;
            else if (member is MethodInfo method)
                memberType = typeof(void);
            else
                return;

            foreach (ObjectEditorMemberAttribute attribute in member.GetCustomAttributes<ObjectEditorMemberAttribute>())
            {
                string category = attribute.Category;
                string Description = attribute.Description;
                double sortIndex = attribute.SortIndex;
                string ListDescription;

                if (Description == null)
                    Description = member.Name;
                ListDescription = Description;

                Description = attribute.Description_Edit_Prefix + Description + attribute.Description_Edit_Postfix;
                ListDescription = attribute.Description_List_Prefix + ListDescription + attribute.Description_List_Postfix;

                if (category == null)
                    category = defaults.DefaultCategory;
                if (category == null)
                    category = DEFAULT_CATEGORY;
                if (((int)attribute.VisibleModes & (int)CurrentMode) != (int)CurrentMode)
                    continue;



                try
                {
                    EditorField editorField = null;
                    FieldData VisibilityField = FindBoolFlag("VisibilityFlagMember", ParentType, ParentMembers, attribute.VisibilityFlagMember);

                    if (attribute is EditableSubFieldAttribute subFieldAttribute)
                    {
                        DefaultEditorFieldValues subFieldDefaults = new DefaultEditorFieldValues(defaults);
                        if (subFieldAttribute.Category != null)
                            subFieldDefaults.DefaultCategory = subFieldAttribute.Category;

                        List<MemberInfo> members = new List<MemberInfo>(ParentMembers);
                        members.Add(member);

                        EditorSubField subField = new EditorSubField(ParentField, VisibilityField);

                        foreach (MemberInfo subMember in memberType.GetMembers(MemberBinding))
                        {
                            if (subMember.MemberType == MemberTypes.Property || subMember.MemberType == MemberTypes.Field || subMember.MemberType == MemberTypes.Method)
                                CreateFields(EditorFields, CurrentMode, subFieldDefaults, subField, memberType, members, subMember, editorInfo);
                        }
                    }
                    else if (attribute is ListEditorAttribute listEditorAttribute)
                    {
                        EmptyListModes emptyListMode = listEditorAttribute.EmptyListMode;

                        FieldData ValueField = new FieldData(ParentMembers, member);

                        FieldData DisplayField = FindField("DisplayField", ParentType, ParentMembers, listEditorAttribute.DisplayField);

                        editorField = CreateListEditorField(memberType, Description, category, sortIndex, emptyListMode, ValueField, DisplayField, editorInfo);
                    }
                    else if (attribute is EditableDropDownFieldAttribute dropDown)
                    {
                        string nullValueDescriptor = defaults.NullValueDescriptor;

                        FieldData ValueField = new FieldData(ParentMembers, member);
                        FieldData ValidateField = FindBoolFlag("ValidateField", ParentType, ParentMembers, dropDown.ValidateField);
                        FieldData OptionsField = FindListField("OptionsField", memberType, ParentType, ParentMembers, dropDown.OptionsField);

                        MethodData OnChangeMethod = FindMethod("OnChangeMethod", ParentType, ParentMembers, dropDown.OnChangeMethod);

                        EditorValueField valueField = CreateEditorDropDownField(memberType, Description, category, sortIndex, OptionsField, nullValueDescriptor, ValueField, editorInfo);
                        valueField.OnChangeMethod = OnChangeMethod;
                        valueField.ValidateField = ValidateField;
                        valueField.ToolTipText = dropDown.ToolTipText;

                        editorField = valueField;
                    }
                    else if (attribute is EditableFieldAttribute editableField)
                    {
                        EmptyStringModes emptyStringMode = defaults.EmptyStringMode;
                        string nullValueDescriptor = defaults.NullValueDescriptor;
                        StringModes stringMode = defaults.StringMode;
                        string dropDownListKey = editableField.DropDownListKey;

                        if (editableField.EmptyStringMode != EmptyStringModes.Unspecified)
                            emptyStringMode = editableField.EmptyStringMode;
                        if (editableField.StringMode != StringModes.Unspecified)
                            stringMode = editableField.StringMode;
                        if (editableField.NullValueDescriptor != null)
                            nullValueDescriptor = editableField.NullValueDescriptor;

                        FieldData ValueField = new FieldData(ParentMembers, member);
                        FieldData ValidateField = FindBoolFlag("ValidateField", ParentType, ParentMembers, editableField.ValidateField);
                        FieldData StringOptionsField = FindStringListField("StringOptionsField", ParentType, ParentMembers, editableField.StringOptionsField);

                        MethodData OnChangeMethod = FindMethod("OnChangeMethod", ParentType, ParentMembers, editableField.OnChangeMethod);
                        MethodData OnButtonClickMethod = FindMethod("OnButtonClickMethod", ParentType, ParentMembers, editableField.OnButtonClickMethod);

                        EditorValueField valueField = CreateEditorValueField(memberType, Description, category, sortIndex, emptyStringMode, stringMode, dropDownListKey, nullValueDescriptor, ValueField, editorInfo, StringOptionsField);
                        valueField.OnChangeMethod = OnChangeMethod;
                        valueField.OnButtonClickMethod = OnButtonClickMethod;
                        valueField.ValidateField = ValidateField;
                        valueField.ButtonText = editableField.ButtonText;
                        valueField.ToolTipText = editableField.ToolTipText;

                        editorField = valueField;
                    }
                    else if (attribute is ClickableButtonMethodAttribute buttonAttribute)
                    {
                        if (!(member is MethodInfo method))
                            throw new Exception(typeof(ClickableButtonMethodAttribute).Name + " expects a method");

                        string text = buttonAttribute.Text;

                        if (text == null)
                            text = Description;

                        MethodData Method = new MethodData(ParentMembers, method);

                        FieldData DisplayField = FindField("DisplayField", ParentType, ParentMembers, buttonAttribute.DisplayField);
                        FieldData EnabledField = FindBoolFlag("EnabledFlagMember", ParentType, ParentMembers, buttonAttribute.EnabledFlagMember);

                        if (method.GetParameters().Length != 0)
                            throw new Exception("Method " + method.Name + " must have 0 parameters");
                        if (method.ReturnType != typeof(void))
                            throw new Exception("Method " + method.Name + "() does not return void");

                        editorField = new EditorButtonMethodField(Description, category, sortIndex, text, Method, DisplayField);

                        editorField.EnabledFlagMember = EnabledField;
                        editorField.ToolTipText = buttonAttribute.ToolTipText;
                    }
                    if (editorField != null)
                    {
                        editorField.Description = Description;
                        editorField.ListDescription = ListDescription;
                        editorField.Category = category;
                        editorField.SortIndex = sortIndex;
                        editorField.VisibilityFlagMember = VisibilityField;
                        editorField.ParentField = ParentField;
                        EditorFields.Add(editorField);
                    }
                }
                catch (Exception ex)
                {
                    EditorField errorField = new EditorLabelField(Description, category, sortIndex, "ERROR: " + ex.Message);
                    EditorFields.Add(errorField);
                }
            }
        }

        private static FieldData FindBoolFlag(string FlagName, Type ParentType, List<MemberInfo> ParentMembers, string MemberName)
        {
            if (string.IsNullOrEmpty(MemberName))
                return null;

            MemberInfo[] members = ParentType.GetMember(MemberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.GetProperty);
            if (members == null || members.Length < 1)
                throw new Exception(FlagName + " " + ParentType.FullName + "." + MemberName + " not found");
            if (members[0] is PropertyInfo property && property.PropertyType != typeof(bool))
                throw new Exception(FlagName + " " + ParentType.FullName + "." + MemberName + " is not bool");
            if (members[0] is FieldInfo field && field.FieldType != typeof(bool))
                throw new Exception(FlagName + " " + ParentType.FullName + "." + MemberName + " is not bool");
            return new FieldData(ParentMembers, members[0]);
        }
        private static FieldData FindStringListField(string FieldName, Type ParentType, List<MemberInfo> ParentMembers, string MemberName)
        {
            if (string.IsNullOrEmpty(MemberName))
                return null;

            MemberInfo[] members = ParentType.GetMember(MemberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.GetProperty);
            if (members == null || members.Length < 1)
                throw new Exception(FieldName + " " + ParentType.FullName + "." + MemberName + " not found");
            if (members[0] is PropertyInfo property && property.PropertyType != typeof(List<string>) && property.PropertyType != typeof(string[]))
                throw new Exception(FieldName + " " + ParentType.FullName + "." + MemberName + " is not a List<string> or string[]");
            if (members[0] is FieldInfo field && field.FieldType != typeof(List<string>) && field.FieldType != typeof(string[]))
                throw new Exception(FieldName + " " + ParentType.FullName + "." + MemberName + " is not a List<string> or string[]");
            return new FieldData(ParentMembers, members[0]);
        }
        private static FieldData FindListField(string FieldName, Type memberType, Type ParentType, List<MemberInfo> ParentMembers, string MemberName)
        {
            if (string.IsNullOrEmpty(MemberName))
                return null;

            MemberInfo[] members = ParentType.GetMember(MemberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.GetProperty);
            if (members == null || members.Length < 1)
                throw new Exception(FieldName + " " + ParentType.FullName + "." + MemberName + " not found");

            if (members[0] is PropertyInfo property)
            {
                if (property.PropertyType == typeof(List<>).MakeGenericType(memberType) || property.PropertyType == memberType.MakeArrayType())
                    return new FieldData(ParentMembers, members[0]);
                throw new Exception(FieldName + " " + ParentType.FullName + "." + MemberName + " is not a List<" + memberType.Name + "> or " + memberType.Name + "[]");
            }
            else if (members[0] is FieldInfo field)
            {
                if (field.FieldType == typeof(List<>).MakeGenericType(memberType) || field.FieldType == memberType.MakeArrayType())
                    return new FieldData(ParentMembers, members[0]);
                throw new Exception(FieldName + " " + ParentType.FullName + "." + MemberName + " is not a List<" + memberType.Name + "> or " + memberType.Name + "[]");
            }

            throw new Exception(FieldName + " " + ParentType.FullName + "." + MemberName + " is not a readable Field or Property");
        }
        private static FieldData FindField(string AttributeFieldName, Type ParentType, List<MemberInfo> ParentMembers, string MemberName)
        {
            if (string.IsNullOrEmpty(MemberName))
                return null;

            foreach (MemberInfo otherMember in ParentType.GetMember(MemberName, MemberBinding))
            {
                if (otherMember is PropertyInfo property && !property.CanRead)
                    throw new Exception(AttributeFieldName + " " + ParentType.FullName + "." + MemberName + " can not be read");

                return new FieldData(ParentMembers, otherMember);
            }

            throw new Exception(AttributeFieldName + " " + ParentType.FullName + "." + MemberName + " not found");
        }
        private static MethodData FindMethod(string AttributeMethodName, Type ParentType, List<MemberInfo> ParentMembers, string MemberName)
        {
            if (string.IsNullOrEmpty(MemberName))
                return null;

            MethodInfo methodInfo = ParentType.GetMethod(MemberName, MemberBinding, null, Type.EmptyTypes, null);
            if (methodInfo == null)
                throw new Exception(AttributeMethodName + " " + ParentType.FullName + "." + MemberName + "() not found");

            if (methodInfo.GetParameters().Length != 0) //This is mearly a sanity check to make sure we don't accidentally return a method that has parameters
                throw new Exception("Method " + methodInfo.Name + " must have 0 parameters");
            if (methodInfo.ReturnType != typeof(void))
                throw new Exception("Method " + methodInfo.Name + "() does not return void");

            return new MethodData(ParentMembers, methodInfo);
        }

        private static EditorValueField CreateEditorValueField(Type memberType, string Description, string Category, double SortIndex, EmptyStringModes EmptyStringMode, StringModes StringMode, string DropDownListKey, string NullValueDescriptor, FieldData ValueField, ObjectEditorInfo editorInfo, FieldData StringOptionsField)
        {
            EditorValueField editorValueField = null;

            if (memberType.IsEnum)
                editorValueField = new EditorEnumField(Description, Category, SortIndex, null, memberType, ValueField);
            else if (memberType == typeof(string))
            {
                if (StringMode == StringModes.DropDownList)
                {
                    List<string> DropDownList;
                    if (string.IsNullOrEmpty(DropDownListKey))
                        throw new Exception("DROP DOWN LIST KEY UNSPECIFIED");
                    DropDownList = editorInfo.GetStringList(DropDownListKey);
                    if (DropDownList == null)
                        throw new Exception("DROP DOWN LIST MISSING: " + DropDownListKey);

                    editorValueField = new EditorComboStringField(Description, Category, SortIndex, DropDownList, EmptyStringMode, NullValueDescriptor, ValueField);
                }
                else
                    editorValueField = new EditorStringField(Description, Category, SortIndex, NullValueDescriptor, EmptyStringMode, StringMode, ValueField);
            }
            else if (memberType == typeof(List<string>))
                editorValueField = new EditorStringListField(Description, Category, SortIndex, ValueField);
            else if (memberType == typeof(decimal))
                editorValueField = new EditorDecimalField(Description, Category, SortIndex, null, ValueField);
            else if (memberType == typeof(int))
                editorValueField = new EditorIntField(Description, Category, SortIndex, null, ValueField);
            else if (memberType == typeof(long))
                editorValueField = new EditorLongField(Description, Category, SortIndex, null, ValueField);
            else if (memberType == typeof(double))
                editorValueField = new EditorDoubleField(Description, Category, SortIndex, null, ValueField);
            else if (memberType == typeof(uint))
                editorValueField = new EditorUIntField(Description, Category, SortIndex, null, ValueField);
            else if (memberType == typeof(bool))
                editorValueField = new EditorBoolField(Description, Category, SortIndex, null, ValueField);

            if (editorValueField == null)
            {
                Type nullableType = Nullable.GetUnderlyingType(memberType);
                if (nullableType != null)
                {
                    if (nullableType.IsEnum)
                        editorValueField = new EditorEnumField(Description, Category, SortIndex, NullValueDescriptor, nullableType, ValueField);
                    else if (nullableType == typeof(decimal))
                        editorValueField = new EditorDecimalField(Description, Category, SortIndex, NullValueDescriptor, ValueField);
                    else if (nullableType == typeof(int))
                        editorValueField = new EditorIntField(Description, Category, SortIndex, NullValueDescriptor, ValueField);
                    else if (nullableType == typeof(long))
                        editorValueField = new EditorLongField(Description, Category, SortIndex, NullValueDescriptor, ValueField);
                    else if (nullableType == typeof(double))
                        editorValueField = new EditorDoubleField(Description, Category, SortIndex, NullValueDescriptor, ValueField);
                    else if (nullableType == typeof(uint))
                        editorValueField = new EditorUIntField(Description, Category, SortIndex, NullValueDescriptor, ValueField);
                    else if (nullableType == typeof(bool))
                        editorValueField = new EditorBoolField(Description, Category, SortIndex, NullValueDescriptor, ValueField);
                }
            }

            if (editorValueField == null && StringMode == StringModes.DropDownObjectList)
            {
                List<object> DropDownList;
                if (string.IsNullOrEmpty(DropDownListKey))
                    throw new Exception("DROP DOWN LIST KEY UNSPECIFIED");
                DropDownList = editorInfo.GetObjectList(DropDownListKey);
                if (DropDownList == null)
                    throw new Exception("DROP DOWN OBJECT LIST MISSING: " + DropDownListKey);

                foreach (object ob in DropDownList)
                {
                    if (ob == null)
                        continue;
                    if (memberType.IsAssignableFrom(ob.GetType()))
                        continue;
                    throw new Exception("OBJECT LIST CONTAINS INVALID TYPE - Expected: " + memberType.FullName + " Received: " + ob.GetType().FullName);
                }

                editorValueField = new EditorComboObjectField(Description, Category, SortIndex, DropDownList, NullValueDescriptor, ValueField);
            }

            if (editorValueField == null)
                throw new Exception("UNKNOWN TYPE - " + memberType.ToString());

            editorValueField.ValueField = ValueField;
            return editorValueField;
        }
        private static EditorValueField CreateEditorDropDownField(Type memberType, string Description, string Category, double SortIndex, FieldData OptionsField, string NullValueDescriptor, FieldData ValueField, ObjectEditorInfo editorInfo)
        {
            EditorValueField editorValueField = null;

            if (OptionsField == null)
                throw new Exception("OptionsField must be specified");

            if (memberType == typeof(string))
            {
                editorValueField = new EditorComboStringField(Description, Category, SortIndex, OptionsField, NullValueDescriptor, ValueField);
            }

            if (editorValueField == null)
            {
                editorValueField = new EditorComboObjectField(Description, Category, SortIndex, OptionsField, NullValueDescriptor, ValueField);
            }

            editorValueField.ValueField = ValueField;
            return editorValueField;
        }
        private static EditorField CreateListEditorField(Type memberType, string Description, string Category, double SortIndex, EmptyListModes emptyListMode, FieldData ValueField, FieldData DisplayField, ObjectEditorInfo editorInfo)
        {
            if (memberType.IsGenericType && (memberType.GetGenericTypeDefinition() == typeof(List<>)) && memberType.GetGenericArguments()[0].IsClass)
            {
                //The member is type List<T> and we need to return an EditorListObject<T>
                //We have to use reflection to get the correctly typed constructor
                //return new EditorListObjectField<memberType.GetGenericArguments()[0]>(Description, Category, SortIndex, editorInfo, data, ObjectBeingEditted);

                //First we check the type requirements
                Type T = memberType.GetGenericArguments()[0];
                if (!typeof(ICloneable).IsAssignableFrom(T))
                    throw new Exception(T.FullName + " must inherit IClonable");
                if (T.GetConstructor(Type.EmptyTypes) == null)
                    throw new Exception(T.FullName + " must have a public new()");

                //Then we get the Type of EditorListObject<T>
                Type listObjectFieldType = typeof(EditorListObjectField<>).MakeGenericType(T);

                //Next we find the constructor for it.  If the constructor changes, the compiler won't be aware that this needs to also change.
                BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                Type[] types = new Type[] { typeof(string), typeof(string), typeof(double), typeof(ObjectEditorInfo), typeof(FieldData), typeof(FieldData), typeof(EmptyListModes) };
                ConstructorInfo listObjectFieldConstructor = listObjectFieldType.GetConstructor(bindingFlags, null, types, null);

                //Finally, we invoke the constructor and return the EditorListObject<T>
                object listObjectField = listObjectFieldConstructor.Invoke(new object[] { Description, Category, SortIndex, editorInfo, ValueField, DisplayField, emptyListMode });
                return (EditorField)listObjectField;
            }

            throw new Exception("ListEditor incompatible type - " + memberType.ToString());
        }

        public static void ResetFieldValues(List<EditorField> Fields, object ObjectBeingEditted)
        {
            foreach (EditorField Field in Fields)
            {
                if (Field is EditorValueField ValueField)
                    ValueField.ResetFieldValue(ObjectBeingEditted);
            }
        }
        public static void SaveFieldValues(List<EditorField> Fields, object ObjectBeingEditted)
        {
            foreach (EditorField Field in Fields)
            {
                if (Field is EditorValueField ValueField)
                    ValueField.SaveFieldValue(ObjectBeingEditted);
            }
        }
    }
}
