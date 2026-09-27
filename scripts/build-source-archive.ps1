param(
    [string]$SourceRoot = (Join-Path $PSScriptRoot '..\sample'),
    [string]$ArchivePath = (Join-Path $PSScriptRoot '..\docs\public\sample-source.zip')
)

$ErrorActionPreference = 'Stop'
$source = (Resolve-Path -LiteralPath $SourceRoot).Path
$archive = [System.IO.Path]::GetFullPath($ArchivePath)
$publicDirectory = Split-Path -Parent $archive
[System.IO.Directory]::CreateDirectory($publicDirectory) | Out-Null

$skipDirectories = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
@('bin', 'obj', 'node_modules', 'dist', '.angular', '.vs', '.git') | ForEach-Object { [void]$skipDirectories.Add($_) }
$skipFiles = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
@('EmployeeManagement.Api.csproj.user') | ForEach-Object { [void]$skipFiles.Add($_) }

function Get-SourceFiles([string]$Directory) {
    foreach ($item in [System.IO.Directory]::EnumerateFileSystemEntries($Directory)) {
        if ([System.IO.Directory]::Exists($item)) {
            if (-not $skipDirectories.Contains([System.IO.Path]::GetFileName($item))) {
                Get-SourceFiles $item
            }
        } elseif (-not $skipFiles.Contains([System.IO.Path]::GetFileName($item))) {
            $item
        }
    }
}

Add-Type -AssemblyName System.IO.Compression
if ([System.IO.File]::Exists($archive)) { [System.IO.File]::Delete($archive) }
$stream = [System.IO.File]::Open($archive, [System.IO.FileMode]::CreateNew)
try {
    $zip = [System.IO.Compression.ZipArchive]::new($stream, [System.IO.Compression.ZipArchiveMode]::Create)
    try {
        $count = 0
        foreach ($file in Get-SourceFiles $source) {
            $relative = $file.Substring($source.Length).TrimStart('\', '/').Replace('\', '/')
            $entry = $zip.CreateEntry("sample/$relative", [System.IO.Compression.CompressionLevel]::Optimal)
            $entryStream = $entry.Open()
            try {
                $fileStream = [System.IO.File]::OpenRead($file)
                try { $fileStream.CopyTo($entryStream) } finally { $fileStream.Dispose() }
            } finally { $entryStream.Dispose() }
            $count++
        }
    } finally { $zip.Dispose() }
} finally { $stream.Dispose() }

Write-Output "Archived $count source files to $archive"
