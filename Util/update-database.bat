"%windir%\Microsoft.Net\Framework\v4.0.30319\msbuild.exe" ..\VPS.Dcat.Entities.EF\VPS.Dcat.Entities.EF.csproj /p:outputpath=%temp%\vps.dcat.ef\

.\migrate\migrate.exe VPS.Dcat.Entities.EF.dll /verbose /startupDirectory:%temp%\vps.dcat.ef /startupConfigurationFile:%~dp0..\VPS.Dcat.Web\web.config
