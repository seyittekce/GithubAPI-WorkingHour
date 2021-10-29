namespace Core.Abstracts
{
    public interface IPassword
    {
        string Generate(int lenght, int numberOfNonAlphanumericCharacters);
    }
}