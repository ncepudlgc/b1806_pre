# sonycontrol

Open-source integration of Sony Bravia REST API for .NET Core

Projects:

Bravia -> Core Library implementats abstractions

Bravia.Abstractions -> Core abstractions allowing for shared dependency injection.

Bravia.UnitTests -> Location for all unit tests.

Bravia.Api -> Web based testing implementation for the core Bravia library.

Bravia.DeviceHost -> Console application running Kestrel to emulate a Bravia remote host using localhost.


Environment Setup


1. Install latest .NET8 SDK from https://dotnet.microsoft.com/en-us/download/dotnet/8.0
    OR
2. Install Visual Studio 2022 . 


Clean & Build the whole solution

1. Navigate to the SonyBraviaControl solution folder
2. run dotnet clean
3. run dotnet restore
4. run dotnet build


Running the Bravia.Api project

This project is intended to test the core Bravia.csproj implementation of the API found here

https://pro-bravia.sony.net/develop/integrate/rest-api/spec/index.html

How to run the API:
1. Navigate to the Bravia.Api folder
2. run dotnet run
3. from a local browser window, navigate to to http://localhost:5110/swagger or http://localhost:5110/swagger/index.html

General Usage Instructions

Adding a Connection
  1. Generate a random GUID for testing
  2. Select the POST Connections/add option > Try it out
  3. Enter the generated GUID as the ID, as well as
       * the hostname of the Sony Bravia connection.
       * the Pre-Shared Key for authorization.
           * if you do not have a Bravia Device, add a device with hostname localhost:5000 while leaving the key blank, then see instructions to run the Bravia.DeviceHost as an alternate local endpoint for testing the API.
           * if you have an alternate suitable endpoint, you can add it as well!
  4. Select the GET Connections option and select Try it out, then enter the generated GUID and select Execute.

  
Sending a command request
  1. Expand the API endpoint to send and select 'Try it out'
  2. Enter the device ID for the expected connection in the previous section.
  3. Fill in the request paramater values.
  3. Select 'Execute' and check for a 200 status.
  
  

Running the Bravia.DeviceHost project

This project is intended to replicate the core Bravia API and allow the Bravia.Api project to test locally without a Bravia device present.

How to run the Device Host:
1. Navigate to the Bravia.DeviceHost folder from a separate terminal window
2. run dotnet run
3. add the device as a connection using the Bravia.Api project at http://localhost:5110/swagger
4. when sending API requests using this device GUID, the Bravia.DeviceHost will emulate the remote Bravia host API, allowing local testing.


Running the Bravia.UnitTests project
1. Navigate to the Bravia.UnitTests folder.
2. run 'dotnet test'.
