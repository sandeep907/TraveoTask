using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace Traveo
{
    public class Helper
    {
        public class Students
        {
            public int StudentId { get; set; }
            public string StudentName { get; set; }
        }
        public class StudentEnrollments
        {
            public int StudentId { get; set; }
            public int CourseId { get; set; }
        }
        public void GetStudents()
        {
            var paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@oldCourseId", string.Empty));
            paramList.Add(new SqlParameter("@newCourseId", string.Empty));
            paramList.Add(new SqlParameter("@flag", 1));
            var res = ExecuteStoredProc("spGetStudents", paramList);
            Console.WriteLine("------------" + "Student List With Respective Professors" + "------------");
            foreach (DataRow dataRow in res.Rows)
            {
                Console.WriteLine(String.Format("|{0,2}|{1,18}|{2,18}|{3,10}|", dataRow.ItemArray[0].ToString().Trim(), dataRow.ItemArray[1].ToString().Trim(), dataRow.ItemArray[2].ToString().Trim(), dataRow.ItemArray[3].ToString().Trim()));
            }
            Console.WriteLine("Press a key to stop");
            Console.ReadKey();
        }
        public void Tracker()
        {
            using (var tableDependency = new SqlTableDependency<StudentEnrollments>(ConfigurationManager.ConnectionStrings["DBString"].ConnectionString, includeOldValues: true))
            {
                tableDependency.OnChanged += TableDependency_Changed;
                tableDependency.Start();
                Console.WriteLine("Waiting for receiving notifications...");
                Console.WriteLine("Press a key to stop");
                Console.ReadKey();
                tableDependency.Stop();
            }

        }
        void TableDependency_Changed(object sender, RecordChangedEventArgs<StudentEnrollments> e)
        {
            if (e.ChangeType == ChangeType.Update)
            {
                var newEntity = e.Entity;
                var oldEntity = e.EntityOldValues;
                StudentEnrlmntTbl_Update(oldEntity.CourseId, newEntity.CourseId);
            }
            if (e.ChangeType == ChangeType.Insert)
            {
                var newEntity = e.Entity;
                StudentEnrlmntTbl_Insert(newEntity.CourseId);
            }
        }
        public void StudentEnrlmntTbl_Update(int oldCourseId, int newCourseId)
        {
            var paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@oldCourseId", oldCourseId));
            paramList.Add(new SqlParameter("@newCourseId", newCourseId));
            paramList.Add(new SqlParameter("@flag", 3));
            var res = ExecuteStoredProc("spGetStudents", paramList);
            Console.WriteLine("------------" + "Updated Data" + "------------");
            foreach (DataRow dataRow in res.Rows)
            {
                Console.WriteLine(String.Format("|{0,2}|{1,18}|{2,19}|{3,10}|", dataRow.ItemArray[0].ToString().Trim(), dataRow.ItemArray[1].ToString().Trim(), dataRow.ItemArray[2].ToString().Trim(), dataRow.ItemArray[3].ToString().Trim()));
            }
        }
        public void StudentEnrlmntTbl_Insert(int newCourseId)
        {
            var paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@oldCourseId", string.Empty));
            paramList.Add(new SqlParameter("@newCourseId", newCourseId));
            paramList.Add(new SqlParameter("@flag", 2));
            var res = ExecuteStoredProc("spGetStudents", paramList);
            Console.WriteLine("------------"+"Updated Data"+ "------------");
            foreach (DataRow dataRow in res.Rows)
            {
                Console.WriteLine(String.Format("|{0,2}|{1,18}|{2,19}|{3,10}|", dataRow.ItemArray[0].ToString().Trim(), dataRow.ItemArray[1].ToString().Trim(), dataRow.ItemArray[2].ToString().Trim(), dataRow.ItemArray[3].ToString().Trim()));
            }
        }
        public DataTable ExecuteStoredProc(string procName, List<SqlParameter> parmList)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBString"].ConnectionString))
            {
                DataTable dt = new DataTable();
                conn.Open();
                SqlCommand cmd = new SqlCommand(procName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (parmList != null)
                {
                    foreach (var param in parmList)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
                return dt;
            }
        }
    }

}