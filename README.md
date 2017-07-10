# CIW

## Description: 
The CIW application retrieves all unprocessed files based on the list of unprocessed files in the Upload table in the database. It will then convert these files to csv, import the data, perform validation, and database checks. If all fields pass it will insert the data into the database, start the sponsorsip/adjudication process. It will then send out appropriate emails to indicate pass/fail status of the upload procedure and directions on next steps.

## Nuget packages and other dll's utilized
* CsvHelper
* DocumentFormat.OpenXml
* FluentValidation
* log4net

## Initial Setup
### Default config setup

The repository contains a web config that points to external config files. These external config files not controlled by version control will need to be created and configured prior to running the application. The files required and the default configuration can be found below. For those on the development team, additional details can be found in the documentation on the google drive in the GIT team drive.


 * **Things to do before your first commit**
   * Make a new branch for development. All pre-existing branches are protected and cannot be pushed to directly.
   * You can publish a new branch and do pull requests to have your changes incorporated into the project.
   * Once you have created a new branch you will need to create the config files. (see below for more info on this)
   * Default version of these files are provided in the repo with the .example extension
   * Copy these files back into the project and change the extension to .config using the previous filename
   * Or create new files that contain the code as seen below
   * After the config files are created go to the commit screen, right click on each of the new files and click exclude. 
   * This will move them to the excluded section of the commit window. 
   * Do not push your config files to the repository. Pull requests that include these files will be rejected.
 
 * **Current config files that will need to be added.**
   * ConnectionStrings.config
   * AppSettings.config
 
* **Default settings for these files will follow this line**
 
   * **ConnectionStrings.config file should contain the following lines.** 
    ~~~ xml
    <connectionStrings>
    <add name="connection name" connectionString="database connection string" />
    </connectionStrings>
    ~~~

   * **AppSettings.config should contain the following lines.**
  ~~~ xml
  <appSettings>
  <add key="DEBUGMODE" value="false" />
  <add key="VERSION" value="V1"/>
  <add key="SMTPSERVER" value="smtp.server.com" />
  <add key="DEFAULTSUBJECT" value="Automated CIW Process" />
  <add key="SUMMARYEMAIL" value="replaceme@example.com" />
  <add key="DEFAULTEMAIL" value="replaceme@example.com" />
  <add key="ONBOARDINGLOCATION" value="onboarding folder path"/>
  <add key="EMAILTEMPLATESLOCATION" value="email template folder path" />
  <add key="CIWPRODUCTIONFILELOCATION" value="production folder path" />
  <add key="CIWDEBUGFILELOCATION" value="debug folder path" />
  <add key="FASEMAIL" value="replaceme@example.com"/>
  <add key="TEMPFOLDER" value="temp folder path" />
  <add key="Salt" value="salt" />
  <add key="EPass" value="epass" />
  <add key="ClientSettingsProvider.ServiceUri" value="" />
  </appSettings>
  ~~~
  
  ***
  
## Usage
CIW V1 files can be inserted into the CIWDEBUGFILELOCATION folder, then set debug to true and you can process these files by starting the application. 
Note that usage of the application will require access to a database with a schema that is mirrored from the current production database.

## Contributing
Fork this repository, make changes in your fork, and then submit a pull-request, remembering not to upload any system specific configuration files, PII, or sensitive data of any type. 

## Credits
GSA
