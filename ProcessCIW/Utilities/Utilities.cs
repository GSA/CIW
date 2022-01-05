﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ProcessCIW.Utilities
{
    sealed class Utilities
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string RemoveDateFromFilename(string s)
        {
            int _pos = s.LastIndexOf('_');
            int _dot = s.LastIndexOf('.');
            if (_pos < 0)
                return s;
            return s.Remove(_pos, _dot - _pos);
        }

        /// <summary>
        /// Returns false if string is white space
        /// Return true if null or empty
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNotWhiteSpace(string s)
        {
            if (s == null)
                return true;
            if (s.Length > 0)
                return s.Trim().Length != 0;
            else return true;
        }

        /// <summary>
        /// Checks if Start date is before end date and end date is later than current date
        /// </summary>
        /// <param name="contractStartDate"></param>
        /// <param name="contractEndDate"></param>
        /// <returns>Bool</returns>
        public static bool StartBeforeEnd(string StartDate, string EndDate)
        {
            //parse into datetime or icomparable data type then check if StartDate < EndDate
            //Note: string is comparable but strings in mm/dd/yyyy format cannot be compared properly
            DateTime _StartDate;
            DateTime _EndDate;
            DateTime Today = DateTime.Now.Date;

            if (DateTime.TryParse(EndDate, out _EndDate) && DateTime.TryParse(StartDate, out _StartDate))
            {
                return ((_StartDate < _EndDate)); // && (Today < _EndDate));
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if end date is greater than current date
        /// </summary>
        /// <param name="End"></param>
        /// <returns></returns>
        public static bool EndIsFutureDate(string End)
        {
            //parse into datetime or icomparable data type then check if EndDate > DateTime.Now.Date
            //Note: string is comparable but strings in mm/dd/yyyy format cannot be compared properly
            DateTime EndDate;
            DateTime Today = DateTime.Now.Date;

            if (DateTime.TryParse(End, out EndDate))
            {
                return ((Today < EndDate));
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if date provided is less than num of years in the future
        /// </summary>
        /// <param name="Start"></param>
        /// <returns>Bool</returns>
        public static bool BeAValidEndDate(string End,int NumOfYearsInTheFuture)
        {
            //parse into datetime or icomparable data type
            DateTime EndDate;
            if (DateTime.TryParse(End, out EndDate))
            {
                if (EndDate >= DateTime.Now.AddYears(NumOfYearsInTheFuture))
                    return false;
            }
            else
                return false;

            return true;
        }

        /// <summary>
        /// Checks if date given was prior to current date
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static bool DateIsValidAndNotFuture(string Date)
        {
            DateTime _Date;
            DateTime Today = DateTime.Now.Date;

            if (DateTime.TryParse(Date, out _Date))
            {
                return ((_Date <= Today));
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if date given can be parsed into datetime
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Bool</returns>
        public static bool BeAValidDate(string date)
        {
            DateTime _date;

            return DateTime.TryParse(date, out _date);
        }

        /// <summary>
        /// Checks if birth date given is valid
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Bool</returns>
        public static bool BeAValidBirthDate(string date)
        {
            DateTime _birthDate;

            if (DateTime.TryParse(date, out _birthDate))
            {
                return ((_birthDate <= DateTime.Now.AddYears(-14)) && (_birthDate > DateTime.Now.AddYears(-100)));
            }
            else return false;
        }

        /// <summary>
        /// Parses a datetime from string then formats in yyyy-MM-dd format
        /// </summary>
        /// <param name="birthDate"></param>
        /// <returns></returns>
        public static string FormatDate(string birthDate)
        {
            DateTime dateOfBirth;

            DateTime.TryParse(birthDate, out dateOfBirth);

            return string.Format("{0:yyyy-MM-dd}", dateOfBirth);
        }

        /// <summary>
        /// Takes a string and return only the numbers in the original string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string TrimPhoneNum(string s)
        {
            return Regex.Replace(s, "[^0-9]", "");
        }

        public static string TrimPoundSign(string s)
        {
            return s.Replace("#", string.Empty);
        }

        public static string CleanSsn(string s)
        {
            return s.Replace("-", string.Empty).Replace(" ", string.Empty).Trim();
        }        

        /// <summary>
        /// SHA256 Hash of the SSN, pass in the full 9 or the last 4
        /// </summary>
        /// <param name="ssn"></param>
        /// <returns>Hashed version of SSN passed in</returns>
        public byte[] HashSSN(string ssn)
        {
            byte[] hashedSSN = null;

            SHA256 shaM = new SHA256Managed();
            
            //Using UTF8 because this only contains ASCII text
            hashedSSN = shaM.ComputeHash(Encoding.UTF8.GetBytes(ssn));

            shaM.Dispose();

            return hashedSSN;
        }

        /// <summary>
        /// Adds -d.docx to end of filename
        /// </summary>
        /// <param name="encryptedFilename"></param>
        /// <returns></returns>
        public string GenerateDecryptedFilename(string encryptedFilename)
        {
            return string.Concat(encryptedFilename, "-d.docx");
        }

        /// <summary>
        /// Deletes all files in the list of files passed in
        /// </summary>
        /// <param name="filesToDelete"></param>
        public static void DeleteFiles(List<string> filesToDelete)
        {
            foreach (var file in filesToDelete)
            {
                log.Info(string.Format("Deleting CIW file {0}.", file));
                File.Delete(file);
            }
        }

        /// <summary>
        /// Checks if contract number follows nomenclature for fas contract
        /// </summary>
        /// <param name="contractnumber"></param>
        /// <returns>Bool</returns>
        public static bool validFAS(string contractnumber)
        {
            bool FASvalid = Regex.IsMatch(contractnumber, @"^(GS\d{2}[A-Z]\w{5})|(47Q[A-Z]{3}\d\d\w{5})|(CM\d{6}CT\d{4})|(DTTS\d{4}D\d{5})|(GS\d{2}\w\d{2}[A-Z]{3}\w{4})$");

            if (FASvalid)
            {
                return true;
            }
            
            else return false;
            
        }


        public static bool validcontractnumber(string contractnumber)
        {
            bool ChildCarevalid = Regex.IsMatch(contractnumber, @"^(\d{4})$");
            bool IAAvalid = Regex.IsMatch(contractnumber, @"^(IAA)(\d\w)*$");
            bool MOUvalid = Regex.IsMatch(contractnumber, @"^(MOU)(\d\w)*$");
            bool MOAvalid = Regex.IsMatch(contractnumber, @"^(MOA)(\d\w)*$");

            if (ChildCarevalid || IAAvalid || MOUvalid || MOAvalid)
            {
                return true;
            }

            else return false;

        }

        public static bool validLeaseAndRandolphcontractnumber(string contractnumber)
        {
            bool Leasevalid = Regex.IsMatch(contractnumber, @"^[Ll](A[LKSZRAEP]|C[AOT]|D[EC]|F[LM]|G[AU]|HI|I[ADLN]|K[SY]|LA|M[ADEHINOPST]|N[CDEHJMVY]|O[HKR]|P[ARW]|RI|S[CD]|T[NX]|UT|V[AIT]|W[AIVY])(\d{5})$");
            bool Randolphvalid = Regex.IsMatch(contractnumber, @"^(RS)(A[LKSZRAEP]|C[AOT]|D[EC]|F[LM]|G[AU]|HI|I[ADLN]|K[SY]|LA|M[ADEHINOPST]|N[CDEHJMVY]|O[HKR]|P[ARW]|RI|S[CD]|T[NX]|UT|V[AIT]|W[AIVY])(\d{4})(\w{2,3})(\d*)$");
            bool CreditUvalid = Regex.IsMatch(contractnumber, @"^(CU)(A[LKSZRAEP]|C[AOT]|D[EC]|F[LM]|G[AU]|HI|I[ADLN]|K[SY]|LA|M[ADEHINOPST]|N[CDEHJMVY]|O[HKR]|P[ARW]|RI|S[CD]|T[NX]|UT|V[AIT]|W[AIVY])([0-9]{4})$");
            bool RevokeLicensevalid = Regex.IsMatch(contractnumber, @"^(RL)(\d\w)*$");


            if (Leasevalid || Randolphvalid || CreditUvalid || RevokeLicensevalid)
            {
               
                    return true;
            }
                else return false;

        }

        public static bool validchildcare(string contractnumber)
        {
           
            bool ChildCarevalid = Regex.IsMatch(contractnumber, @"^(\d{4})$");

            if (ChildCarevalid)
            {
                return true;
            }

            else return false;

        }

    }
}