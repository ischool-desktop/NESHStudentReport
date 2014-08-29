using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NESHStudentReport
{
    class Permissions
    {
        public static string 學期成績通知單3至6年級 { get { return "bafbb97a-45db-4502-8142-eb20fb4481c3"; } }

        public static bool 學期成績通知單3至6年級權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[學期成績通知單3至6年級].Executable;
            }
        }

        public static string 學期成績通知單7至8年級 { get { return "1f5b42a2-f80e-4154-9390-55050a866b13"; } }

        public static bool 學期成績通知單7至8年級權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[學期成績通知單7至8年級].Executable;
            }
        }

        public static string 學期成績通知單9至12年級 { get { return "f3398500-b1ab-491f-bd76-672d0543afd7"; } }

        public static bool 學期成績通知單9至12年級權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[學期成績通知單9至12年級].Executable;
            }
        }
    }
}