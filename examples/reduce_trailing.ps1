$env:COMPlus_TieredCompilation='0'
$env:COMPlus_JitEnableEHWriteThru='0'
Get-Content -Path ..\Fuzzlyn\bin\Release\publish\Execution_Mismatch.txt -Wait | % {
	if ($_ -notmatch '^Seed: [0-9]+$') {
	    return
	}

	$seed = ($_ -split ' ')[1]
	if (Test-Path "reduced\\$seed.cs") {
		Write-Host "Skipping $seed because it is already reduced"
		return
	}
	
	Write-Host "Reducing $seed"
	& ..\Fuzzlyn\bin\Release\publish\Fuzzlyn.exe --seed=$seed --reduce > "reduced\\$seed.cs"
	if ($lastexitcode -ne 0) {
		Write-Host "  ..got error exit code ($lastexitcode). Reducing with sub processes.."
		& ..\Fuzzlyn\bin\Release\publish\Fuzzlyn.exe --seed=$seed --reduce --reduce-use-child-processes > "reduced\\$seed.cs"
	}
	Write-Host "  ..done"
}