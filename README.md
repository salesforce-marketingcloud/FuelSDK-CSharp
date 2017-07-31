FuelSDK-CSharp
============

ExactTarget Fuel SDK for C# 

## Overview ##
The Fuel SDK for C# provides easy access to ExactTarget's Fuel API Family services, including a collection of REST APIs and a SOAP API. These APIs provide access to ExactTarget functionality via common collection types. 

## New Features in Version .9 ##
- **Streamlined Folder Support**: All objects that support folders within the UI now have a standardized property called FolderID.
- **Interaction Support**: Now supports Import and EmailSendDefinition objects .
- **Tracking Events Batching Support**: By default, all tracking event types will only pull new data since the last time a request was made using the same filter.  If you would like to override this functionality to pull all data, simply set the GetSinceLastBatch property to false.
- **Greater Flexibility for Authentication**: Previously the application keys required for authentication had to be hard-coded in an xml config file (FuelSDK_config.xml). While this option is still available, an additional option to pass these at the time the ET_Client class is instantiated allows has been added.  
- **Sandbox (Test) Environment Support**: Support for non-production accounts in the ExactTarget Production Support environment. 

## Requirements ##
- .NET Studio 2010 or higher (WCF)
- .NET Framework 3.5 

## Getting Started ##
The FuelSDK-CSharp solution file includes two projects. One for the actual SDK and one for a web based testing app as an example of how to use the SDK dll and other dependent files.

Rename the  FuelSDK_config.xml.template file in the objsamples project to FuelSDK_config.xml, then edit so you can input the ClientID and Client Secret values provided when you registered your application. If you are building a HubExchange application for the Interactive Marketing Hub then, you must also provide the Application Signature (appsignature). Only change the value for the defaultwsdl configuration item if instructed by ExactTarget.

If you have not registered your application or you need to lookup your Application Key or Application Signature values, please go to App Center at [Code@: ExactTarget's Developer Community](http://code.exacttarget.com "CODE@").

## Example Request ##
All ExactTarget services exposed through the Fuel SDK begin with be prefixed with "ET_". Start by working with the ET_List object:

Add a using statement to reference the FuelSDK's functionality:

>using FuelSDK;

Next, create an instance of the ET_Client class:

>ET_Client myclient = new ET_Client();            

Create an instance of the ExactTarget we want to work with:

>ET_List list = new ET_List();

Associate the ETClient to the object using the client property:

>list.authStub = myclient;

Utilize one of the ET_List methods:

>GetReturn getFR = list.Get();


Print out the results for viewing


>Console.WriteLine("Get Status: " + getFR.Status.ToString());
>
>Console.WriteLine("Message: " + getFR.Message.ToString());
>
>Console.WriteLine("Code: " + getFR.Code.ToString());
>
>Console.WriteLine("Results Length: " + getFR.Results.Length);

>foreach (ET_List ResultList in getFR.Results) <br />
>{<br/>
>&nbsp;&nbsp;&nbsp;Console.WriteLine("--ID: " + ResultList.ID + ", Name: " + ResultList.ListName + ", Description: " + ResultList.Description);
><br/>
>}
</pre>



## ET_Client Class ##
The ET_Client class takes care of many of the required steps when accessing ExactTarget's API, including retrieving appropriate access tokens, handling token state for managing refresh, and determining the appropriate endpoints for API requests. In order to leverage the advantages this class provides, use a single instance of this class for an entire session. Do not instantiate a new ET_Client object for each request made.

The ET_Client class takes 1 parameter which is a NameValueCollection that can be used for passing authentication information for use with SSO with a JWT or for passing ClientID/ClientSecret if you would prefer to not use the config file option. 

Example passing JWT: 
> NameValueCollection parameters = new NameValueCollection();<br>
parameters.Add("clientId", "3bjbc3mg4nbk64z5kzczf89n");<br>
parameters.Add("clientSecret", "ssnGAPvZg6kmm775KPj2Q4Cs");<br>
parameters.Add("jwt", "JWT Value Goes Here");<br>
parameters.Add("appSignature", "j8hj87jhf54gfk54d2lijs");<br>
ET_Client myclient = new ET_Client(parameters);<br>

Example passing ClientID/ClientSecret only: 
> NameValueCollection parameters = new NameValueCollection();<br>
parameters.Add("clientId", "3bjbc3mg4nbk64z5kzczf89n");<br>
parameters.Add("clientSecret", "ssnGAPvZg6kmm775KPj2Q4Cs");<br>
ET_Client myclient = new ET_Client(parameters);<br>

Example passing flag for sandbox environment with ClientID/ClientSecret: 
> NameValueCollection parameters = new NameValueCollection();<br>
parameters.Add("clientId", "3bjbc3mg4nbk64z5kzczf89n");<br>
parameters.Add("clientSecret", "ssnGAPvZg6kmm775KPj2Q4Cs");<br>
parameters.Add("sandbox", "true");<br>
ET_Client myclient = new ET_Client(parameters);<br>

## Responses ##
All methods on Fuel SDK objects return an object that follows the same structure, regardless of the type of call. This object contains a common set of properties used to display details about the request.

- Status: Boolean value that indicates if the call was successful
- Code: HTTP Error Code 
- Message: Text values containing more details in the event of an error
- Results: Typed collection containing the results of the call.

Get Methods also return an addition value to indicate if more information is available (that information can be retrieved using the GetMoreResults method):

- MoreResults: Boolean value that indicates on Get requests if more data is available.


## Samples ##
The objsamples project (included in solution) contains sample calls for all the available functionality.

## Copyright and license ##
Copyright (c) 2013 ExactTarget

Licensed under the MIT License (the "License"); you may not use this work except in compliance with the License. You may obtain a copy of the License in the COPYING file.

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
