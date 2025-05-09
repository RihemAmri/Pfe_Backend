$apiUrl = "http://localhost:4011/api/credit/mise-a-jour-automatique"

try {
    $headers = @{
        "Content-Type" = "application/json"
    }
    $response = Invoke-RestMethod -Uri $apiUrl -Method POST -Headers $headers
    Write-Output "✅ Mise à jour réussie : $response"
} catch {
    Write-Output "❌ Erreur lors de l’appel API : $_"
}
