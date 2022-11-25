setlocal
set PATH=%PATH%;C:\opencover;C:\NUnit.Console\bin;C:\Users\mcong\.nuget\packages\reportgenerator\4.8.12\tools\net5.0
OpenCover.Console.exe "-targetargs:Logo.dll" "-target:nunit3-console.exe" "-output:codecoverage.xml"  -filter:"+[Logo]Logo.Core.* -[nunit3-console]*"
ReportGenerator "-reports:codecoverage.xml" "-targetdir:reports"
pause