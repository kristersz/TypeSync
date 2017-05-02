. ".\TypeSync-Functions.ps1"

RunTypeSync "Debug" "ModelGeneration" "..\samples\dotnetfull\Samples.DotNetFull.ViewModels\Samples.DotNetFull.ViewModels.csproj" "C:\Dev\Generated\DotNetFull"
RunTypeSync "Debug" "WebClientGeneration" "..\samples\dotnetfull\Samples.DotNetFull.WebAPI\Samples.DotNetFull.WebAPI.csproj" "C:\Dev\Generated\DotNetFull"
RunTypeSync "Debug" "ValidatorGeneration" "..\samples\dotnetfull\Samples.DotNetFull.ViewModels\Samples.DotNetFull.ViewModels.csproj" "C:\Dev\Generated\DotNetFull"

Tree "C:\Dev\Generated\DotNetFull" /F /A
