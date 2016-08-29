properties { 
  $base_dir = resolve-path .
  $build_dir = "$base_dir\build"
  $35_build_dir = "$build_dir\3.5"
  $40_build_dir = "$build_dir\4.0"
  $packageinfo_dir = "$base_dir\nuspecs"
  $release_dir = "$base_dir\Release"
  $sln_file = "$base_dir\FuelSDK-CSharp.sln"
  $tools_dir = "$base_dir\tools"
  $packages_dir = "$base_dir\packages"
  $version = "0.0.0"
  $35_config = "Release"
  $40_config = "Release.4"
  $run_tests = $false
}
Framework "4.0"

#include .\psake_ext.ps1
	
task default -depends Package

task Clean {
	remove-item -force -recurse $build_dir -ErrorAction SilentlyContinue
	remove-item -force -recurse $release_dir -ErrorAction SilentlyContinue
}

task Init -depends Clean {
	new-item $build_dir -itemType directory
	new-item $release_dir -itemType directory
}

task Compile -depends Init {
	msbuild $sln_file /p:"OutDir=$35_build_dir;Configuration=$35_config" /m
	msbuild $sln_file /target:Rebuild /p:"OutDir=$40_build_dir;Configuration=$40_config" /m
}

task Test -depends Compile -precondition { return $run_tests } {
	$old = pwd
	cd $build_dir
	#& $tools_dir\xUnit\xunit.console.clr4.exe "$35_build_dir\FuelSDK.Tests.dll" /noshadow
	#& $tools_dir\xUnit\xunit.console.clr4.exe "$40_build_dir\FuelSDK.Tests.dll" /noshadow
	cd $old
}

task Dependency {
	$package_files = @(Get-ChildItem . -include *packages.config -recurse)
	foreach ($package in $package_files)
	{
		Write-Host $package.FullName
		& $tools_dir\NuGet.exe install $package.FullName -o packages
	}
}

task Release -depends Dependency, Compile, Test {
	cd $build_dir
	& $tools_dir\7za.exe a $release_dir\FuelSDK-CSharp.zip `
		*\FuelSDK.dll `
		*\FuelSDK.xml `
		*\FuelSDK_config.xml `
		*\Newtonsoft.Json.dll `
		*\Newtonsoft.Json.xml `
    	..\license.txt
	if ($lastExitCode -ne 0) {
		throw "Error: Failed to execute ZIP command"
    }
}

task Package -depends Release {
	$spec_files = @(Get-ChildItem $packageinfo_dir -include *.nuspec -recurse)
	foreach ($spec in $spec_files)
	{
		& $tools_dir\NuGet.exe pack $spec.FullName -o $release_dir -Symbols -BasePath $base_dir
	}
}

task Push -depends Package {
	$spec_files = @(Get-ChildItem $release_dir -include *.nupkg -recurse)
	foreach ($spec in $spec_files)
	{
		& $tools_dir\NuGet.exe push $spec.FullName -source "https://www.nuget.org"
	}
}

