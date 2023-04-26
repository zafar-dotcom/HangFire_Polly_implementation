using Hangfire_background_jobs.IServices;
using Hangfire_background_jobs.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Transactions;

namespace Hangfire_background_jobs.Services
{
    public class DAL :IDAL
    {
        private readonly string str = "server=localhost;port=3306;uid=root;pwd=sobiazafar@2023;database=mvc_crud";


        public List<TransactionModel> GetfromMasterDEtail()
        {
            using (MySqlConnection conn = new MySqlConnection(str))
            {
                DataTable tbl = new DataTable();
                List<TransactionModel> lst = new List<TransactionModel>();
                // TransactionModel obj = new TransactionModel();
                conn.Open();
                string query = "select a.app_id,e.app_id,a.name, a.age ," +
                    "a.qualificaion,a.total_experience ,e.company_name,e.designation" +
                    " ,e.years_worked from \r\nApplicant a join experience e on a.app_id=e.app_id";
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = query;
                        MySqlDataReader dr = cmd.ExecuteReader();
                        tbl.Load(dr);
                        conn.Close();
                        foreach (DataRow row in tbl.Rows)
                        {
                            TransactionModel obj = new TransactionModel()
                            {
                                app_id = (int)row["app_id"],
                                Name = row["name"].ToString(),
                                Age = (int)row["age"],
                                Qualificaion = row["qualificaion"].ToString(),
                                Total_Experience = row["total_experience"].ToString(),
                                Experiences = new List<Experience>()
                            };

                            Experience exp = new Experience()
                            {
                                app_id = (int)row["app_id"],
                                company_name = row["company_name"].ToString(),
                                designation = row["designation"].ToString(),
                                years_worked = (int)row["years_worked"]
                            };
                            obj.Experiences.Add(exp);
                            //check in the list if applicant already exit
                            var exitingobj = lst.FirstOrDefault(x => x.app_id == obj.app_id);
                            if (exitingobj != null)
                            {
                                exitingobj.Experiences.Add(exp);
                            }
                            else
                            {
                                lst.Add(obj);
                            }


                        }
                    }
                    return lst;

                }

                catch (Exception)
                {

                    throw;

                }
            }
        }
        public bool Update_master_detail(TransactionModel modl)
        {
            using (var transscope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {


                try
                {
                    using (MySqlConnection conn = new MySqlConnection(str))
                    {
                        conn.Open();
                        string query1 = "update Applicant set name=@name,gender=@gender,age=@age,qualificaion=@qualificaion,total_experience=@total_experience where app_id=@app_id";
                        MySqlCommand cmd1 = new MySqlCommand(query1, conn);
                        cmd1.Parameters.AddWithValue("@name", modl.Name);
                        cmd1.Parameters.AddWithValue("@gender", modl.Gender);
                        cmd1.Parameters.AddWithValue("@age", modl.Age);
                        cmd1.Parameters.AddWithValue("@qualificaion", modl.Qualificaion);
                        cmd1.Parameters.AddWithValue("@total_experience", modl.Total_Experience);
                        cmd1.Parameters.AddWithValue("@app_id", modl.app_id);
                        cmd1.ExecuteNonQuery();

                        cmd1.CommandText = "delete from experience where app_id=@app_id";
                        cmd1.ExecuteNonQuery();

                        foreach (var experience in modl.Experiences)
                        {
                            string query = "insert into experience (company_name,designation,years_worked,app_id)values(@company_name,@designation,@years_worked,@app_id)";
                            MySqlCommand cmd2 = new MySqlCommand(query, conn);
                            cmd2.Parameters.AddWithValue("@company_name", experience.company_name);
                            cmd2.Parameters.AddWithValue("@designation", experience.designation);
                            cmd2.Parameters.AddWithValue("@years_worked", experience.years_worked);
                            cmd2.Parameters.AddWithValue("@app_id", modl.app_id);
                            cmd2.ExecuteNonQuery();
                            cmd2.Parameters.Clear();

                        }

                    }
                    transscope.Complete();
                    return true;
                }
                catch (Exception)
                {
                    transscope.Dispose();
                    throw;
                    return false;
                }
            }
        }


    }
}
