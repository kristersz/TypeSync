. ".\TypeSync-Functions.ps1"

RunTypeSync "Debug" "ModelGeneration" "..\samples\dotnetcore\Samples.DotNetCore.ViewModels\Samples.DotNetCore.ViewModels.csproj" "C:\Dev\Generated\DotNetCore"
RunTypeSync "Debug" "WebClientGeneration" "..\samples\dotnetcore\Samples.DotNetCore.WebAPI\Samples.DotNetCore.WebAPI.csproj" "C:\Dev\Generated\DotNetCore"
RunTypeSync "Debug" "ValidatorGeneration" "..\samples\dotnetcore\Samples.DotNetCore.ViewModels\Samples.DotNetCore.ViewModels.csproj" "C:\Dev\Generated\DotNetCore"

Tree "C:\Dev\Generated\DotNetCore" /F /A
