using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using FISCA.Data;
using K12.Data;
using K12.BusinessLogic;
using FISCA.UDT;
using CourseGradeB.EduAdminExtendControls;

namespace NESHStudentReport
{
    public class DataAccess
    {

        private static AccessHelper _A = new AccessHelper();
        public static Dictionary<string, List<string>> AbsenceSetDic = new Dictionary<string, List<string>>();
        public static Dictionary<string, SubjectRecord> SubjectChineseNameRef = new Dictionary<string, SubjectRecord>();

        public static List<Student> GetGrade(string SchoolYear,List<string> StudentIDs,bool IsConvertScore,string title)
        {
            //假別設定初始化
            AbsenceSetDicInit();
            //科目參照初始化
            SubjectRefInit();

            Dictionary<string, Student> Students = new Dictionary<string, Student>();

            try
            {
                string strStudentSQL = "select name,english_name,seat_no,class_name,student.id from student left outer join class on class.id=student.ref_class_id where student.id in (" + string.Join(",", StudentIDs.ToArray()) + ")";

                string strScoreSQL = "select ref_student_id,score_info,school_year,semester from sems_subj_score where school_year=" + SchoolYear + " and ref_student_id in (" + string.Join(",", StudentIDs.ToArray()) + ")";

                QueryHelper QueryHelper = Utility.QueryHelper;

                #region 取得學生資料
                DataTable tblResult = QueryHelper.Select(strStudentSQL);

                foreach (DataRow row in tblResult.Rows)
                {
                    string StudentID = row.Field<string>("id");

                    if (!Students.ContainsKey(StudentID))
                    {
                        Student vStudent = new Student();

                        vStudent.FillBasic(row, SchoolYear);

                        Students.Add(vStudent.StudentID, vStudent); 
                    }
                }
                #endregion

                #region 取得成績資料
                tblResult = QueryHelper.Select(strScoreSQL);

                foreach (DataRow row in tblResult.Rows)
                {
                    string StudentID = row.Field<string>("ref_student_id");

                    Students[StudentID].FillScore(row,IsConvertScore,title);
                }
                #endregion

                //Get AutoSummaryRecord
                List<AutoSummaryRecord> AutoSummaryRecords = AutoSummary.Select(StudentIDs, null);
                AutoSummaryRecords = AutoSummaryRecords.FindAll(x => ("" + x.SchoolYear).Equals(SchoolYear));

                #region 取得獎懲資料
                foreach (AutoSummaryRecord record in AutoSummaryRecords)
                {
                    string StudentID = record.RefStudentID;

                    Students[StudentID].FillDiscipline(record);
                }

                //List<DisciplineRecord> DisciplineRecords = Discipline.SelectByStudentIDs(StudentIDs);

                //DisciplineRecords = DisciplineRecords.FindAll(x => ("" + x.SchoolYear).Equals(SchoolYear));

                //foreach (DisciplineRecord DisciplineRecord in DisciplineRecords)
                //{
                //    string StudentID = DisciplineRecord.RefStudentID;

                //    Students[StudentID].FillDiscipline(DisciplineRecord);
                //}
                #endregion

                #region 取得缺曠資料
                foreach (AutoSummaryRecord record in AutoSummaryRecords)
                {
                    string StudentID = record.RefStudentID;

                    Students[StudentID].FillAttendance(record);
                }
                //List<int> SchoolYears = new List<int>();

                //SchoolYears.Add(int.Parse(SchoolYear));

                //List<AttendanceRecord> AttendanceRecords = Attendance.Select(StudentIDs, null, null, null,SchoolYears,null);

                //foreach(AttendanceRecord AttendanceRecord in AttendanceRecords)
                //{
                //    string StudentID = AttendanceRecord.RefStudentID;

                //    Students[StudentID].FillAttendance(AttendanceRecord);
                //}
                #endregion

                #region 取得學期曆程

                List<SemesterHistoryRecord> SemesterRecords = SemesterHistory.SelectByStudentIDs(StudentIDs);

                foreach (SemesterHistoryRecord SemesterRecord in SemesterRecords)
                {
                    string StudentID = SemesterRecord.RefStudentID;

                    Students[StudentID].FillSemesterHistory(SemesterRecord);
                }
                #endregion
            }
            catch (Exception ve)
            {
                MessageBox.Show(ve.Message);
            }

            return Students.Values.ToList();
        }

        /// <summary>
        /// 取得假別設定
        /// </summary>
        /// <returns></returns>
        public static void AbsenceSetDicInit()
        {
            AbsenceSetDic.Clear();

            AbsenceSetDic.Add("事病假", new List<string>());
            AbsenceSetDic.Add("曠課", new List<string>());
            AbsenceSetDic.Add("遲到", new List<string>());
            AbsenceSetDic.Add("升旗", new List<string>());
            AbsenceSetDic.Add("早午休遲到", new List<string>());
            AbsenceSetDic.Add("早午休曠課", new List<string>());

            foreach (AbsenceUDT au in _A.Select<AbsenceUDT>())
            {
                if (AbsenceSetDic.ContainsKey(au.Target) && !AbsenceSetDic[au.Target].Contains(au.Source))
                    AbsenceSetDic[au.Target].Add(au.Source);
            }
        }

        /// <summary>
        /// 初始化科目參照
        /// </summary>
        public static void SubjectRefInit()
        {
            SubjectChineseNameRef.Clear();

            foreach (SubjectRecord subj in _A.Select<SubjectRecord>())
            {
                if (!SubjectChineseNameRef.ContainsKey(subj.Name))
                    SubjectChineseNameRef.Add(subj.Name, subj);
            }
        }

        /// <summary>
        /// 取得科目中文名稱
        /// </summary>
        /// <param name="subj"></param>
        /// <returns></returns>
        public static string GetSubjectChineseName(string subj)
        {
            if (SubjectChineseNameRef.ContainsKey(subj))
                return SubjectChineseNameRef[subj].ChineseName + " ";
            else
                return string.Empty;
        }
    }
}