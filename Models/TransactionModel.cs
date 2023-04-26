namespace Hangfire_background_jobs.Models
{
    public class TransactionModel
    {

        ////   public int ProjectID { get; set; }
        //   public string Name { get; set; }
        //   public int MemberID { get; set; }
        // //  public int MyProperty { get; set; }

        public int app_id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string Qualificaion { get; set; }
        public string Total_Experience { get; set; }
        public List<Experience> Experiences { get; set; }
    }
    public class Experience
    {
        public int exp_id { get; set; }
        public string company_name { get; set; }
        public string designation { get; set; }
        public int years_worked { get; set; }
        public int app_id { get; set; }
    }
}
