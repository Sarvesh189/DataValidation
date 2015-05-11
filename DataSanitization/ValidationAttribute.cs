using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataValidationLibrary
{
    [AttributeUsage(AttributeTargets.All)]
    public class ValidationAttribute:Attribute
    { 

        public ValidationAttribute(bool isValidationRequired)
        {
            ValidationRequired = isValidationRequired;
        }

        public bool ValidationRequired
        {
            get;
            set;
        }
        public string AllowedCharacters
        {
            get;
            set;
        }

        public Multiplicity AllowedCharactersMultiplicity
        {
            get;
            set;

        }

        public bool ValidateAssociatedObject
        {
            get;
            set;
        }
        public string RegexString
        {
            get;
            set;
        }

        
    }

    public enum Multiplicity
    { 
        ZeroOrOnce,
        ZeroOrMore,
        OneOrMore,

    }
}
