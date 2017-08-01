## Version 1.0.0 - 08/01/2017 ##
* ** code refactor : **code refactored to individual class files. Classes starting with “ET_” are deprecated now and all SDK API objects start with “ET”. 

Project tree structure 
	- FuelSDK-CSharp 	: SDK Project 
	- FuelSDK.Test 	: NUnit test project
	- objsamples 		: Sample/Example project
	- docs 			: SDK API HTML documentation 

* ** NUnit test case : **Nunit test case project added. This covers basic happy path testing. All the test cases use “ET” classes. For samples that uses “ET_” classes, please see “objsamples” project. Advanced and more comprehensive test cases will be added in future releases.

* ** API docs :** added API documentation using doxygen documentation framework. (under docs/ directory)

* **  JWT:** JWT.cs is removed from the project and added as dependency.