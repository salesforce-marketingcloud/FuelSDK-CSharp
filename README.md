FuelSDK-CSharp
============

ExactTarget Fuel SDK for C# 

## Overview ##
The Fuel SDK for C# provides easy access to ExactTarget's Fuel API Family services, including a collection of REST APIs and a SOAP API. These APIs provide access to ExactTarget functionality via common collection types. 

## Requirements ##
- .NET Studio 2010 or higher (WCF)
- .NET Framework 3.5 

## Getting Started ##
The FuelSDK-CSharp solution file includes two projects. One for the actual SDK and one for a web based testing app as an example of how to use the SDK dll and other dependent files.

Rename the  FuelSDK_config.xml.template file in the SDK-Tester-CSharp project to FuelSDK_config.xml, then edit so you can input the ClientID and Client Secret values provided when you registered your application. If you are building a HubExchange application for the Interactive Marketing Hub then, you must also provide the Application Signature (appsignature). Only change the value for the defaultwsdl configuration item if instructed by ExactTarget.

If you have not registered your application or you need to lookup your Application Key or Application Signature values, please go to App Center at [Code@: ExactTarget's Developer Community](http://code.exacttarget.com "CODE@").

## Example Request ##
All ExactTarget services exposed through the Fuel SDK begin with be prefixed with "ET_". Start by working with the ET_List object:

Add a using statement to reference the Fuel SDK's functionality:

>using FuelSDK_Net;

Next, create an instance of the ETClient class:

>ETClient client = new ETClient();

Create an instance of the service we want to work with:

>ET_ListService ListService = new ET_ListService();

Associate the ETClient to the object using the client property:

>ListService.client = client;

Utilize one of the ET_List methods:

>ET_Get get = ListService.Get();

Print out the results for viewing


>foreach (List list in get.results)<br/>
>{<br/>
>&nbsp;&nbsp;&nbsp;lblMessage.Text += "&lt;br>ID: " + list.ID;<br/>
>&nbsp;&nbsp;&nbsp;lblMessage.Text += "&lt;br>Name: " + list.ListName;<br/>
>&nbsp;&nbsp;&nbsp;lblMessage.Text += "&lt;br>Created: " + list.CreatedDate.ToString() + "&lt;br>";<br/>
}<br/>
</pre>



## ETClient Class ##
The ETClient class takes care of many of the required steps when accessing ExactTarget's API, including retrieving appropriate access tokens, handling token state for managing refresh, and determining the appropriate endpoints for API requests. In order to leverage the advantages this class provides, use a single instance of this class for an entire session. Do not instantiate a new ETClient object for each request made.

## Responses ##
All methods on Fuel SDK objects return a generic object that follows the same structure, regardless of the type of call. This object contains a common set of properties used to display details about the request.

- status: Boolean value that indicates if the call was successful
- code: HTTP Error Code 
- message: Text values containing more details in the event of an error
- results: Typed collection containing the results of the call.
- results: Generic collection containing the results of the call.

Get Methods also return an addition value to indicate if more information is available (that information can be retrieved using the getMoreResults method):

- moreResults: Boolean value that indicates on Get requests if more data is available.

And make sure to type the results generic collection anytime you need to interact with the results.

## Samples ##
The SDK-Tester-CSharp project (included in solution) contains sample calls for all the objects.

## Copyright and license ##
Copyright (c) 2013 ExactTarget

Licensed under the MIT License (the "License"); you may not use this work except in compliance with the License. You may obtain a copy of the License in the COPYING file.

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
