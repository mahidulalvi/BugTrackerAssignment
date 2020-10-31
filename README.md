# bug-tracker

Bug Tracker is an IT Business Management application which is used to track Tickets in various projects involving multiple users. The project is based on the ASP.NET framework's MVC template. 

The project requirements are sourced from `Coder Foundry`. This project was originally completed on May 6, 2019.


## logic

Users of this application are usually involved in different roles of an information technology related organization. As this organization may have many types of projects ongoing, this application will act as an interface to track, record, maintain and support them in terms of debugging issues, implementing new features, database related tasks and other essential tasks based on priority.

All users also have access to all account management features such as registration, login, setting passwords, retrieving forgotten passwords, changing passwords and logout.


# project setup

Clone the project and clean and rebuild solution. Then create a file named 'private.config' in the project directory('BugTracker'). In this file create these xml tags:

```
	<appsettings>
	  <add key="SmtpPassword" value="" />
	  <add key="SmtpHost" value="" />
	  <add key="SmtpPort" value="" />
	  <add key="SmtpFrom" value="" />
	  <add key="SmtpUsername" value="" />
	</appsettings>`
```

Insert your smtp configuration in the tags value attributes. The `private.config` file is
added in gitignore and should not be added to VCS.

After following the above instructions, open the project in Visual Studio and run `update-database` command to initialize the database. Make sure MSSQL Server is installed and running. Installing and using MSSQL Server Management Studio may help in managing the database.

Now the project can be run using `F5`(for debugging) or `CTRL + F5` for running without debugging.

&nbsp;

&nbsp;

&nbsp;


## project requirements

For anyone intersted, the project specific requirements can be found [here](./CoderFoundryProjectRequirements.docx.pdf).