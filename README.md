# Projet Automatisation de la Chaîne de Programmation

Ce projet a pour but de gérer la migration et l'analyse de rapports Google Drive à l'aide d'une architecture microservices.

## Équipe

- Théo TORNATORE
- Paul SAGUEZ
  (Groupe BUT3-ALT)

## Architecture

Le projet est composé de plusieurs services orchestrés par Docker Compose :

- **IHM (Angular)** : Interface utilisateur pour visualiser et rechercher les rapports.
- **API (C# .NET 8)** : Backend gérant la communication avec la base de données MongoDB.
- **TRAITEMENT (Python/FastAPI)** : Service d'ingestion et de traitement du fichier CSV source.
- **MongoDB** : Base de données NoSQL pour le stockage des rapports.
- **Nginx** : Reverse proxy exposant l'ensemble des services sur le port 8080.

## Prérequis

- Docker Desktop ou Docker Engine & Docker Compose installés.
- (Optionnel) .NET 8 SDK pour le développement local de l'API.
- (Optionnel) Node.js pour le développement local de l'IHM.

## Installation et Démarrage

1. **Cloner le dépôt** :

   ```bash
   git clone <votre-repo-url>
   cd projet-automatisation-chaine-prod
   ```

2. **Placer le fichier source** :
   Assurez-vous que le fichier `GDriveMigrationReport_20251127140429003_Google Drive objects.csv` est présent à la racine (ou configurez le chemin dans le service de traitement).

3. **Lancer les services** :
   ```bash
   docker-compose up --build
   ```

## Utilisation

Une fois les services démarrés :

- **Application Web** : Accessible sur `http://localhost:8080`.
- **API Swagger** : Documentation de l'API C# accessible sur `http://localhost:5000/swagger` (ou via le proxy si configuré).
- **API Traitement** : Accessible sur `http://localhost:8000/docs` (FastAPI Swagger).

## Endpoints API C# (.NET)

- `POST /api/migration` : Enregistrement de rapports de migration.
- `GET /api/migration/search` : Recherche de rapports par titre ou statut.

## Structure du Projet

- `/API` : Code source C# .NET.
- `/IHM` : Code source Angular.
- `/TRAITEMENT` : Code source Python.
- `/BDD` : Scripts ou documentation liés à la base de données.
- `/nginx` : Configuration du reverse proxy.
