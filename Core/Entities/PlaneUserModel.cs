namespace Core.Entities
{
    public class PlaneUserModel : PlaneRepositoryModel
    {
        public Octokit.User User { get; set; }
    }
}
