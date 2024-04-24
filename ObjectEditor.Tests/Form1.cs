using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ObjectEditor.Tests.Classes;

namespace ObjectEditor.Tests
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ButtonTestClass buttonTest = new ButtonTestClass();
        private void btnButtonTest_Click(object sender, EventArgs e)
        {
            ObjectEditors.ShowObjectEditor("Button Test", buttonTest, null);
        }

        readonly Dictionary<string, DictionaryTestClass> dictionaryTest = new Dictionary<string, DictionaryTestClass>();
        private void button1_Click(object sender, EventArgs e)
        {
            ObjectEditorInfo info = new ObjectEditorInfo();
            info.JsonPackager = new Json.JsonPackager(System.Reflection.Assembly.GetAssembly(typeof(DictionaryTestClass)));

            Dictionary<string, DictionaryTestClass> copy = dictionaryTest.Copy();
            if (ObjectEditors.ShowDictionaryEditor("Dictionary Test", copy, info) == DialogResult.OK)
            {
                dictionaryTest.SetRange(copy);
            }
        }

        EnumTestClass testEnum = new EnumTestClass();
        private void button2_Click(object sender, EventArgs e)
        {
            ObjectEditors.ShowObjectEditor("Enum Test", testEnum, null);
        }

        List<ListTestClass> listTest = new List<ListTestClass>();
        private void button3_Click(object sender, EventArgs e)
        {
            List<ListTestClass> copy = listTest.Copy();
            if (ObjectEditors.ShowListEditor("List Test", copy, null) == DialogResult.OK)
                listTest.SetRange(copy);
        }

        StringsTestClass stringsTest = new StringsTestClass();
        private void btnStringsTest_Click(object sender, EventArgs e)
        {
            ObjectEditors.ShowObjectEditor("String Test", stringsTest, null);
        }
    }

    public static class helpers
    {
        public static List<T> Copy<T>(this List<T> list) where T : ICloneable
        {
            List<T> copy = new List<T>(list.Capacity);
            foreach (T ob in list)
                copy.Add((T)ob.Clone());
            return copy;
        }
        public static void SetRange<T>(this List<T> list, List<T> values)
        {
            list.Clear();
            list.AddRange(values);
        }
        public static Dictionary<K, V> Copy<K, V>(this Dictionary<K, V> dict) where V : ICloneable
        {
            Dictionary<K, V> copy = new Dictionary<K, V>(dict.Count);
            foreach (KeyValuePair<K, V> ob in dict)
                copy[ob.Key] = (V)ob.Value.Clone();
            return copy;
        }
        public static void SetRange<K, V>(this Dictionary<K, V> dict, Dictionary<K, V> values)
        {
            dict.Clear();
            foreach (KeyValuePair<K, V> ob in values)
                dict[ob.Key] = ob.Value;
        }
    }
}
