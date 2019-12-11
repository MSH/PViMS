REM Ensure VS Build Tools Are Installed Prior to running this build
REM VS 2017 https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=BuildTools&rel=15
REM VS 2019 https://download.visualstudio.microsoft.com/download/pr/aab801bf-dcd0-4d7c-8552-a0c3b4fee032/5a2cee2a57d38e90f6a555044782097f/vs_buildtools.exe
REM Ensure "Web development build tools" is installed on the build workstation

"C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\msbuild.exe" pvims.sln /t:pvims_web:Rebuild /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=FolderProfile /p:DebugSymbols=false /p:DebugType=None