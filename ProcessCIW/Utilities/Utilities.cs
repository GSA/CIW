using System;
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
                if ( (_birthDate > DateTime.Now) || (_birthDate >= DateTime.Now.AddYears(-15)) || (_birthDate < new DateTime(1900,1,1)) )
                    return false;
            }
            else
                return false;

            return true;
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
    }
}