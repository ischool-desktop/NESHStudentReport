using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using FISCA.Data;
using SmartSchool.API.PlugIn;

namespace NESHStudentReport
{
    class ExportSemesterScore : SmartSchool.API.PlugIn.Export.Exporter
    {
        public ExportSemesterScore()
        {
            this.Image = null;
            this.Text = "匯出學期成績";
        }

        public override void InitializeExport(SmartSchool.API.PlugIn.Export.ExportWizard wizard)
        {
            List<string> SelectedFields = new List<string>() { "學生系統編號", "學號", "班級", "座號", "姓名" };
            List<string> SelectableFields = new List<string>(){"領域","科目","學年度", "學期","權數","節數" , "成績" , "GPA" ,"Level"};

            wizard.SelectedFields.AddRange(SelectedFields);
            wizard.ExportableFields.AddRange(SelectableFields);

            wizard.ExportPackage += delegate(object sender, SmartSchool.API.PlugIn.Export.ExportPackageEventArgs e)
            {
                try
                {
                    List<string> StudentIDs = e.List;

                    string strScoreSQL = "select ref_student_id,school_year,semester,score_info from sems_subj_score where ref_student_id in (" + string.Join(",", StudentIDs.ToArray()) + ")";

                    QueryHelper QueryHelper = Utility.QueryHelper;

                    //學生系統編號 學號 班級 座號 姓名 領域 科目	學年度 學期 權數 節數 成績 GPA Level

                    DataTable tblScore = QueryHelper.Select(strScoreSQL);

                    foreach (DataRow row in tblScore.Rows)
                    {
                        string StudentID = row.Field<string>("ref_student_id");
                        string SchoolYear = row.Field<string>("school_year");
                        string Semester = row.Field<string>("semester");
                        string ScoreInfo = "<root>" + row.Field<string>("score_info") + "</root>";

                        XElement elmScoreInfo = XElement.Load(new StringReader(ScoreInfo));

                        foreach (XElement elmSubject in elmScoreInfo
                            .Element("SemesterSubjectScoreInfo")
                            .Elements("Subject"))
                        {
                            RowData vRow = new RowData();

                            //<Subject GPA=""4.5"" Level=""11"" 努力程度="""" 成績=""100"" 文字描述="""" 權數=""1"" 科目=""物理"" 節數=""1"" 註記="""" 領域=""""/>
                            // "學年度", "學期","領域","科目","權數","節數" , "成績" , "GPA" ,"Level"

                            vRow.ID = StudentID;
                            string Subject = elmSubject.AttributeText("科目");
                            string Score = elmSubject.AttributeText("成績");
                            string Period = elmSubject.AttributeText("節數");
                            string Domain = elmSubject.AttributeText("領域");
                            string Grade = Score;

                            vRow.Add("學年度", SchoolYear);
                            vRow.Add("學期", Semester);
                            vRow.Add("領域", Domain);
                            vRow.Add("科目", Subject);
                            vRow.Add("權數", elmSubject.AttributeText("權數"));
                            vRow.Add("節數", elmSubject.AttributeText("節數"));
                            vRow.Add("成績", elmSubject.AttributeText("成績"));
                            vRow.Add("GPA", elmSubject.AttributeText("GPA"));
                            vRow.Add("Level", elmSubject.AttributeText("Level"));
                            
                            e.Items.Add(vRow);
                        }
                    }
                }
                catch (Exception ve)
                {
                    MessageBox.Show(ve.Message);
                }
            };
        }
    }
}