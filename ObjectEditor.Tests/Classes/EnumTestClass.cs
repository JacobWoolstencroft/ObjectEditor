using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectEditor.Attributes;

namespace ObjectEditor.Tests.Classes
{
    [EditableObject(PreferredCategoryOrder = new string[] { null, "Correct Letters" })]
    class EnumTestClass
    {
        enum Letter
        {
            A,
            B,
            C
        }

        public EnumTestClass()
        {
            CorrectLetter1 = RandomLetter();
            CorrectLetter2 = RandomLetter();
            CorrectLetter3 = RandomLetter();
        }
        private Random rnd = new Random();
        private Letter RandomLetter()
        {
            switch (rnd.Next(0, 4))
            {
                case 0:
                default:
                    return Letter.A;
                case 1:
                    return Letter.B;
                case 2:
                    return Letter.C;
            }
        }

        [EditableField]
        bool Letter1Visible = true, Letter2Visible = true, Letter3Visible = true;

        [EditableField(VisibilityFlagMember = "Letter1Visible")]
        Letter? Letter1 = null;

        [EditableField(VisibilityFlagMember = "Letter2Visible")]
        Letter? Letter2 = null;

        [EditableField(VisibilityFlagMember = "Letter3Visible")]
        Letter? Letter3 = null;

        [EditableField(Description = "Correct Letters")]
        string CorrectLetters
        {
            get
            {
                if (Letter1 == null || Letter2 == null || Letter3 == null)
                    return "[Test not complete]";
                int correct = 0;
                if (Letter1.Value == CorrectLetter1)
                    correct++;
                if (Letter2.Value == CorrectLetter2)
                    correct++;
                if (Letter3.Value == CorrectLetter3)
                    correct++;

                return correct + " out of 3";
            }
        }

        [EditableField(Description = "Show Correct Letters")]
        bool ShowCorrectLetters;

        [EditableField(Description = "Correct Letter 1", Category = "Correct Letters", VisibilityFlagMember = "ShowCorrectLetters")]
        Letter CorrectLetter1 = Letter.A;
        [EditableField(Description = "Correct Letter 2", Category = "Correct Letters", VisibilityFlagMember = "ShowCorrectLetters")]
        Letter CorrectLetter2 = Letter.A;
        [EditableField(Description = "Correct Letter 3", Category = "Correct Letters", VisibilityFlagMember = "ShowCorrectLetters")]
        Letter CorrectLetter3 = Letter.A;

        [ClickableButtonMethod(Description = "Create New Test", Category = "Correct Letters", VisibilityFlagMember = "ShowCorrectLetters")]
        void CreateNewTest()
        {
            Letter1 = null;
            Letter2 = null;
            Letter3 = null;
            CorrectLetter1 = RandomLetter();
            CorrectLetter2 = RandomLetter();
            CorrectLetter3 = RandomLetter();
            ShowCorrectLetters = false;
        }


        [EditableField(Description = "Hidden Category Test", StringMode = StringModes.Multiline)]
        string hiddenCategoryTestDesc
        {
            get
            {
                return "The \"Correct Letters\" category should only appear if \"Show Correct Letters\" is true"
                    + "\nClicking \"Create New Test\" should randomize the correct letters, reset \"Show Correct Letters\", and hide the \"Correct Letters\" tab"
                    + "\nClicking \"Create New Test\" should NOT result in a letter still being shown, or show correct letters showing \"YES\"";
            }
        }
    }
}
