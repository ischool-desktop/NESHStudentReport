using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml.Linq;
using CourseGradeB;
using K12.Data;

namespace NESHStudentReport
{
    public class Student
    {
        /// <summary>
        /// 學生系統編號
        /// </summary>
        public string StudentID { get; set; }

        /// <summary>
        /// 學生姓名
        /// </summary>
        public string StudentName { get; set; }

        /// <summary>
        /// 座號
        /// </summary>
        public string SeatNo { get; set; }

        /// <summary>
        /// 班級名稱
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 上學期總平均
        /// </summary>
        public string FirstAvgScore { get; set; }

        /// <summary>
        /// 下學期總平均
        /// </summary>
        public string SecondAvgScore { get; set; }

        /// <summary>
        /// 上學期GPA
        /// </summary>
        public string FirstAvgGPA { get; set; }

        /// <summary>
        /// 下學期GPA
        /// </summary>
        public string SecondAvgGPA { get; set; }

        /// <summary>
        /// 科目列表
        /// </summary>
        public List<Subject> Subjects { get; set; }

        #region 獎懲相關屬性

        public int FirstMeritA { get; set; }

        public int FirstMeritB { get; set; }

        public int FirstMeritC { get; set; }

        public int FirstDemeritA { get; set; }

        public int FirstDemeritB { get; set; }

        public int FirstDemeritC { get; set; }

        public int SecondMeritA { get; set; }

        public int SecondMeritB { get; set; }

        public int SecondMeritC { get; set; }

        public int SecondDemeritA { get; set; }

        public int SecondDemeritB { get; set; }

        public int SecondDemeritC { get; set; }

        #endregion

        #region 缺曠相關資料
        /// <summary>
        /// 學期應出席日期
        /// </summary>
        public int FirstTotalDaysOfSchool { get; set; }

        /// <summary>
        /// 事/病假節數
        /// </summary>
        public int FirstPerodsAbsentExcused { get; set; }

        /// <summary>
        /// 曠課節數
        /// </summary>
        public int FirstPerodsAbsentUnexcused { get; set; }

        /// <summary>
        /// 遲到次數
        /// </summary>
        public int FirstTimesTardy { get; set; }

        /// <summary>
        /// 升旗
        /// </summary>
        public int FirstFlagCeremonyUnexcused { get; set; }

        /// <summary>
        /// 早/午休遲到
        /// </summary>
        public int FirstQuietTimeTardy { get; set; }

        /// <summary>
        /// 早/午休曠課
        /// </summary>
        public int FirstQuietTimeUnexcused { get; set; }

        /// <summary>
        /// 學期應出席日期
        /// </summary>
        public int SecondTotalDaysOfSchool { get; set; }

        /// <summary>
        /// 事/病假節數
        /// </summary>
        public int SecondPerodsAbsentExcused { get; set; }

        /// <summary>
        /// 曠課節數
        /// </summary>
        public int SecondPerodsAbsentUnexcused { get; set; }

        /// <summary>
        /// 遲到次數
        /// </summary>
        public int SecondTimesTardy { get; set; }

        /// <summary>
        /// 升旗
        /// </summary>
        public int SecondFlagCeremonyUnexcused { get; set; }

        /// <summary>
        /// 早/午休遲到
        /// </summary>
        public int SecondQuietTimeTardy { get; set; }

        /// <summary>
        /// 早/午休曠課
        /// </summary>
        public int SecondQuietTimeUnexcused { get; set; }
        #endregion

        /// <summary>
        /// 顯示學年
        /// </summary>
        public string DisplaySchoolYear { get; set; }

        /// <summary>
        /// 學年
        /// </summary>
        public string SchoolYear { get; set; }

        /// <summary>
        /// 上學期上課天數
        /// </summary>
        public int FirstSchoolDayCount { get; set; }

        /// <summary>
        /// 下學期上課天數
        /// </summary>
        public int SecondSchoolDayCount { get; set; }

        public Student()
        {
            Subjects = new List<Subject>();
        }

        private string GetSchoolYearDisplay(string SchoolYear)
        {
            try
            {
                int intSchoolYear = int.Parse(SchoolYear) + 1911;

                return (intSchoolYear - 1) + "~" + intSchoolYear;
            }
            catch
            {
                return SchoolYear;
            }
        }

        public void FillBasic(DataRow row, string SchoolYear)
        {
            this.DisplaySchoolYear = GetSchoolYearDisplay(SchoolYear);
            this.SchoolYear = SchoolYear;
            FillBasic(row);
        }

