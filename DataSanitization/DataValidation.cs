using DataValidationLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataValidationLibrary
{
    public class DataValidation : IDataValidation
    {
        // private string _specialCharacters = @"^[a-zA-Z0-9_.,]+(-)?$";
        private string _specialCharacters = @"^[a-zA-Z0-9_.,]+[-]?[a-zA-Z0-9_.,]*$";
        private string _allowedCharacters = string.Empty;
        private string _regexString = string.Empty;
        private Multiplicity _multiplicity = Multiplicity.ZeroOrMore;
        private bool _validateAssociatedObject = false;
        public bool ValidateRequest<T>(T request)
        {
            
            bool isValidRequest = false;
            if (typeof(T) == typeof(String))
            {              
                isValidRequest = ValidateString(Convert.ToString(request));
            }
            else
            {
                var customValidationAttribute = typeof(T).GetCustomAttribute<ValidationAttribute>();
                if(customValidationAttribute != null)
                {
                    _validateAssociatedObject = customValidationAttribute.ValidateAssociatedObject;
                    if (!customValidationAttribute.ValidationRequired) return true;
                }
                 
                
                var propertiesInfo = typeof(T).GetProperties().Where(p => p.GetCustomAttributes(typeof(ValidationAttribute), true).Where(ca => ((ValidationAttribute)ca).ValidationRequired).Any());
                foreach (var propInfo in propertiesInfo)
                {
                    var validationAttribute = propInfo.GetCustomAttribute(typeof(ValidationAttribute)) as ValidationAttribute;
                    if (validationAttribute != null)
                    {
                        _allowedCharacters = validationAttribute.AllowedCharacters;
                        _multiplicity = validationAttribute.AllowedCharactersMultiplicity;
                        _regexString = validationAttribute.RegexString;
                    }
                    if (propInfo.PropertyType == typeof(String))
                    {
                        string propValue = Convert.ToString(propInfo.GetValue(request));
                        isValidRequest = ValidateString(propValue);
                        if (!isValidRequest) break;
                    }
                    else if (propInfo.PropertyType == typeof(IList<string>))
                    {
                        var items = propInfo.GetValue(request) as IList<string>;
                        if (items != null)
                        {
                            foreach (var item in items)
                            {
                                isValidRequest = ValidateString(item);
                                if (!isValidRequest) break;
                            }
                        }
                    }
                    else if (_validateAssociatedObject && !(propInfo.PropertyType.IsValueType || propInfo.PropertyType.IsEnum || propInfo.PropertyType.IsPrimitive))
                    {
                        if (propInfo.PropertyType.Name == typeof(IList<>).Name)
                        {
                            var items = propInfo.GetValue(request) as IList;
                            foreach (var item in items)
                            {
                                isValidRequest = ValidateAssociatedObjects(item);
                                if (!isValidRequest) return false;
                            }
                        }
                        else
                        {
                            var typedatavalidation = this.GetType();

                            var minfo = typedatavalidation.GetMethods().FirstOrDefault(m => m.IsGenericMethod);

                            var validationMethod = minfo.MakeGenericMethod(propInfo.PropertyType);
                            object[] args = { propInfo.GetValue(request) };
                            var result = validationMethod.Invoke(this, args);
                            isValidRequest = (bool)result;
                        }
                    }

                }
            }
            return isValidRequest;
        }

        private bool ValidateAssociatedObjects(object request)
        {
            var typedatavalidation = this.GetType();

            var minfo = typedatavalidation.GetMethods().FirstOrDefault(m => m.IsGenericMethod);

            var validationMethod = minfo.MakeGenericMethod(request.GetType());
            object[] args = { request };
            var result = validationMethod.Invoke(this, args);
            return (bool)result;

        }

        private string GetSpecialCharacters()
        {
            string tempSpecialcharactersFormat = string.Empty;
            if (!string.IsNullOrEmpty(_allowedCharacters))
            {
                switch (_multiplicity)
                {
                    case Multiplicity.OneOrMore: tempSpecialcharactersFormat = _specialCharacters.Insert(14, _allowedCharacters); break;
                    case Multiplicity.ZeroOrOnce: tempSpecialcharactersFormat = _specialCharacters.Insert(17, _allowedCharacters); break;
                    case Multiplicity.ZeroOrMore: tempSpecialcharactersFormat = _specialCharacters.Insert(30, _allowedCharacters); break;
                }

            }
            else
            {
                tempSpecialcharactersFormat = _specialCharacters;
            }
            return tempSpecialcharactersFormat;
        }


        private bool ValidateString(string request)
        {
            if (!string.IsNullOrEmpty(request))
            {
                string specialCharacters = string.Empty;
                if (!string.IsNullOrEmpty(_regexString))
                {
                    specialCharacters = _regexString;
                }
                else
                {
                    specialCharacters = GetSpecialCharacters();
                }


                Regex rgx = new Regex(specialCharacters);
                return rgx.IsMatch(request);
            }
            return true;
        }




        public bool ValidateRequest(string request, string regex)
        {
            _regexString = regex;
            return ValidateString(request);
        }

        public bool ValidateRequest(string request, string allowedCharacters, Multiplicity allowedCharactersMultiplicity)
        {
            _allowedCharacters = allowedCharacters;
            _multiplicity = allowedCharactersMultiplicity;
            return ValidateString(request);
        }
    }
}
