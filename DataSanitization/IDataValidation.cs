using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataValidationLibrary
{
   public interface IDataValidation
    {
       bool ValidateRequest<T>(T request);
       bool ValidateRequest(string request, string regex);

       bool ValidateRequest(string request, string allowedCharacters,  Multiplicity allowedCharactersMultiplicity);
    }
}
