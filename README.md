Salesforce Marketing Cloud Fuel SDK for C# 
============


## Overview ##
The Salesforce Marketing Cloud C# SDK enables developers to easily access the Salesforce Marketing Cloud (formerly ExactTarget) via the C# platform. Among other things, the SDK:
* automatically acquires and refreshes Marketing Cloud access tokens
* enables developers to access both Marketing Cloud SOAP and REST APIs in the same session
* exposes simplified versions of the most commonly used Marketing Cloud objects and methods as C# native objects

## Requirements ##
* .NET Studio 2013 or higher (WCF)
* .NET Framework 4
## Download ##
To use the C# Fuel SDK add the [Salesforce Marketing Cloud Fuel SDK nuget package](https://www.nuget.org/packages/SFMC.FuelSDK/) using the following command:
`Install-Package SFMC.FuelSDK `

## Versions ##
To see the latest feature added to the C# Fuel SDK go to [Releases](https://github.com/salesforce-marketingcloud/FuelSDK-CSharp/releases).

## Getting started ##
The MC SDK has now implemented two authentication possibilities: 
* Legacy authentication
* OAuth 2.0 authentication 

Until August 1st, 2019 both authentication options will be supported.
Starting with August 1st, 2019 only OAuth 2.0 will be supported.

**Configure**  

Rename the `App.config.transform` file in the FuelSDK-CSharp project to `App.config`.

For Legacy authentication, use the below example for `App.config`

```
<?xml version="1.0"?>
<configuration>
  <configSections>
    <section name="fuelSDK" type="FuelSDK.FuelSDKConfigurationSection, FuelSDK" />
  </configSections>
  <fuelSDK
    appSignature="none"
    clientId="YOUR_CLIENT_ID"
    clientSecret="YOUR_CLIENT_SECRET"
    authEndPoint="YOUR_AUTH_TSE/v1/requestToken"
    restEndPoint="YOUR_REST_TSE" />
</configuration>
```

For OAuth2 authentication [More Details][here](https://developer.salesforce.com/docs/atlas.en-us.mc-app-development.meta/mc-app-development/access-token-s2s.htm)., use the below example for `App.config`
```
<?xml version="1.0"?>
<configuration>
  <configSections>
    <section name="fuelSDK" type="FuelSDK.FuelSDKConfigurationSection, FuelSDK" />
  </configSections>
  <fuelSDK
    appSignature="none"
    clientId="YOUR_CLIENT_ID"
    clientSecret="YOUR_CLIENT_SECRET"
    authEndPoint="YOUR_AUTH_TSE"
    restEndPoint="YOUR_REST_TSE"
    useOAuth2Authentication="true" 
    accountId="TARGET_ACCOUNT_ID"
    scope="DATA_ACCESS_PERMISSIONS" />
</configuration>
```

More details and a comparison between legacy and enhanced packages can be found [here](https://developer.salesforce.com/docs/atlas.en-us.mc-app-development.meta/mc-app-development/installed-package-types.htm#).

## Example Request ## 
All Salesforce Marketing Cloud services exposed through the Fuel SDK begin with the "ET" prefix. 

**ETClient Class**

The ETClient class takes care of many of the required steps when accessing Salesforce Marketing Cloud's API, including retrieving appropriate access tokens, handling token state for managing refresh, and determining the appropriate endpoints for API requests. In order to leverage the advantages this class provides, use a single instance of this class for an entire session. Do not instantiate a new ETClient object for each request made.

**ETList object**

Add a using statement to reference the FuelSDK's functionality:

`using FuelSDK;`

Next, create an instance of the ETClient class:

`ETClient myClient = new ETClient();`

Create an instance of the Salesforce Marketing Cloud we want to work with:

`ETList list = new ETList();`

Associate the ETClient to the object using the client property:

`list.AuthStub = myClient;`

Utilize one of the ETList methods:

`GetReturn allLists = list.Get();`

Print out the results for viewing

```
Console.WriteLine("Get Status: " + allLists.Status.ToString());
Console.WriteLine("Message: " + allLists.Message.ToString());
Console.WriteLine("Code: " + allLists.Code.ToString());
Console.WriteLine("Results Length: " + allLists.Results.Length);
foreach (ETList ResultList in allLists.Results)
{
    Console.WriteLine("--ID: " + ResultList.ID + ", Name: " + ResultList.ListName + ", Description: " + ResultList.Description);
}
```

**Responses**

All methods on Fuel SDK objects return an object that follows the same structure, regardless of the type of call. This object contains a common set of properties used to display details about the request.
* Status: Boolean value that indicates if the call was successful.
* Code: HTTP status code.
* Message: Text values containing more details in the event of an error.
* Results: Typed collection containing the results of the call.
Get Methods also return an additional value to indicate if more information is available (that information can be retrieved using the GetMoreResults method):
* MoreResults: Boolean value that indicates on Get requests if more data is available.

**Sample calls**

The *objsamples* project (included in solution) contains sample calls for all the available functionality.


## Contact us ##
* Request a new feature, add a question or report a bug on [GitHub](https://github.com/salesforce-marketingcloud/FuelSDK-CSharp/issues).
* Vote for [Popular Feature Requests](https://github.com/salesforce-marketingcloud/FuelSDK-CSharp/issues?q=is%3Aopen+sort%3Areactions-%2B1-desc+) by making relevant comments and add your [reaction](https://github.com/blog/2119-add-reactions-to-pull-requests-issues-and-comments). Use a reaction in place of a "+1" comment:
    + 👍 - upvote
    + 👎 - downvote

## License ##
By contributing your code, you agree to license your contribution under the terms of the [BSD 3-Clause License](https://github.com/salesforce-marketingcloud/FuelSDK-CSharp/blob/master/license.md).
