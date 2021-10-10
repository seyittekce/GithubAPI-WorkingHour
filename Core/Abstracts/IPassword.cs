using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Abstracts
{
    public interface IPassword
    {
        string Generate(int lenght, int numberOfNonAlphanumericCharacters);
    }
}