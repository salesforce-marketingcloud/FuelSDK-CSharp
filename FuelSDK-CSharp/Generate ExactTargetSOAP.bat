SvcUtil.exe https://webservice.exacttarget.com/ETFramework.wsdl /out:ExactTargetSOAP-Gen.cs /namespace:*,FuelSDK  /noconfig
awk -f fixer.awk ExactTargetSOAP-Gen.cs > ExactTargetSOAP.cs
rm ExactTargetSOAP-Gen.cs
pause