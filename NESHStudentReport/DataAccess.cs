using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using FISCA.Data;
using K12.Data;

namespace NESHStudentReport
{
    public class DataAccess
    {
        public static List<Student> GetGrade(string SchoolYear,List<string> StudentIDs,bool IsConvertScore)
        {
            Dictionary<string, Student> Students = new Dictionary<string, Student>();

            try
            {
                string strStudentSQL = "select name,english_name,seat_no,class_name,student.id from student left outer join class on class.id=student.ref_class_id where student.id in (" + string.Join(",", StudentIDs.ToArray()) + ")";

                string strScoreSQL = "select ref_student_id,score_info,school_year,semester from sems_subj_score where ref_student_id in (" + string.Join(",", StudentIDs.ToArray()) + ")";

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

                    Students[StudentID].FillScore(row,IsConvertScore);
                }
                #endregion

                #region 取得獎懲資料

                List<DisciplineRecord> DisciplineRecords = Discipline.SelectByStudentIDs(StudentIDs);

                DisciplineRecords = DisciplineRecords.FindAll(x => ("" + x.SchoolYear).Equals(SchoolYear));

                foreach (DisciplineRecord DisciplineRecord in DisciplineRecords)
                {
                    string StudentID = DisciplineRecord.RefStudentID;

                    Students[StudentID].FillDiscipline(DisciplineRecord);
                }
                #endregion

                #region 取得缺曠資料
                List<int> SchoolYears = new List<int>();

                SchoolYears.Add(int.Parse(SchoolYear));

                List<AttendanceRecord> AttendanceRecords = Attendance.Select(StudentIDs, null, null, null,SchoolYears,null);

                foreach(AttendanceRecord AttendanceRecord in AttendanceRecords)
                {
                    string StudentID = AttendanceRecord.RefStudentID;

                    Students[StudentID].FillAttendance(AttendanceRecord);
                }
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
    }
}