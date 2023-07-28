# User Guide For PSCmdlets

## Cmdlets Folder

This folder contains the cmdlets that provide the interface for this program from C# to PowerShell. The cmdlets themselves contain minimal code, outsourcing all the main code elements to either the Tools or Scripts namespaces.

## Scripts Folder

This folder contains the main functionality of the program, i.e. interacting with data core.

## Tools Folder

This folder contains useful functions such as writing/reading from csv files, writing to console, input data validation.


## Cmdlets Outline

It is important to note that these cmdlets are designed in a way that allows them to be used either as a console app (i.e. user enters values in the console line by line) or by the use of pre-written scripts. The use of pre-written scripts allows the user to write a .ps1 script, define variables and then run the cmdlets directly from this script, this allows for quick execution of repeated procedures.

The cmdlets are designed to work with PowerShell 7 and/or visual studio code with the PowerShell extension installed. These scripts will not work with PowerShell ISE.

For help with the cmdlets type:

```PowerShell
Get-Help -command "Your-Cmdlet"
```

where you replace "Your-Cmdlet with the name of the cmdlet you require help with. 

These cmdlets also feature bespoke help operations in various situations, when these are avaliable they are displayed in the console output and can be accessed by using the "?" key.

### Get Snapshot

- Allows the user to get a snapshot tag value from the data source. 

Parameters:

- DataCoreUrl -> Validated (validated that it is a URL)
- DataSource  -> Unvalidated (for now)
- TagName     -> Unvalidated (for now)



### Get Raw

- Allows the user to get a raw data sample from the data source. Note there is a maximum limit of 100 set on this request.

Parameters:

- DataCoreUrl -> Validated (validated that it is a URL)
- DataSource  -> Unvalidated (for now)
- TagName     -> Unvalidated (for now)
- Samples     -> Validated (less than 100)
- StartDate   -> Validated (Must be parsed to valid DateTime Object)*
- EndDate     -> Validated (Must be parsed to valid DateTime Object)*

*there is further downstream processing that ensures the start date is further in the past than the end date.

### Get Plot

- Allows the user to download a sample of tag data that is optimised for plotting. Ther user has the option whether they would like to save this data to a .csv file.

Parameters:

- DataCoreUrl -> Validated (validated that it is a URL)
- DataSource  -> Unvalidated (for now)
- TagName     -> Unvalidated (for now)
- StartDate   -> Validated (Must be parsed to valid DateTime Object)*
- EndDate     -> Validated (Must be parsed to valid DateTime Object)*
- Interval    -> Unvalidated (Required)
- FilePath    -> Validated (Must parse to valid file path)**

*there is further downstream processing that ensures the start date is further in the past than the end date.
**File path must be csv.


### Get Processed

- Allows the user to get a sample of tag data that has been processed by a data function i.e. interp, avg, min, max. The user has the option to save the data to a csv file.

Parameters:

- DataCoreUrl  -> Validated (validated that it is a URL)
- DataSource   -> Unvalidated (for now)
- TagName      -> Unvalidated (for now)
- StartDate    -> Validated (Must be parsed to valid DateTime Object)*
- EndDate      -> Validated (Must be parsed to valid DateTime Object)*
- DataFunction -> Validated (Must be valid data function)
- Interval    -> Unvalidated (Required)
- FilePath    -> Validated (Must parse to valid file path)**

*there is further downstream processing that ensures the start date is further in the past than the end date.
**File path must be csv.

### Add ScriptTags

- Allows the user to upload script tags to the data core server. The upload must be of csv format.

Parameters:

- DataCoreUrl  -> Validated (validated that it is a URL)
- DataSource   -> Unvalidated (for now)
- Filepath     -> Validated (Must parse to valid file path)*

*There is further downstream processing of the file path, if the file cannot be found an error is returned. 

### Get ExampleScriptTagDefinitions

- Allows the user to download a file of example script tags, for the specified template.

Parameters:

- DataCoreUrl  -> Validated (validated that it is a URL)
- DataSource   -> Unvalidated (for now)
- Filepath     -> Validated (Must parse to valid file path)
- TemolateId   -> Validated (must be a valid template Id)

### Get ScriptTagDefinitions

- Allows the user to download the current script tags for a specified data source.

Parameters:

- DataCoreUrl  -> Validated (validated that it is a URL)
- DataSource   -> Unvalidated (for now)
- Filepath     -> Validated (Must parse to valid file path)
- TemolateId   -> Validated (must be a valid template Id)
- NameFilter   -> Validated (Must be a valid name filter, * selects all)
