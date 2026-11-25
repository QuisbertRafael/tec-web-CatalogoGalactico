# Compatible con Windows PowerShell 5.1. La API debe estar corriendo.
$base = 'http://localhost:5156'
$pruebas = @('/personajes', '/cartas/1/progresion', '/cartas/1/poder?fecha=3%20ABY', '/personajes/2147483647')
foreach ($ruta in $pruebas) {
    Write-Host "`n=== GET $ruta ===" -ForegroundColor Cyan
    curl.exe -sS -i "$base$ruta"
    Write-Host ""
}