        public void FillBasic(DataRow row)
        {
            StudentID = row.Field<string>("id");
            StudentName = row.Field<string>("name") + " " + row.Field<string>("english_name");
            SeatNo = row.Field<string>("seat_no");
            ClassName = row.Field<string>("class_name");
        }
        public void FillDiscipline(DisciplineRecord vRecord)
        {
            if (vRecord.Semester.Equals(1))
            {
                this.FirstMeritA += K12.Data.Int.GetValue(vRecord.MeritA);
                this.FirstMeritB += K12.Data.Int.GetValue(vRecord.MeritB);
                this.FirstMeritC += K12.Data.Int.GetValue(vRecord.MeritC);

                this.FirstDemeritA += K12.Data.Int.GetValue(vRecord.DemeritA);
                this.FirstDemeritB += K12.Data.Int.GetValue(vRecord.DemeritB);
                this.FirstDemeritC += K12.Data.Int.GetValue(vRecord.DemeritC);
            }
            else if (vRecord.Semester.Equals(2))
            {
                this.SecondMeritA += K12.Data.Int.GetValue(vRecord.MeritA);
                this.SecondMeritB += K12.Data.Int.GetValue(vRecord.MeritB);
                this.SecondMeritC += K12.Data.Int.GetValue(vRecord.MeritC);

                this.SecondDemeritA += K12.Data.Int.GetValue(vRecord.DemeritA);
                this.SecondDemeritB += K12.Data.Int.GetValue(vRecord.DemeritB);
                this.SecondDemeritC += K12.Data.Int.GetValue(vRecord.DemeritC); 
            }
        }

        public void FillSemesterHistory(SemesterHistoryRecord vRecord)
        {
            foreach(SemesterHistoryItem Item in vRecord.SemesterHistoryItems)
            {
                if (("" + Item.SchoolYear).Equals(SchoolYear))
                {
                    if (Item.Semester.Equals(1))
                    {
                        FirstSchoolDayCount = Item.SchoolDayCount.HasValue? Item.SchoolDayCount.Value : 0 ;
                    }
                    else if (Item.Semester.Equals(2))
                    {
                        SecondSchoolDayCount = Item.SchoolDayCount.HasValue ? Item.SchoolDayCount.Value : 0;
                    }
                }
            }
        }

        public void FillAttendance(AttendanceRecord vRecord)
        {
            if (vRecord.Semester.Equals(1))
            {
                foreach (AttendancePeriod Period in vRecord.PeriodDetail)
                {
                    if (Period.AbsenceType.Equals("事假") || Period.AbsenceType.Equals("病假"))
                        FirstPerodsAbsentExcused++;
                    else if (Period.AbsenceType.Equals("曠課"))
                        FirstPerodsAbsentUnexcused++;
                    else if (Period.AbsenceType.Equals("遲到"))
                        FirstTimesTardy++;
                    else if (Period.AbsenceType.Equals("早休遲到") || Period.AbsenceType.Equals("午休遲到"))
                        FirstQuietTimeTardy++;
                    else if (Period.AbsenceType.Equals("早休曠課") || Period.AbsenceType.Equals("午休曠課"))
                        FirstQuietTimeUnexcused++;
                    else if (Period.AbsenceType.Equals("升旗"))
                        FirstFlagCeremonyUnexcused++;
                }
            }
            else if (vRecord.Semester.Equals(2))
            {
                foreach (AttendancePeriod Period in vRecord.PeriodDetail)
                {
                    if (Period.AbsenceType.Equals("事假") || Period.AbsenceType.Equals("病假"))
                        SecondPerodsAbsentExcused++;
                    else if (Period.AbsenceType.Equals("曠課"))
                        SecondPerodsAbsentUnexcused++;
                    else if (Period.AbsenceType.Equals("遲到"))
                        SecondTimesTardy++;
                    else if (Period.AbsenceType.Equals("早休遲到") || Period.AbsenceType.Equals("午休遲到"))
                        SecondQuietTimeTardy++;
                    else if (Period.AbsenceType.Equals("早休曠課") || Period.AbsenceType.Equals("午休曠課"))
                        SecondQuietTimeUnexcused++;
                    else if (Period.AbsenceType.Equals("升旗"))
                        SecondFlagCeremonyUnexcused++;
                }
            }
        }

        private string GetScore(string Score,bool IsConvertScore)
        {
            if (!IsConvertScore)
                return Score;

            decimal intScore;

            if (decimal.TryParse(Score, out intScore))
                return Tool.GPA.Eval(intScore).Letter;
            else
                return Score;
        }

