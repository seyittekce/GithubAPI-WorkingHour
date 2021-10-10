using System.Collections.Generic;


namespace Core.Entities
{
    public class RepositoryModel
    {
        public string Name { get; set; } = "";
        public string Owner { get; set; } = "";
        public string TotalWorkingHour { get; set; } = "0";
        public IEnumerable<RepositoryWorkingDays> RepositoryWorkingDays { get; set; } =
            new List<RepositoryWorkingDays>();
    }

    public class RepositoryModelWithMaxLength
    {
        public List<RepositoryModel> Repository {  get; set; }
        public int MaxLength {  get; set; }
    }
}
