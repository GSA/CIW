using MySql.Data.MySqlClient;
using ProcessCIW.Interface;
using ProcessCIW.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ProcessCIW.Utilities
{
    public class DataAccess : IDataAccess
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private volatile static DataAccess dataAccess;
        private readonly MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString());
        private readonly MySqlCommand cmd = new MySqlCommand();
        private readonly IUtilities U = new Utilities();

        private DataAccess() { }

        public static DataAccess GetInstance()
        {
            object lockingObject = new object();
            if (dataAccess == null)
            {
                lock (lockingObject)
                {
                    if (dataAccess == null)
                    {
                        dataAccess = new DataAccess();
                    }
                }
            }
            return dataAccess;
        }

        /// <summary>
        /// Verifies that email provided is a valid GSA POC email
        /// </summary>
        /// <param name="workEMail"></param>
        /// <returns></returns>
        public bool BeAValidEMail(string workEMail)
        {
            try
            {
                using (conn)
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    using (cmd)
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.CommandText = "CIW_IsValidGSAPOC";

                        cmd.Parameters.Clear();

                        MySqlParameter[] sponsorParameters = new MySqlParameter[]
                        {
                            new MySqlParameter { ParameterName = "workEMail", Value = workEMail, MySqlDbType = MySqlDbType.VarChar, Size = 64, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "rowsReturned", MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Output }
                        };

                        cmd.Parameters.AddRange(sponsorParameters);

                        cmd.ExecuteNonQuery();

                        var rows = cmd.Parameters["rowsReturned"].Value;

                        log.Info(string.Format("CIW_IsValidGSAPOC returned with {0} rows and result: {1}", rows, (int)rows > 0));

                        if ((int)rows == 1)
                            return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }
    

    /// <summary>
    /// Returns whether or not the user entered in a valid building.
    /// </summary>
    /// <param name="buildingID"></param>
    /// <returns>Bool</returns>
    public bool BeAValidBuilding(string buildingID)
        {
            try
            {
                using (conn)
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    using (cmd)
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.CommandText = "CIW_IsValidBuilding";

                        cmd.Parameters.Clear();

                        MySqlParameter[] buildingParameters = new MySqlParameter[]
                        {
                            new MySqlParameter { ParameterName = "buildingID", Value = buildingID, MySqlDbType = MySqlDbType.VarChar, Size = 6, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "rowsReturned", MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Output }
                        };

                        cmd.Parameters.AddRange(buildingParameters);

                        cmd.ExecuteNonQuery();

                        if ((int)cmd.Parameters["rowsReturned"].Value == 1)
                            return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }    

    /// <summary>
    /// Calls stored procedure to help validate if state and country codes for Mexico/Canada match
    /// </summary>
    /// <param name="state"></param>
    /// <param name="country"></param>
    /// <returns></returns>
    public bool ValidateState(string state, string country)
        {
            try
            {
                using (conn)
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    using (cmd)
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "CIW_ValidateStateNonUS";
                        cmd.Parameters.Clear();

                        MySqlParameter[] userParameters = new MySqlParameter[]
                        {
                            new MySqlParameter { ParameterName = "State", Value = state, MySqlDbType = MySqlDbType.VarChar, Size = 2, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "CountryMXCA", Value = country, MySqlDbType = MySqlDbType.VarChar, Size = 2, Direction = ParameterDirection.Input },

                            new MySqlParameter { ParameterName = "IsValid", MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Output }
                        };

                        cmd.Parameters.AddRange(userParameters);
                        cmd.ExecuteNonQuery();

                        if ((int)cmd.Parameters["IsValid"].Value != 1)
                            return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calls stored procedure to check if SSN is duplicate
        /// </summary>
        /// <param name="ssn"></param>
        /// <returns></returns>
        public bool NotBeADuplicateSSN(string ssn)
        {
            try
            {
                using (conn)
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    using (cmd)
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.CommandText = "CIW_DuplicateSSNFound";

                        cmd.Parameters.Clear();

                        MySqlParameter[] userParameters = new MySqlParameter[]
                        {
                            new MySqlParameter { ParameterName = "personSSN", Value = ssn, MySqlDbType = MySqlDbType.VarChar, Size = 20, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "duplicateSSN", MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Output }
                        };

                        cmd.Parameters.AddRange(userParameters);

                        cmd.ExecuteNonQuery();

                        if ((int)cmd.Parameters["duplicateSSN"].Value > 0)
                            return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calls stored procedure that checks if a duplicate user exists
        /// </summary>
        /// <param name="lastName"></param>
        /// <param name="dob"></param>
        /// <param name="ssn"></param>
        /// <returns></returns>
        public bool NotBeADuplicateUser(string lastName, string dob, string ssn)
        {
            bool result;
            try
            {
                using (conn)
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    using (cmd)
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.CommandText = "CIW_DoesUserExist";

                        cmd.Parameters.Clear();

                        MySqlParameter[] userParameters = new MySqlParameter[]
                        {
                            new MySqlParameter { ParameterName = "lastName", Value = lastName, MySqlDbType = MySqlDbType.VarChar, Size = 60, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "personSSN", Value = ssn, MySqlDbType = MySqlDbType.VarChar, Size = 20, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "personDOB", Value = U.FormatDate(dob), MySqlDbType = MySqlDbType.VarChar, Size = 20, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "rowsReturned", MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Output }
                        };

                        cmd.Parameters.AddRange(userParameters);

                        cmd.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + " - " + ex.InnerException);
                return false;
            }

            result = ((int)cmd.Parameters["rowsReturned"].Value == 0);
            log.Info(String.Format("NotBeADuplicateUser completed with rowsReturned:{0} and return value:{1}", cmd.Parameters["rowsReturned"].Value, result));
            return result;
        }

        /// <summary>
        /// Updates upload table after finished processing by calling stored procedure
        /// </summary>
        /// <param name="documentID"></param>
        /// <param name="processedResult"></param>
        public void UpdateProcessed(int documentID, int processedResult)
        {
            log.Info(string.Format("Updating processed document {0} with result {1}", documentID, processedResult));

            using (conn)
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                using (cmd)
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "CIW_UpdateProcessed";
                    cmd.CommandType = CommandType.StoredProcedure;

                    MySqlParameter[] ProcessedParameters = new MySqlParameter[]
                    {
                        new MySqlParameter { ParameterName = "documentID", Value = documentID, MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Input },
                        new MySqlParameter { ParameterName = "processedResult", Value = processedResult, MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Input },
                        new MySqlParameter { ParameterName = "SQLExceptionWarning", MySqlDbType=MySqlDbType.VarChar, Size=4000, Direction = ParameterDirection.Output },
                    };

                    cmd.Parameters.AddRange(ProcessedParameters);

                    cmd.ExecuteNonQuery();

                    log.Info(string.Format("CIW_UpdateProcessed completed and error message: {0}", cmd.Parameters["SQLExceptionWarning"].Value));
                }
            }
        }

        /// <summary>
        /// Gets a list of unprocessed files by calling a stored procedure
        /// </summary>
        /// <returns>List of unprocessed files</returns>
        public List<UnprocessedFiles> GetUnprocessedFiles()
        {
            List<UnprocessedFiles> uf = new List<UnprocessedFiles>();

            using (conn)
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                using (cmd)
                {
                    MySqlDataReader unprocessedFiles;

                    cmd.Connection = conn;
                    cmd.CommandText = "CIW_Unprocessed";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("SQLExceptionWarning", MySqlDbType.VarChar, 4000);

                    unprocessedFiles = cmd.ExecuteReader();

                    while (unprocessedFiles.Read())
                    {
                        uf.Add(
                                new UnprocessedFiles
                                {
                                    ID = (int)unprocessedFiles[0],
                                    PersID = (int)unprocessedFiles[1],
                                    FileName = unprocessedFiles[2].ToString()
                                }
                              );
                    }
                    log.Info(string.Format("CIW_Unprocessed returned with {0} unprocessed files and SQLExceptionWarning:{1}", uf.Count, cmd.Parameters["SQLExceptionWarning"].Value));

                    return uf;
                }
            }           
        }

        public DataSet GetFipsCodeFromCountryName(string placeOfBirthCountryName, string homeCountryName, string citizenshipCountryName)
        {
            DataSet DS = new DataSet();

            log.Info("Getting fips code from database");
            try
            {
                using (conn)
                {
                    using (cmd)
                    {
                        using (MySqlDataAdapter DA = new MySqlDataAdapter(cmd))
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = "uspGetFipsCode";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Clear();

                            MySqlParameter[] userParameters = new MySqlParameter[]
                            {
                            new MySqlParameter { ParameterName = "placeOfBirthCountryNameShort", Value = placeOfBirthCountryName, MySqlDbType = MySqlDbType.VarChar, Size = 60, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "homeCountryName", Value = homeCountryName, MySqlDbType = MySqlDbType.VarChar, Size = 60, Direction = ParameterDirection.Input },
                            new MySqlParameter { ParameterName = "citizenshipCountryName", Value = citizenshipCountryName, MySqlDbType = MySqlDbType.VarChar, Size = 60, Direction = ParameterDirection.Input },

                            new MySqlParameter { ParameterName = "SQLExceptionWarning", MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Output }
                            };

                            cmd.Parameters.AddRange(userParameters);

                            conn.Open();
                            DA.Fill(DS);

                            if (DS.Tables[0].Rows.Count > 0 && DS.Tables[1].Rows.Count > 0 && DS.Tables[2].Rows.Count > 0)
                            {
                                log.Info(String.Format("Get Fips code returned {0}, {1}, and {2}", DS.Tables[0].Rows[0].ItemArray[0].ToString(), DS.Tables[1].Rows[0].ItemArray[0].ToString(), DS.Tables[2].Rows[0].ItemArray[0].ToString()));
                            }
                            else
                            {
                                log.Warn("Dataset has an empty row. Can be caused by not selecting a country.");
                            }
                            return DS;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + " - " + ex.InnerException);
                throw;
            }
        }

        /// <summary>
        /// Function that calls stored procedure to retrieve uploader information based on uploader ID.
        /// Values converted to strings and stored in previously declared variables.
        /// </summary>
        public UploaderInformation GetUploaderInformation(int uploaderID)
        {
            UploaderInformation uploaderInfo = new UploaderInformation();            

            using (conn)
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                using (cmd)
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "CIW_GetUploaderInfo";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Clear();

                    MySqlParameter[] ContractHeaderParameters = new MySqlParameter[]
                    {
                        new MySqlParameter { ParameterName = "UploaderId", Value = uploaderID, MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Input },

                        new MySqlParameter { ParameterName = "PrefixedName", MySqlDbType=MySqlDbType.VarChar, Size=64, Direction = ParameterDirection.Output },
                        new MySqlParameter { ParameterName = "ZoneLetter", MySqlDbType=MySqlDbType.VarChar, Size=2, Direction = ParameterDirection.Output },
                        new MySqlParameter { ParameterName = "ZoneEmail", MySqlDbType=MySqlDbType.VarChar, Size=64, Direction = ParameterDirection.Output },
                        new MySqlParameter { ParameterName = "ZonePhone", MySqlDbType=MySqlDbType.VarChar, Size=22, Direction = ParameterDirection.Output },
                        new MySqlParameter { ParameterName = "UploaderWorkEmail", MySqlDbType=MySqlDbType.VarChar, Size=64, Direction = ParameterDirection.Output },
                        new MySqlParameter { ParameterName = "UploaderMajorOrg", MySqlDbType=MySqlDbType.VarChar, Size=2, Direction = ParameterDirection.Output },
                    };

                    cmd.Parameters.AddRange(ContractHeaderParameters);

                    cmd.ExecuteNonQuery();

                    uploaderInfo.PrefixedName = (string)cmd.Parameters["PrefixedName"].Value;
                    uploaderInfo.ZoneEmail = (string)cmd.Parameters["ZoneEmail"].Value;
                    uploaderInfo.UploaderWorkEmail = (string)cmd.Parameters["UploaderWorkEmail"].Value;
                    uploaderInfo.UploaderMajorOrg = (string)cmd.Parameters["UploaderMajorOrg"].Value;

                    log.Info(string.Format("GetUploaderInformation completed with PrefixedName:{0} UploaderWorkEmail:{1} UploaderMajorOrg:{2}", uploaderInfo.PrefixedName, uploaderInfo.UploaderWorkEmail, uploaderInfo.UploaderMajorOrg));

                    return uploaderInfo;
                }
            }
        }     
    }
}
