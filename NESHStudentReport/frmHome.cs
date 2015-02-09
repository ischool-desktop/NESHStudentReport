using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aspose.Words;
using FISCA.Presentation.Controls;

namespace NESHStudentReport
{
    public partial class frmHome : FISCA.Presentation.Controls.BaseForm
    {
        private byte[] template;
        private string title;

        public frmHome(string title,byte[] template)
        {
            InitializeComponent();

            this.Text = title;
            this.TitleText = title;

            this.title = title;
            this.template = template;
        }

        private void labelX1_Click(object sender, EventArgs e)
        {

        }

        private void Completed(string inputReportName, Document inputDoc)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.Title = "另存新檔";
            sd.FileName = inputReportName + DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss") + ".doc";
            sd.Filter = "Word檔案 (*.doc)|*.doc|所有檔案 (*.*)|*.*";
            sd.AddExtension = true;
            if (sd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    inputDoc.Save(sd.FileName, Aspose.Words.SaveFormat.Doc);
                    System.Diagnostics.Process.Start(sd.FileName);
                }
                catch
                {
                    MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        //private void MailMerge_MergeField(object sender, Aspose.Words.Reporting.MergeFieldEventArgs e)
        //{
        //    #region 科目成績

        //    #endregion
        //}

        private void btnRun_Click(object sender, EventArgs e)
        {
            //假別設定初始化
            //DataAccess.AbsenceSetDicInit();
            
            string SelectedSchoolYear = "" + cmbSchoolYear.SelectedItem;
            List<string> SelectedStudentIDs = K12.Presentation.NLDPanels.Student.SelectedSource;

            if (K12.Data.Utility.Utility.IsNullOrEmpty(SelectedStudentIDs))
            {
                MessageBox.Show("請選取學生！");
                return;
            }

            if (!string.IsNullOrEmpty(SelectedSchoolYear))
            {
                this.btnPrint.Enabled = false;

                Task<Document> task = Task<Document>.Factory.StartNew(() =>
                {
                    MemoryStream template = new MemoryStream(this.template);
                    Document doc = new Document();
                    doc.Sections.Clear();
                    List<string> keys = new List<string>();
                    List<object> values = new List<object>();
                    Dictionary<string, object> mergeKeyValue = new Dictionary<string,object>();
                    List<Student> Students = new List<Student>();
                    
                    if (this.title.Contains("9"))
                        Students = DataAccess.GetGrade(SelectedSchoolYear, SelectedStudentIDs, false);
                    else
                        Students = DataAccess.GetGrade(SelectedSchoolYear, SelectedStudentIDs, true);

                    foreach (Student vStudent in Students)
                    {
                        template.Seek(0, SeekOrigin.Begin);
                        //Document dataDoc = new Document(template, "", LoadFormat.Doc, "");
                        //dataDoc.MailMerge.MergeField += new Aspose.Words.Reporting.MergeFieldEventHandler(MailMerge_MergeField);
                        //dataDoc.MailMerge.RemoveEmptyParagraphs = true;
                        //Document dataDoc = new Document(new MemoryStream(template));
                        Document dataDoc = new Document(template);

                        mergeKeyValue = vStudent.OutputValue();

                        dataDoc.MailMerge.Execute(mergeKeyValue.Keys.ToArray(), mergeKeyValue.Values.ToArray());
                        doc.Sections.Add(doc.ImportNode(dataDoc.Sections[0], true));
                    }

                    return doc;
                });
                task.ContinueWith((x) =>
                {
                    this.btnPrint.Enabled = true;

                    if (x.Exception != null)
                        MessageBox.Show(x.Exception.InnerException.Message);
                    else
                        Completed(this.TitleText, x.Result);
                }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void frmHome_Load(object sender, EventArgs e)
        {
            string SchoolYear = K12.Data.School.DefaultSchoolYear;
            string Semester = K12.Data.School.DefaultSemester;

            int vSchoolYear;

            if (int.TryParse(SchoolYear, out vSchoolYear))
            {
                for (int i = vSchoolYear - 3; i <= vSchoolYear; i++)
                    cmbSchoolYear.Items.Add(i);
                cmbSchoolYear.SelectedIndex = 3;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new SetForm().ShowDialog();
        }
    }
}