Function RunTypeSync ($Configuration, $UseCase, $InputPath, $OutputPath)
{
    $executable = "..\src\TypeSync\bin\$($Configuration)\TypeSync.exe"

    & $executable -u $UseCase -i $InputPath -o $OutputPath
}
