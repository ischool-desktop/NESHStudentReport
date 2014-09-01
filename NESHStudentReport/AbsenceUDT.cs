using FISCA.UDT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NESHStudentReport
{
    [FISCA.UDT.TableName("NESHStudentReport.AbsenceUDT")]
    public class AbsenceUDT : ActiveRecord
    {
        [FISCA.UDT.Field(Field = "target")]
        public string Target { get; set; }

        [FISCA.UDT.Field(Field = "source")]
        public string Source { get; set; }
    }
}
