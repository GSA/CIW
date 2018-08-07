using MySql.Data.MySqlClient;
using ProcessCIW.Interface;
using ProcessCIW.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessCIW.Utilities
{
    public class DataAccess : IDataAccess
    {
        private readonly ILogTool log;
        private volatile static DataAccess dataAccess;
        private readonly MySqlConnection conn;
        private MySqlCommand cmd = new MySqlCommand();
        private readonly IUtilities U;
        private CIW ciwInformation;
        private int uploaderID;
        MySqlTransaction trans;

        private DataAccess(MySqlConnection conn, IUtilities U, ILogTool log)
        {
            this.conn = conn;
            this.U = U;
            this.log = log;
        }

        public static DataAccess GetInstance(MySqlConnection conn, IUtilities U, ILogTool log)
        {
            object lockingObject = new object();
            if (dataAccess == null)
            {
                lock (lockingObject)
                {
                    if (dataAccess == null)
                    {
                        dataAccess = new DataAccess(conn, U, log);
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
                    cmd.Parameters.Clear();
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
                    cmd.Parameters.Clear();
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

        /// <summary>
        /// Constructor to receive new user data and uploader ID
        /// </summary>
        /// <param name="newUser"></param>
        /// <param name="uploaderID"></param>
        public int InsertCIW(CIW newUser, int uploaderID)
        {
            ciwInformation = newUser;
            this.uploaderID = uploaderID;
            return SaveCIW();
        }

        /// <summary>
        /// Called to start saving the CIW in the database.
        /// All inserts are inside a transaction
        /// Data insertion is all or nothing
        /// </summary>
        /// <returns>Id of person inserted or 0 if fail</returns>
        private int SaveCIW()
        {
            try
            {
                using (conn)
                {
                    conn.Open();

                    trans = conn.BeginTransaction();

                    //Should auto rollback if it fails
                    using (cmd)
                    {
                        int personID = 0;

                        cmd = conn.CreateCommand();

                        cmd.Connection = conn;
                        cmd.Transaction = trans;
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (trans)
                        {
                            log.Info(string.Format("Inserting user {0}", ciwInformation.FullNameForLog));

                            //Call stored procedure "CIW_InsertPerson" and assign to personID
                            personID = InsertNewUser(cmd, "CIW_InsertPerson");

                            if (personID == 0)
                            {
                                //Rollback on failure
                                trans.Rollback();
                                return 0;
                            }

                            //Continue on success
                            if (personID > 0)
                            {
                                log.Info(string.Format("{0} inserted with id {1}", ciwInformation.FullNameForLog, personID));
                                int contractID = 0;

                                if (ciwInformation.ContractorType == "Child Care" || ciwInformation.InvestigationTypeRequested == "Tier 1C")
                                {
                                    log.Info("Inserting/Retrieving Child Care/Tier 1C contract header");

                                    //Insert Contract header of child care worker or tier 1C and assign return value to contractID
                                    contractID = InsertContractHeaderChildCare(cmd, "CIW_InsertContractHeader_ChildCare");
                                }
                                else
                                {
                                    log.Info(String.Format("Inserting/Updating contract header {0} with start date of {1} and end date {2}", ciwInformation.TaskOrderDeliveryOrder, ciwInformation.ContractStartDate, ciwInformation.ContractEndDate));

                                    //Insert Contract header and assign return value to contractID
                                    contractID = InsertOrUpdateContractHeader(cmd, "CIW_InsertOrUpdateContractHeader");
                                }

                                //Continue on success
                                if (contractID > 0)
                                {
                                    log.Info(string.Format("Contract Id is {0}", contractID));
                                    int rows = 0;

                                    //Associate person with contract
                                    rows = AssignContract(cmd, "CIW_InsertContractPerson", personID, contractID);

                                    if (rows > 0)
                                    {
                                        log.Info(String.Format("Successfully associated contract {0} to {1}", ciwInformation.TaskOrderDeliveryOrder, ciwInformation.FullNameForLog));

                                        log.Info(String.Format("Begin inserting {0} vendor POC('s)", ciwInformation.VendorPOC.Count));
                                        foreach (var POC in ciwInformation.VendorPOC)
                                        {
                                            log.Info(String.Format("Inserting vendorPOC {0} {1} with Email:{2}", POC.FirstName, POC.LastName, POC.EMail));

                                            //Insert VendorPOC's by iterating through collection of vendors
                                            InsertVendorPOC(cmd, "CIW_InsertVendorPOC", personID, contractID, POC.FirstName, POC.LastName, POC.WorkPhone, POC.EMail);

                                        }

                                        log.Info(String.Format("Begin inserting {0} GSA POC('s)", ciwInformation.GSAPOC.Count));
                                        foreach (var GSAPOC in ciwInformation.GSAPOC)
                                        {
                                            log.Info(String.Format("Adding GSAPOC with Email:{0}", GSAPOC.EMail));

                                            //Insert GSAPOC's by iterating through collection of POC's
                                            InsertGSAPOC(cmd, "CIW_InsertGSAPOC", personID, contractID, GSAPOC.EMail, GSAPOC.IsPM_COR_CO_CS);
                                        }
                                    }
                                }
                            }

                            trans.Commit();

                            return personID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Insert failed:{0}", ex.Message));

                try
                {
                    trans.Rollback();
                }
                catch (Exception ex2)
                {
                    log.Error(String.Format("Rollback failed:{0}", ex2.Message));
                }

                return 0;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Function that calls stored procedure for inserting a user
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="storedProcedure"></param>
        /// <returns>ID of new user</returns>
        private int InsertNewUser(MySqlCommand cmd, string storedProcedure)
        {

            cmd.CommandText = storedProcedure;
            cmd.Parameters.Clear();

            string persGuid = System.Guid.NewGuid().ToString();
            Byte[] hashedSSNFull;
            Byte[] hashedSSNFour;

            hashedSSNFull = U.HashSSN(ciwInformation.SocialSecurityNumber);
            hashedSSNFour = U.HashSSN(ciwInformation.SocialSecurityNumber.Substring(ciwInformation.SocialSecurityNumber.Length - 4));

            MySqlParameter[] UserParamters = new MySqlParameter[]
                {
                    //section 1 row 1
                    new MySqlParameter { ParameterName = "oPersFamName", Value = ciwInformation.LastName, MySqlDbType = MySqlDbType.VarChar, Size = 60, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersGivenName", Value = ciwInformation.FirstName, MySqlDbType = MySqlDbType.VarChar, Size = 60, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersMiddleName", Value = ciwInformation.MiddleName, MySqlDbType = MySqlDbType.VarChar, Size = 60, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersSuffix", Value = ciwInformation.Suffix == "N/A" ? "" : ciwInformation.Suffix, MySqlDbType = MySqlDbType.VarChar, Size = 12, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersGender", Value = ciwInformation.Sex, MySqlDbType = MySqlDbType.VarChar, Size = 1, Direction = ParameterDirection.Input },

                    //Section 1 - Row 2
                    new MySqlParameter { ParameterName = "oPersSSN", Value = ciwInformation.SocialSecurityNumber, MySqlDbType = MySqlDbType.TinyBlob, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "strBirthDate", Value = ciwInformation.DateOfBirth , MySqlDbType = MySqlDbType.Date, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersBirthCity", Value = ciwInformation.PlaceOfBirthCity , MySqlDbType = MySqlDbType.TinyBlob, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersBirthCountry", Value = ciwInformation.PlaceOfBirthCountry , MySqlDbType = MySqlDbType.TinyBlob, Direction = ParameterDirection.Input },

                    //Use POB:State or POB:MexCan if State is null or empty
                    new MySqlParameter { ParameterName = "oPersBirthState", Value = FieldPicker(ciwInformation.PlaceOfBirthState, ciwInformation.PlaceOfBirthMexicoCanada), MySqlDbType = MySqlDbType.TinyBlob, Direction = ParameterDirection.Input },

                    //Section 1 - Row 3
                    new MySqlParameter { ParameterName = "oPersHomeAddr1", Value = ciwInformation.HomeAddressOne , MySqlDbType = MySqlDbType.TinyBlob, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersHomeAddr2", Value = ciwInformation.HomeAddressTwo , MySqlDbType = MySqlDbType.TinyBlob, Direction = ParameterDirection.Input },

                    //Section 1 - Row 4
                    new MySqlParameter { ParameterName = "oPersHomeCity", Value = ciwInformation.HomeAddressCity , MySqlDbType = MySqlDbType.VarChar, Size = 50, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersHomeCountry", Value = ciwInformation.HomeAddressCountry , MySqlDbType = MySqlDbType.VarChar, Size = 2, Direction = ParameterDirection.Input },

                    //Use US first, then mexcan
                    new MySqlParameter { ParameterName = "oPersHomeState", Value = FieldPicker(ciwInformation.HomeAddressUSState, ciwInformation.HomeAddressMexicoStateCanadaProvince) , MySqlDbType = MySqlDbType.VarChar, Size = 2, Direction = ParameterDirection.Input },

                    new MySqlParameter { ParameterName = "oPersHomeZip", Value = ciwInformation.HomeAddressZip , MySqlDbType = MySqlDbType.VarChar, Size = 10, Direction = ParameterDirection.Input },

                    //Section 1 - Row 5
                    new MySqlParameter { ParameterName = "oPersWorkCell", Value = FormatPhoneNumber(ciwInformation.PhoneNumberWorkCell) , MySqlDbType = MySqlDbType.VarChar, Size = 22, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersWorkPhone", Value = FormatPhoneNumber(ciwInformation.PhoneNumberWork) , MySqlDbType = MySqlDbType.VarChar, Size = 22, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersHomeEmail", Value = ciwInformation.PersonalEmailAddress , MySqlDbType = MySqlDbType.TinyBlob, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersJobTitle", Value = ciwInformation.PositionJobTitle , MySqlDbType = MySqlDbType.VarChar, Size = 60, Direction = ParameterDirection.Input },

                    //Section 1 - Row 6
                    new MySqlParameter { ParameterName = "oPersPriorInvestigation", Value = ConvertYesNo(ciwInformation.PriorInvestigation), MySqlDbType = MySqlDbType.Byte, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersPriorInvestigationDate", Value = U.BeAValidDate(ciwInformation.ApproximiateInvestigationDate) ? ciwInformation.ApproximiateInvestigationDate : (object)DBNull.Value, MySqlDbType = MySqlDbType.VarChar, Size = 12, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersPriorInvestigationWhere", Value = ciwInformation.AgencyAdjudicatedPriorInvestigation , MySqlDbType = MySqlDbType.VarChar, Size = 50, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersIsCitizen", Value = ConvertYesNo(ciwInformation.Citizen) , MySqlDbType = MySqlDbType.Byte, Direction = ParameterDirection.Input },

                    //Section 1 - Row 7
                    new MySqlParameter { ParameterName = "oPersForeignPOE", Value = ciwInformation.PortOfEntryUSCityAndState , MySqlDbType = MySqlDbType.TinyBlob, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersForeignDOE", Value = ciwInformation.DateOfEntry == String.Empty ? (object)DBNull.Value : ciwInformation.DateOfEntry, MySqlDbType = MySqlDbType.TinyBlob, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersForeignRegistration", Value = ciwInformation.AlienRegistrationNumber , MySqlDbType = MySqlDbType.TinyBlob, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersCitizenCountry", Value = ciwInformation.CitzenshipCountry , MySqlDbType = MySqlDbType.VarChar, Size = 2, Direction = ParameterDirection.Input },

                    //Section 3
                    new MySqlParameter { ParameterName = "oPersInvestigationRWANumber", Value = ciwInformation.RWAIAANumber , MySqlDbType = MySqlDbType.VarChar, Size = 20, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersRwaiaaAgy", Value = ciwInformation.RWAIAAAgency , MySqlDbType = MySqlDbType.VarChar, Size = 45, Direction = ParameterDirection.Input },

                    //Section 4
                    //-- WorkBuilding - GSA building number or Other if GSA building number is null
                    new MySqlParameter { ParameterName = "oPersWorkBuilding", Value = FieldPicker(ciwInformation.BuildingNumber, ciwInformation.Other) , MySqlDbType = MySqlDbType.VarChar, Size = 6, Direction = ParameterDirection.Input },

                    new MySqlParameter { ParameterName = "oPersContractorType", Value = ciwInformation.ContractorType == "Building Support" ? "PBS" : ciwInformation.ContractorType , MySqlDbType = MySqlDbType.VarChar, Size = 10, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersContractorCategory", Value = ciwInformation.ArraLongTermContractor , MySqlDbType = MySqlDbType.VarChar, Size = 30, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersMajorOrg", Value = ciwInformation.SponsoringMajorOrg , MySqlDbType = MySqlDbType.VarChar, Size = 2, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersOfficeSymbol", Value = ciwInformation.SponsoringOfficeSymbol , MySqlDbType = MySqlDbType.VarChar, Size = 12, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersRegion", Value = ciwInformation.Region.PadLeft(2,'0') , MySqlDbType = MySqlDbType.VarChar, Size = 3, Direction = ParameterDirection.Input },

                    //Section 5
                    new MySqlParameter { ParameterName = "oPersInvestigationTypeRequested", Value = ciwInformation.InvestigationTypeRequested , MySqlDbType = MySqlDbType.VarChar, Size = 12, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "oPersIsCardRequired", Value = ConvertYesNo(ciwInformation.AccessCardRequired) , MySqlDbType = MySqlDbType.Byte, Direction = ParameterDirection.Input },

                    //Not on CIW
	                new MySqlParameter { ParameterName = "PersGUID", Value = persGuid, MySqlDbType=MySqlDbType.VarChar, Size=36, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "PersHashedSSN", Value = hashedSSNFull, MySqlDbType=MySqlDbType.VarBinary, Size=32, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "PersHashedSSNLastFour", Value = hashedSSNFour, MySqlDbType=MySqlDbType.VarBinary, Size=32, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "UploaderID", Value = uploaderID, MySqlDbType=MySqlDbType.Int32, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "PersID", MySqlDbType=MySqlDbType.Int32, Direction = ParameterDirection.Output },
                    new MySqlParameter { ParameterName = "SQLExceptionWarning", MySqlDbType=MySqlDbType.VarChar, Size=4000, Direction = ParameterDirection.Output },
                };

            cmd.Parameters.AddRange(UserParamters);

            cmd.ExecuteNonQuery();

            log.Info(String.Format("InsertNewUser completed with persId:{0} and SqlException:{1}", cmd.Parameters["PersID"].Value, cmd.Parameters["SQLExceptionWarning"].Value));

            //Returns the Person ID
            return (int)cmd.Parameters["PersID"].Value;
        }

        /// <summary>
        /// removes [.-()] from phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>new phone number</returns>
        private string FormatPhoneNumber(string phoneNumber)
        {
            return phoneNumber.Replace(".", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "");
        }

        /// <summary>
        /// Function that calls stored procedure to insert a contract
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="storedProcedure"></param>
        /// <returns>Contract ID</returns>
        private int InsertOrUpdateContractHeader(MySqlCommand cmd, string storedProcedure)
        {
            cmd.CommandText = storedProcedure;
            cmd.Parameters.Clear();

            MySqlParameter[] ContractHeaderParameters = new MySqlParameter[]
                {
                    new MySqlParameter { ParameterName = "ContractDUNSNumber", Value = ciwInformation.DataUniversalNumberingSystem, MySqlDbType = MySqlDbType.VarChar, Size = 45, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "ContractTaskOrderNumber", Value = ciwInformation.ContractNumberType.Equals("Task Order/Delivery Order Number")?ciwInformation.TaskOrderDeliveryOrder:(object)DBNull.Value, MySqlDbType = MySqlDbType.VarChar, Size = 45, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "ContractNumber", Value = ciwInformation.TaskOrderDeliveryOrder, MySqlDbType = MySqlDbType.VarChar, Size=45, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "ContractDateStart", Value = string.Format("{0:yyyy-MM-dd}", ciwInformation.ContractStartDate) , MySqlDbType = MySqlDbType.Date, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "ContractDateEnd", Value = string.Format("{0:yyyy-MM-dd}", ciwInformation.ContractEndDate) , MySqlDbType = MySqlDbType.Date, Direction = ParameterDirection.Input },

                    new MySqlParameter { ParameterName = "ContractHasOptionYears", Value = ConvertYesNo(ciwInformation.HasOptionYears) , MySqlDbType = MySqlDbType.Bit, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "ContractNumOptionYrs", Value = ciwInformation.NumberOfOptionYears != "" ? ciwInformation.NumberOfOptionYears : "0" , MySqlDbType = MySqlDbType.Int64, Direction = ParameterDirection.Input },

                    new MySqlParameter { ParameterName = "ContractCompanyName", Value = ciwInformation.CompanyName , MySqlDbType = MySqlDbType.VarChar, Size=96, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "UploaderID", Value = uploaderID, MySqlDbType=MySqlDbType.Int32, Direction = ParameterDirection.Input },

                    new MySqlParameter { ParameterName = "ContractID", MySqlDbType=MySqlDbType.Int32, Direction = ParameterDirection.Output },
                    new MySqlParameter { ParameterName = "SQLExceptionWarning", MySqlDbType=MySqlDbType.VarChar, Size=4000, Direction = ParameterDirection.Output },
                };

            cmd.Parameters.AddRange(ContractHeaderParameters);

            cmd.ExecuteNonQuery();

            //Returns the Contract ID
            log.Info(string.Format("InsertOrUpdateContractHeader completed with ContractId:{0} and SqlException:{1}", cmd.Parameters["ContractID"].Value, cmd.Parameters["SQLExceptionWarning"].Value));

            return (int)cmd.Parameters["ContractID"].Value;
        }

        /// <summary>
        /// Function that calls stored procedure to insert child care contract
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="storedProcedure"></param>
        /// <returns>Contract ID</returns>
        private int InsertContractHeaderChildCare(MySqlCommand cmd, string storedProcedure)
        {
            cmd.CommandText = storedProcedure;
            cmd.Parameters.Clear();
            MySqlParameter[] ContractHeaderParameters = new MySqlParameter[]
                {
                    new MySqlParameter { ParameterName = "ContractDUNSNumber", Value = ciwInformation.DataUniversalNumberingSystem, MySqlDbType = MySqlDbType.VarChar, Size = 45, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "ContractTaskOrderNumber", Value = ciwInformation.ContractNumberType.Equals("Task Order/Delivery Order Number")?ciwInformation.TaskOrderDeliveryOrder:(object)DBNull.Value, MySqlDbType = MySqlDbType.VarChar, Size = 45, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "ContractNumber", Value = ciwInformation.TaskOrderDeliveryOrder, MySqlDbType = MySqlDbType.VarChar, Size=45, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "ContractHasOptionYears", Value = ConvertYesNo(ciwInformation.HasOptionYears) , MySqlDbType = MySqlDbType.Bit, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "ContractNumOptionYrs", Value = ciwInformation.NumberOfOptionYears != "" ? ciwInformation.NumberOfOptionYears : "0" , MySqlDbType = MySqlDbType.Int64, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "ContractCompanyName", Value = ciwInformation.CompanyName , MySqlDbType = MySqlDbType.VarChar, Size=96, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "UploaderID", Value = uploaderID, MySqlDbType=MySqlDbType.Int32, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "ContractID", MySqlDbType=MySqlDbType.Int32, Direction = ParameterDirection.Output },
                    new MySqlParameter { ParameterName = "SQLExceptionWarning", MySqlDbType=MySqlDbType.VarChar, Size=4000, Direction = ParameterDirection.Output },
                };
            cmd.Parameters.AddRange(ContractHeaderParameters);
            cmd.ExecuteNonQuery();
            //Returns the Contract ID
            log.Info(string.Format("InsertContractHeader_ChildCare completed with ContractId:{0} and SqlException:{1}", cmd.Parameters["ContractID"].Value, cmd.Parameters["SQLExceptionWarning"].Value));
            return (int)cmd.Parameters["ContractID"].Value;
        }

        /// <summary>
        /// Calls stored procedure that inserts an association between person and contract
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="storedProcedure"></param>
        /// <param name="personId"></param>
        /// <param name="contractId"></param>
        /// <returns>Number of rows affected</returns>
        private int AssignContract(MySqlCommand cmd, string storedProcedure, int personId, int contractId)
        {
            cmd.CommandText = storedProcedure;
            cmd.Parameters.Clear();

            MySqlParameter[] AssignContractParameters = new MySqlParameter[]
                {
                    new MySqlParameter { ParameterName = "ConperContractId", Value = contractId, MySqlDbType = MySqlDbType.Int64, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "ConperPersId", Value =personId , MySqlDbType = MySqlDbType.Int64, Direction = ParameterDirection.Input },

                    //ConperVendorId generated with DateTime.Now.Ticks
                    new MySqlParameter { ParameterName = "ConperVendorId", Value = DateTime.Now.Ticks, MySqlDbType = MySqlDbType.Int64, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "UploaderID", Value = uploaderID, MySqlDbType=MySqlDbType.Int32, Direction = ParameterDirection.Input },

                    new MySqlParameter { ParameterName = "Result", MySqlDbType=MySqlDbType.Int32, Direction = ParameterDirection.Output },
                    new MySqlParameter { ParameterName = "SQLExceptionWarning", MySqlDbType=MySqlDbType.VarChar, Size=4000, Direction = ParameterDirection.Output },
                };
            cmd.Parameters.AddRange(AssignContractParameters);

            cmd.ExecuteNonQuery();

            log.Info(String.Format("AssignContract completed with Result:{0} and SqlException:{1}", cmd.Parameters["Result"].Value, cmd.Parameters["SQLExceptionWarning"].Value));

            //Returns 'Result' from stored procedure that indicates number of rows affected
            return (int)cmd.Parameters["Result"].Value;
        }

        /// <summary>
        /// Calls stored procedure that inserts VendorPOC's
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="storedProcedure"></param>
        /// <param name="personId"></param>
        /// <param name="contractId"></param>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        /// <param name="phone"></param>
        /// <param name="email"></param>
        /// <returns>Number of rows affected</returns>
        private void InsertVendorPOC(MySqlCommand cmd, string storedProcedure, int personId, int contractId, string fName, string lName, string phone, string email)
        {
            cmd.CommandText = storedProcedure;

            cmd.Parameters.Clear();

            MySqlParameter[] VendorPOCParameters = new MySqlParameter[]
                {
                    new MySqlParameter { ParameterName = "NameFirst", Value = fName , MySqlDbType = MySqlDbType.VarChar, Size = 45, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "NameLast", Value = lName , MySqlDbType = MySqlDbType.VarChar, Size = 45, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "Phone", Value = phone , MySqlDbType = MySqlDbType.VarChar, Size = 30, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "Email", Value = email , MySqlDbType = MySqlDbType.VarChar, Size = 100, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "VendPrime", Value = ciwInformation.CompanyName , MySqlDbType = MySqlDbType.VarChar, Size = 255, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "VendSub", Value = ciwInformation.CompanyNameSub , MySqlDbType = MySqlDbType.VarChar, Size = 255, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "ContractID", Value = contractId, MySqlDbType = MySqlDbType.Int64, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "PersID", Value =personId , MySqlDbType = MySqlDbType.Int64, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "UploaderID", Value = uploaderID, MySqlDbType=MySqlDbType.Int32, Direction = ParameterDirection.Input },

                    new MySqlParameter { ParameterName = "Result", MySqlDbType=MySqlDbType.Int32, Direction = ParameterDirection.Output },
                    new MySqlParameter { ParameterName = "SQLExceptionWarning", MySqlDbType=MySqlDbType.VarChar, Size=4000, Direction = ParameterDirection.Output },
                };
            cmd.Parameters.AddRange(VendorPOCParameters);

            cmd.ExecuteNonQuery();

            log.Info(String.Format("InsertVendorPOC completed with Result:{0} and SqlException:{1}", cmd.Parameters["Result"].Value, cmd.Parameters["SQLExceptionWarning"].Value));

        }

        /// <summary>
        /// Calls stored procedure that inserts GSAPOC's
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="storedProcedure"></param>
        /// <param name="personId"></param>
        /// <param name="contractId"></param>
        /// <param name="email"></param>
        /// <param name="roleTypeId"></param>
        /// <returns>Number of rows affected</returns>
        private void InsertGSAPOC(MySqlCommand cmd, string storedProcedure, int personId, int contractId, string email, string roleTypeId)
        {
            cmd.CommandText = storedProcedure;

            cmd.Parameters.Clear();

            MySqlParameter[] GSAPOCParameters = new MySqlParameter[]
                {
                    new MySqlParameter { ParameterName = "SponsorEmail", Value = email , MySqlDbType = MySqlDbType.VarChar, Size= 64, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "RoleTypeID", Value = GetRoleTypeNum(roleTypeId) , MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "PersID", Value =personId , MySqlDbType = MySqlDbType.Int64, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "ContractID", Value = contractId, MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Input },
                    new MySqlParameter { ParameterName = "UploaderID", Value = uploaderID, MySqlDbType=MySqlDbType.Int32, Direction = ParameterDirection.Input },

                    new MySqlParameter { ParameterName = "Result", MySqlDbType=MySqlDbType.Int32, Direction = ParameterDirection.Output },
                    new MySqlParameter { ParameterName = "SQLExceptionWarning", MySqlDbType=MySqlDbType.VarChar, Size=4000, Direction = ParameterDirection.Output },
                };
            cmd.Parameters.AddRange(GSAPOCParameters);

            cmd.ExecuteNonQuery();

            log.Info(String.Format("InsertGSAPOC completed with Result:{0} and SqlException:{1}", cmd.Parameters["Result"].Value, cmd.Parameters["SQLExceptionWarning"].Value));

        }

        /// <summary>
        /// Helper function that returns first choice if not null or white space, else it returns second choice
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Selected string</returns>
        private string FieldPicker(string a, string b)
        {
            if (!string.IsNullOrWhiteSpace(a))
            {
                return a;
            }
            else return b;
        }

        /// <summary>
        /// Converts yes/no to 1/0 for database insertion
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool ConvertYesNo(string value)
        {
            return value.Equals("Yes", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Converts roleTypeId to a numeric representation stored in the database
        /// </summary>
        /// <param name="roleTypeId"></param>
        /// <returns></returns>
        private int GetRoleTypeNum(string roleTypeId)
        {
            if (roleTypeId == "N/A")
                return 0;
            else if (roleTypeId == "CS")
                return 5;
            else if (roleTypeId == "PM")
                return 3;
            else if (roleTypeId == "COR")
                return 2;
            else if (roleTypeId == "CO")
                return 1;
            else return -1;

        }
    }
}
