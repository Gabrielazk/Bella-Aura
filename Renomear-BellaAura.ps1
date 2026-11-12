# Renomeia tudo de "loja" para "BellaAura"
# Rodar na pasta C:\Bella Aura, com o Visual Studio FECHADO
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

# 1. Confere se esta no lugar certo
foreach ($p in "loja\loja.csproj", "loja.Tests\loja.Tests.csproj", "loja.slnx") {
    if (-not (Test-Path $p)) { Write-Host "Nao achei $p. Rode o script em C:\Bella Aura." -ForegroundColor Red; exit 1 }
}

# 2. So continua se nao tiver mudanca sem commit (assim da pra desfazer facil)
git status --porcelain --untracked-files=no { Write-Host "Tem mudanca sem commit. Faca commit/push antes." -ForegroundColor Red; exit 1 }

# 3. Apaga pastas geradas (voltam sozinhas no build)
foreach ($p in ".vs", "loja\bin", "loja\obj", "loja.Tests\bin", "loja.Tests\obj") {
    if (Test-Path $p) { Remove-Item $p -Recurse -Force }
}

# 4. Renomeia arquivos e pastas pelo git (ele entende que e o mesmo arquivo)
git mv "loja\loja.csproj" "loja\BellaAura.csproj"
git mv "loja" "BellaAura"
git mv "loja.Tests\loja.Tests.csproj" "loja.Tests\BellaAura.Tests.csproj"
git mv "loja.Tests" "BellaAura.Tests"
git mv "loja.slnx" "Bella Aura.slnx"

# Arquivos .user (nao vao pro git) tambem mudam de nome
if (Test-Path "BellaAura\loja.csproj.user") { Rename-Item "BellaAura\loja.csproj.user" "BellaAura.csproj.user" }

# 5. Troca os textos dentro dos arquivos
$arquivos = Get-ChildItem "BellaAura", "BellaAura.Tests" -Recurse -File -Include *.cs, *.cshtml, *.csproj, *.json
$arquivos += Get-Item "Bella Aura.slnx"
$utf8 = New-Object System.Text.UTF8Encoding($true)

foreach ($a in $arquivos) {
    $texto = [System.IO.File]::ReadAllText($a.FullName)
    $novo = $texto

    # caminhos dos projetos (no .slnx e na referencia dos testes)
    $novo = $novo.Replace("loja.Tests/loja.Tests.csproj", "BellaAura.Tests/BellaAura.Tests.csproj")
    $novo = $novo.Replace("loja.Tests\loja.Tests.csproj", "BellaAura.Tests\BellaAura.Tests.csproj")
    $novo = $novo.Replace("loja/loja.csproj", "BellaAura/BellaAura.csproj")
    $novo = $novo.Replace("loja\loja.csproj", "BellaAura\BellaAura.csproj")

    # CSS isolado que o _Layout puxa
    $novo = $novo.Replace("loja.styles.css", "BellaAura.styles.css")

    # namespaces (so pega "loja" de codigo, nao o texto "loja" das telas)
    $novo = $novo -creplace '\bnamespace loja\b', 'namespace BellaAura'
    $novo = $novo -creplace '\busing loja\b', 'using BellaAura'
    $novo = $novo -creplace '\bloja\.(?=[A-Z])', 'BellaAura.'

    if ($novo -cne $texto) {
        [System.IO.File]::WriteAllText($a.FullName, $novo, $utf8)
        Write-Host "Alterado: $($a.FullName.Replace($PSScriptRoot + '\', ''))"
    }
}

# 6. Confere se ainda sobrou "loja" em codigo
Write-Host "`nSobrou 'loja' em algum codigo? (vazio = tudo certo)" -ForegroundColor Yellow
Get-ChildItem "BellaAura", "BellaAura.Tests" -Recurse -File -Include *.cs, *.csproj |
    Select-String -Pattern '\bloja\b' -CaseSensitive |
    ForEach-Object { "$($_.Path):$($_.LineNumber)  $($_.Line.Trim())" }

# 7. Compila e roda os testes
Write-Host "`nRodando os testes..." -ForegroundColor Yellow
dotnet test "Bella Aura.slnx"

Write-Host "`nPronto! Abra 'Bella Aura.slnx' no VS, rode o site e, se estiver tudo ok, faca o commit." -ForegroundColor Green