        public void FillScore(DataRow row,bool IsConvertScore)
        {
            //<SemesterSubjectScoreInfo>
            //    <Subject GPA=""0"" Level=""4"" 努力程度="""" 成績=""50"" 文字描述="""" 權數=""3"" 科目=""奇怪的科目"" 節數=""3"" 註記="""" 領域=""TEST""/>
            //    <Subject GPA=""4.5"" Level=""11"" 努力程度="""" 成績=""100"" 文字描述="""" 權數=""1"" 科目=""物理"" 節數=""1"" 註記="""" 領域=""""/>
            //</SemesterSubjectScoreInfo>"

            string Semester = row.Field<string>("semester");
            string ScoreInfo = "<root>" + row.Field<string>("score_info") + "</root>";

            XElement elmScoreInfo = XElement.Load(new StringReader(ScoreInfo));

            string AvgScore = elmScoreInfo.ElementText("AvgScore");
            string AvgGPA = elmScoreInfo.ElementText("AvgGPA");

            if (Semester.Equals("1"))
            {
                this.FirstAvgScore = GetScore(AvgScore, IsConvertScore);
                this.FirstAvgGPA = AvgGPA;
            }
            else if (Semester.Equals("2"))
            { 
                this.SecondAvgScore = GetScore(AvgScore, IsConvertScore);
                this.SecondAvgGPA = AvgGPA;
            }

            foreach (XElement elmSubject in elmScoreInfo
                .Element("SemesterSubjectScoreInfo")
                .Elements("Subject"))
            {
                string Subject = elmSubject.AttributeText("科目");
                string Score = elmSubject.AttributeText("成績");
                string Period = elmSubject.AttributeText("節數");
                string Grade = Score;

                Grade = GetScore(Score,IsConvertScore);

                Subject vSubject = Subjects.Find(x => x.Name.Equals(Subject));

                if (vSubject == null)
                {
                    vSubject = new Subject();
                    vSubject.Name = Subject;
                    vSubject.Hour = Period;
                    Subjects.Add(vSubject); 
                }

                if (Semester.Equals("1"))
                    vSubject.FristGrade = Grade;
                else if (Semester.Equals("2"))
                    vSubject.SecondGrade = Grade;
            }
        }

        public Dictionary<string, object> OutputValue()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            result.Add("學年度", DisplaySchoolYear);
            result.Add("姓名", StudentName);
            result.Add("班級", ClassName);
            result.Add("座號", SeatNo);

            result.Add("上學期總平均", FirstAvgScore);
            result.Add("下學期總平均", SecondAvgScore);

            result.Add("上學期GPA", FirstAvgGPA);
            result.Add("下學期GPA", SecondAvgGPA);

            result.Add("上課天數上",FirstSchoolDayCount);
            result.Add("上課天數下",SecondSchoolDayCount);

            result.Add("大功上", FirstMeritA);
            result.Add("小功上", FirstMeritB);
            result.Add("嘉獎上", FirstMeritC);
            result.Add("大過上", FirstDemeritA);
            result.Add("小過上", FirstDemeritB);
            result.Add("警告上", FirstDemeritC);

            result.Add("大功下", SecondMeritA);
            result.Add("小功下", SecondMeritB);
            result.Add("嘉獎下", SecondMeritC);
            result.Add("大過下", SecondDemeritA);
            result.Add("小過下", SecondDemeritB);
            result.Add("警告下", SecondDemeritC);

            result.Add("事病假節數上", FirstPerodsAbsentExcused );
            result.Add("曠課節數上", FirstPerodsAbsentUnexcused);
            result.Add("遲到次數上", FirstTimesTardy);
            result.Add("早午休遲到上", FirstQuietTimeTardy);
            result.Add("早午休曠課上", FirstQuietTimeUnexcused);
            result.Add("升旗上", FirstFlagCeremonyUnexcused);

            result.Add("事病假節數下", SecondPerodsAbsentExcused);
            result.Add("曠課節數下", SecondPerodsAbsentUnexcused);
            result.Add("遲到次數下", SecondTimesTardy);
            result.Add("早午休遲到下", SecondQuietTimeTardy);
            result.Add("早午休曠課下", SecondQuietTimeUnexcused);
            result.Add("升旗下", SecondFlagCeremonyUnexcused);

            for (int i = 0; i < Subjects.Count; i++)
            {
                result.Add("科目"+(i+1), Subjects[i].Name);
                result.Add("科目" + (i + 1) + "時數", Subjects[i].Hour);
                result.Add("科目" + (i + 1) + "上學期", Subjects[i].FristGrade);
                result.Add("科目" + (i + 1) + "下學期", Subjects[i].SecondGrade);
            }

            for (int i = Subjects.Count ; i < 20; i++)
            {
                result.Add("科目" + (i + 1), string.Empty);
                result.Add("科目" + (i + 1) + "時數", string.Empty);
                result.Add("科目" + (i + 1) + "上學期", string.Empty);
                result.Add("科目" + (i + 1) + "下學期", string.Empty);
            }

            return result;
        }
    }
}