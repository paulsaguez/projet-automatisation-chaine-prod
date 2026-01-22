# Architecture & Flux de Données

Ce document décrit le cheminement complet d'un fichier CSV depuis l'upload utilisateur jusqu'à l'insertion en base de données, démontrant le rôle central du service de traitement Python.

## Schéma du Flux (Data Flow)

```mermaid
sequenceDiagram
    participant User
    participant IHM as Angular (Frontend)
    participant Nginx as Reverse Proxy
    participant Py as Python (Traitement)
    participant API as C# API
    participant DB as MongoDB

    User->>IHM: Sélectionne Fichier CSV

    rect rgb(240, 248, 255)
    note right of IHM: Découpage (Frontend)
    IHM->>IHM: slice(file, 1MB)
    loop Pour chaque morceau (chunk)
        IHM->>Nginx: POST /traitement/upload_chunk
        Nginx->>Py: Proxy Pass (Port 8000)
        Py->>Py: Stockage temporaire /tmp
    end
    end

    rect rgb(255, 250, 240)
    note right of Py: Traitement (Python)
    Py->>Py: Assemblage des morceaux
    Py->>Py: Lecture CSV (pandas)
    Py->>Py: Transformation des données
    end

    rect rgb(240, 255, 240)
    note right of API: Persistance (C# & Mongo)
    Py->>Nginx: POST /api/data/migration
    Nginx->>API: Proxy Pass (Port 5000)
    API->>DB: InsertManyAsync()
    end

    API-->>Py: 201 Created
    Py-->>IHM: JSON "Succès"
    IHM-->>User: Notification "Upload Terminé"
```

## Vérification par le Code

Voici les preuves que le fichier passe bien par le service de traitement (Python).

### 1. Frontend (Angular)

Il n'appelle **jamais** l'API C# directement pour l'upload. Il cible `/traitement/upload_chunk`.

> [!NOTE]
> Fichier : [add-data.component.ts](file:///Users/pauls/Documents/IUT/R5.Real.07 - Automatisation de la chaîne de programmation/Projet/projet-automatisation-chaine-prod/IHM/src/app/add-data/add-data.component.ts)

```typescript
this.http.post("/traitement/upload_chunk", formData);
```

### 2. Reverse Proxy (Nginx)

La route `/traitement/` redirige vers le conteneur Python (`traitement-fastapi`) sur le port 8000.

> [!NOTE]
> Fichier : [nginx.conf](file:///Users/pauls/Documents/IUT/R5.Real.07 - Automatisation de la chaîne de programmation/Projet/projet-automatisation-chaine-prod/nginx/nginx.conf)

```nginx
location /traitement/ {
    proxy_pass http://traitement/;
}
```

### 3. Service de Traitement (Python)

C'est ici que la logique se fait. Le script :

1.  Reçoit les morceaux (`upload_chunk`).
2.  Reconstitue le fichier.
3.  Utilise **pandas** pour lire et nettoyer le CSV.
4.  Envoie les données propres à l'API C#.

> [!NOTE]
> Fichier : [main.py](file:///Users/pauls/Documents/IUT/R5.Real.07 - Automatisation de la chaîne de programmation/Projet/projet-automatisation-chaine-prod/TRAITEMENT/main.py)

```python
# Lecture et Parsing
df = pd.read_csv(final_file_path)

# Envoi à l'API C#
api_endpoint = f"{API_URL}/api/migration"
requests.post(api_endpoint, json=reports)
```

### 4. API Backend (C#)

L'API reçoit des données JSON déjà structurées (et non un fichier brut).

> [!NOTE]
> Fichier : [MigrationController.cs](file:///Users/pauls/Documents/IUT/R5.Real.07 - Automatisation de la chaîne de programmation/Projet/projet-automatisation-chaine-prod/API/Controllers/MigrationController.cs)

```csharp
[HttpPost]
public async Task<IActionResult> Post([FromBody] List<MigrationReport> reports)
```
