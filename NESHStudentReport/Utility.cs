using FISCA.UDT;
using FISCA.Data;

namespace NESHStudentReport
{
    /// <summary>
    /// 常用函式庫
    /// </summary>
    public class Utility
    {
        private static AccessHelper mHelper = null;
        private static QueryHelper mQueryHelper = null;

        /// <summary>
        /// 取得QueryHelper
        /// </summary>
        public static QueryHelper QueryHelper
        {
            get
            {
                if (mQueryHelper == null)
                    mQueryHelper = new QueryHelper();

                return mQueryHelper;
            }
        }

        /// <summary>
        /// 取得AccessHelper
        /// </summary>
        /// <returns></returns>
        public static AccessHelper AccessHelper
        {
            get
            {
                if (mHelper == null)
                    mHelper = new AccessHelper();

                return mHelper;
            }
        }
    }
}